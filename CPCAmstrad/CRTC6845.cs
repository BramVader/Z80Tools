using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CPCAmstrad
{
    public class CRTC6845 : IDisposable
    {
        private static readonly string[] regNames = [
            /* 00 */ nameof(HorizontalTotal),
            /* 01 */ nameof(HorizontalDisplayed),
            /* 02 */ nameof(HorizontalSyncPosition),
            /* 03 */ nameof(HorizontalAndVerticalSyncWidths),
            /* 04 */ nameof(VerticalTotal),
            /* 05 */ nameof(VerticalTotalAdjust),
            /* 06 */ nameof(VerticalDisplayed),
            /* 07 */ nameof(VerticalSyncPosition),
            /* 08 */ nameof(InterlaceAndSkew),
            /* 09 */ nameof(MaximumRasterAddress),
            /* 10 */ nameof(CursorStartRaster),
            /* 11 */ nameof(CursorEndRaster),
            /* 12 */ nameof(DisplayStartAddressHigh),
            /* 13 */ nameof(DisplayStartAddressLow),
            /* 14 */ nameof(CursorAddressHigh),
            /* 16 */ nameof(LightPenAddressHigh),
            /* 15 */ nameof(CursorAddressLow),
            /* 17 */ nameof(LightPenAddressLow)
        ];

        private static readonly ParameterExpression par1 = Expression.Parameter(typeof(CRTC6845), "it");
        private static readonly ParameterExpression par2 = Expression.Parameter(typeof(int), "value");
        private static readonly Action<CRTC6845, int>[] setters = regNames
            .Select(x =>
                Expression.Lambda<Action<CRTC6845, int>>(
                    Expression.Assign(Expression.Property(par1, x), par2),
                    par1, par2
                ).Compile()
            ).ToArray();
        private static readonly Func<CRTC6845, int>[] getters = regNames
            .Select(x =>
                Expression.Lambda<Func<CRTC6845, int>>(
                    Expression.Property(par1, x),
                    par1
                ).Compile()
            ).ToArray();

        private CPC464Model hardwareModel;
        private readonly GateArray gateArray;
        private readonly bool[] ramEnabled;
        private long stateCounter = 0L;

        private int c0 = 0;        // Horizontal char counter (0..63, where 0..39 is chars, the rest is border)
        private int c3l = 0;       // Horizontal sync counter
        private int c3h = 0;       // Vertical sync counter
        private int c4 = 0;        // Vertical char counter (0..38, where 0..24 is chars, the rest is border)
        private int c5 = 0;        // Total adjust counter (extra scanlines)
        private int c9 = 0;        // Raster line counter (0..7)
        private int ma = 0x3000;   // The CRTC memory address, increasing each clock
        private int lma = 0x3000;  // Storage of ma at the start of each line
        private bool verticalTotalAdjusting = false;    // In adjust mode: rendering VerticalTotalAdjust scanlines
        private bool hsync = false;
        private bool vsync = false;
        private int scanline = 0;
        private int divider = 0;
        private bool dispenHor = true;
        private bool dispenVer = true;
        private int width;
        private int height;
        private int offsx;
        private int offsy;
        private int vscanStart;
        private int vscanEnd;
        private int hscanStart;
        private int hscanEnd;

        private int[] screenBuffer = null;
        private Bitmap screenBitmap = null;

        private AutoResetEvent renderRequest = new(false);
        private CancellationTokenSource cts = new();

        public CRTC6845(CPC464Model hardwareModel)
        {
            this.hardwareModel = hardwareModel;
            this.gateArray = hardwareModel.GateArray;
            this.ramEnabled = [false, false, true];
            Precalc();
            Task.Run(() => RenderLoop(cts.Token));
        }

        // 00: Width of the screen, in characters. Should always be 63 (64 characters). 1 character == 1μs.
        public int HorizontalTotal { get; set; } = 63;

        // 01: Number of characters displayed. Once horizontal character count (HCC) matches this value, DISPTMG is set to 1.
        public int HorizontalDisplayed { get; set; } = 40;

        // 02: When to start the HSync signal.
        public int HorizontalSyncPosition { get; set; } = 46;

        // 03: HSync pulse width in characters (0 means 16 on some CRTC), should always be more than 8;
        //     VSync width in scan-lines. (0 means 16 on some CRTC. Not present on all CRTCs, fixed to 16 lines on these)
        public int HorizontalAndVerticalSyncWidths { get; set; } = 0x8E;

        // 04: Height of the screen, in characters.
        public int VerticalTotal { get; set; } = 38;

        // 05: Measured in scanlines, can be used for smooth vertical scrolling on CPC.
        public int VerticalTotalAdjust { get; set; } = 0;

        // 06: Height of displayed screen in characters. Once vertical character count (VCC) matches this value, DISPTMG is set to 1.
        public int VerticalDisplayed { get; set; } = 25;

        // 07: When to start the VSync signal, in characters.
        public int VerticalSyncPosition { get; set; } = 30;

        // 08: 00: No interlace; 01: Interlace Sync Raster Scan Mode; 10: No Interlace; 11: Interlace Sync and Video Raster Scan Mode
        public int InterlaceAndSkew { get; set; } = 0;

        // 09: Maximum scan line address on CPC can hold between 0 and 7, higher values' upper bits are ignored
        public int MaximumRasterAddress { get; set; } = 7;

        // 10: Cursor not used on CPC. B = Blink On/Off; P = Blink Period Control (Slow/Fast). Sets first raster row of character that cursor is on to invert.
        public int CursorStartRaster { get; set; } = 0;

        // 11: Sets last raster row of character that cursor is on to invert
        public int CursorEndRaster { get; set; } = 0;

        // 12: Allows you to offset the start of screen memory, useful when using memory from address &0000.
        public int DisplayStartAddressHigh { get; set; } = 48;

        // 13: Allows you to offset the start of screen memory, useful when using memory from address &0000.
        public int DisplayStartAddressLow { get; set; } = 0;

        // 14: Cursor not supported on CPC
        public int CursorAddressHigh { get; set; }

        // 15: Cursor not supported on CPC
        public int CursorAddressLow { get; set; }

        // 16:
        public int LightPenAddressHigh { get; set; }

        // 17:
        public int LightPenAddressLow { get; set; }


        public int RegisterSelect { get; set; }

        public int RegisterValue
        {
            get
            {
                if (RegisterSelect >= 0 && RegisterSelect < 18)
                    return getters[RegisterSelect](this);
                return 0xFF;
            }
            set
            {
                if (RegisterSelect >= 0 && RegisterSelect < 18)
                {
                    setters[RegisterSelect](this, value);
                    Precalc();
                }
            }
        }

        private void Precalc()
        {
            width = (HorizontalTotal + 1) * 16;
            height = (VerticalTotal + 1) * (MaximumRasterAddress + 1);
            offsx = ((HorizontalTotal + 1) - (HorizontalSyncPosition + 2)) / 2;
            offsy = 6 * (MaximumRasterAddress + 1);

            // Note: Certain CRTC's do not support setting r3h and will be initialized as 0
            // However, a value of 0 is interpreted as 16
            int vscanWidth = HorizontalAndVerticalSyncWidths >> 4;  // r3h: default 8
            if (vscanWidth == 0) vscanWidth = 16;
            vscanStart = VerticalSyncPosition * (MaximumRasterAddress + 1);
            vscanEnd = vscanStart + vscanWidth;

            hscanStart = HorizontalSyncPosition; // r2: default 46
            hscanEnd = HorizontalSyncPosition + (HorizontalAndVerticalSyncWidths & 0x0F);  // r3l: default 14
        }

        private void RenderLoop(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                renderRequest.WaitOne();
                if (screenBuffer != null)
                {
                    if (screenBitmap == null || screenBitmap.Width != width || screenBitmap.Height != height)
                    {
                        screenBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    }
                    var data = screenBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                    Marshal.Copy(screenBuffer, 0, data.Scan0, screenBuffer.Length);
                    screenBitmap.UnlockBits(data);
                    hardwareModel.CPCScreen.InvokeRender(screenBitmap);
                }
            }
        }

        // :15:14|13:12:11|10: 9: 8: 7: 6: 5: 4: 3: 2: 1| 0|
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // | MA  |   RA   |              MA             | X|
        // |13:12| 2: 1: 0| 9: 8: 7: 6: 5: 4: 3: 2: 1: 0| 0|
        //
        // CRTC wiring: MA0..MA9   (char address 0..40x25) connected to A1..A10
        //              RA0..R2    (char generator 0..7) connected to A11..A13
        //              MA12..MA13 (memory page/bank) connected to A14..A15
        //
        // CRTC scans the screen (CRT) in horizontal lines, top to bottom and it includes
        // the non-display area (border). Each horizontal line has the width of 64 chars
        // (=64µs -> HorizontalTotal|r0) of which 40 chars (HorizontalDisplayed|r1) are
        // visible. During the scanning of one line, c0 [0..63] increments 64 times followed by
        // an increment of c9 [0..7].
        // This repeats until c9 exceeds 7 (MaximumRasterAddress|r9) (one line of text rendered).
        // Both counters c0 and c9 are reset and the line counter c4 [0..38] is incremented.
        // The whole process is repeated until c4 exceeds 38 (VerticalTotal|r4) .
        // After that, extra scanlines are rendered, depending on VerticalTotalAdjust|r5. When 0
        // or 16, no extra scanlines are rendered.
        // 
        // Memory offset "ma" is the value of r12/r13, initialized at the start of the frame and has
        // default value of 0x3000 and increments with 40, everytime there is a vertical scroll.
        // Since bit 4 and 5 of r12 (e.g. bit 12 and 13 of ma) are connected to A14/A15, this means a
        // memory offset of 0xC000.
        //
        // Calculation of address:
        //    MA0..MA9 = ma + c0 + c4 * (HorizontalTotal|r0 + 1)
        //    RA0..RA2 = c9
        // 
        // Since the CRTC addresses also the non-display area, the address runs from ma to ma+64x39 = 0xC000..0xC9C0
        // and since only the lower 10 bits are used, this means 0x0000..0x1C0 and never exceeds 0x3FF.
        // Also memory during the non-display area is addressed but rendered as 'border' by the Gate Array.
        // Adjusting HorizontalDisplayed / VerticalDisplayed can display these areas as well.

        public void Simulate(long stateCounter)
        {
            while (this.stateCounter < stateCounter)
            {
                divider = (divider + 1) % 4; // Divide frequency by 4
                if (divider == 0)
                {
                    bool dispen = dispenHor && dispenVer;

                    // The video RAM address, see comment above
                    int vma = ((ma & 0x3000) << 2) |     // 0000, 4000, 8000 or C000
                        ((c9 & 0x07) << 11) |            // rasterline 0..7
                        ((ma & 0x3FF) << 1);             // char address 

                    if (screenBuffer == null || screenBuffer.Length != width * height)
                        screenBuffer = new int[width * height];

                    // Calculate syncs
                    hsync =
                        c0 >= hscanStart &&
                        c0 < hscanEnd;

                    vsync = 
                        scanline >= vscanStart && 
                        scanline < vscanEnd;

                    // Rendering 16 pixels/2 bytes (always 16, regardsless of mode)
                    int x = ((c0 + offsx) % (HorizontalTotal + 1)) << 4;
                    int y = (scanline + offsy) % height;
                    int addr = y * width + x;
                    int value = dispen ? hardwareModel.MemoryModel.Read(vma++, ramEnabled) : -1;
                    gateArray.Render(value, screenBuffer, addr);
                    value = dispen ? hardwareModel.MemoryModel.Read(vma++, ramEnabled) : -1;
                    gateArray.Render(value, screenBuffer, addr + 8);

                    // Increment counters
                    c0++;
                    if (c0 > HorizontalTotal)   // r0: default 63
                    {
                        c0 = 0;
                        dispenHor = true;
                        if (!verticalTotalAdjusting)
                        {
                            // Raster line counter
                            scanline++;
                            c9++;
                            if (c9 > MaximumRasterAddress)     // r9: default 7
                            {
                                c9 = 0;

                                // Vertical counter
                                lma = ma;
                                c4++;
                                if (c4 == VerticalDisplayed) dispenVer = false;

                                if (c4 > VerticalTotal) // r4: default 38
                                {
                                    c4 = 0;
                                    verticalTotalAdjusting = true;
                                }
                            }
                            else
                            {
                                ma = lma;
                            }
                        }
                        else
                        {
                            if (c5 == VerticalTotalAdjust) // r5: default 0
                            {
                                c5 = 0;     // Reset adjust counter
                                scanline = 0;
                                dispenVer = true;
                                verticalTotalAdjusting = false;
                                lma = ma = (DisplayStartAddressHigh << 8) | DisplayStartAddressLow;
                                renderRequest.Set();
                            }
                            else
                            {
                                c5++;
                            }
                        }
                    }
                    else
                    {
                        if (dispenHor) ma++;
                        if (c0 == HorizontalDisplayed) dispenHor = false;
                    }
                }
                this.stateCounter++;
            }
        }

        public void Dispose()
        {
            cts.Cancel();
        }

        public bool HSync => hsync;

        public bool VSync => vsync;
    }
}
