using System;
using System.Linq;
using System.Linq.Expressions;

namespace CPCAmstrad
{
    public class CRTC6845
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
            /* 15 */ nameof(CursorAddressLow),
            /* 16 */ nameof(LightPenAddressHigh),
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

        private double cpuClockFrequencyMHz;
        private long cpuStatesPerSecond;

        private long cpuStatesHSyncCycle;
        private long cpuStatesHSyncStart;
        private long cpuStatesHSyncStop;

        private long cpuStatesVSyncCycle;
        private long cpuStatesVSyncStart;
        private long cpuStatesVSyncStop;

        private int totalwidth, totalheight;
        private int width, height;
        private int rasterLines;

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
                    setters[RegisterSelect](this,value);
                HandleSetting();
            }
        }

        public void CalcSyncs()
        {
            /* Commented values are when CRTC is loaded with default values */

            long syncWidthH = HorizontalAndVerticalSyncWidths & 0xF;    // 14
            if (syncWidthH == 0) syncWidthH = 16;
            long syncWidthV = HorizontalAndVerticalSyncWidths >> 4;     // 8
            if (syncWidthV == 0) syncWidthV = 16;

            totalwidth = (HorizontalTotal + 1) * 16;    // 1024
            width = (HorizontalDisplayed) * 16;         // 640
            rasterLines = (MaximumRasterAddress & 0x07) + 1;    // 8

            cpuStatesHSyncCycle = (HorizontalTotal + 1) * cpuStatesPerSecond / (long)1E6;   // 256
            cpuStatesHSyncStart = HorizontalSyncPosition * cpuStatesPerSecond / (long)1E6;  // 184
            cpuStatesHSyncStop = (syncWidthH * cpuStatesPerSecond / (long)1E6 + cpuStatesHSyncStart) % cpuStatesHSyncCycle;     // 240

            totalheight = ((VerticalTotal & 0x7F) + 1) * rasterLines + (VerticalTotalAdjust & 0x1F);    // 312
            height = (VerticalDisplayed & 0x7F) * rasterLines;      // 200
            cpuStatesVSyncCycle = totalheight * cpuStatesHSyncCycle;        // 79872
            cpuStatesVSyncStart = (VerticalSyncPosition & 0x7F) * ((MaximumRasterAddress & 0x07) + 1) * cpuStatesHSyncCycle;    // 61440
            cpuStatesVSyncStop = (cpuStatesVSyncStart + syncWidthV * cpuStatesHSyncCycle) % cpuStatesVSyncCycle;    // 63488
        }

        public double CpuClockFrequencyMHz
        {
            get { return cpuClockFrequencyMHz; }
            set
            {
                cpuClockFrequencyMHz = value;
                cpuStatesPerSecond = (long)(value * 1E6);
                CalcSyncs();
            }
        }

        public bool GetHSync(long stateCounter)
        {
            long count = stateCounter % cpuStatesHSyncCycle;
            return cpuStatesHSyncStop > cpuStatesHSyncStart ?
                count >= cpuStatesHSyncStart && count < cpuStatesHSyncStop :
                count >= cpuStatesHSyncStart || count < cpuStatesHSyncStop;
        }

        public bool GetVSync(long stateCounter)
        {
            long count = stateCounter % cpuStatesVSyncCycle;
            return cpuStatesVSyncStop > cpuStatesVSyncStart ?
                count >= cpuStatesVSyncStart && count < cpuStatesVSyncStop :
                count >= cpuStatesVSyncStart || count < cpuStatesVSyncStop;
        }

        // Returns -1 when DISPTMG is 1.
        public int RamAddr(long stateCounter)
        {
            int x = (int)((stateCounter % cpuStatesHSyncCycle) * (long)1E6 / cpuStatesPerSecond);
            int y = (int)((stateCounter % cpuStatesVSyncCycle) / cpuStatesHSyncCycle);
            if (x > HorizontalDisplayed) return -1;
            if (y > height) return -1;
            return x + y * HorizontalDisplayed;
        }

        public int VideoPage
        {
            get { return videoPage; }
        }

        public int VideoSize
        {
            get { return videoSize; }
        }

        public int VideoOffset
        {
            get { return videoOffset; }
        }

        protected int videoPage = 0xC000;
        protected int videoSize = 0x4000;
        protected int videoOffset = 0x0000;

        private void HandleSetting()
        {
            videoPage = (DisplayStartAddressHigh & 0x30) << 10;     // 0xC000
            videoSize = (DisplayStartAddressHigh & 0x0C) == 0x0C ? 0x8000 : 0x4000;     // 0x4000
            videoOffset = ((DisplayStartAddressHigh << 8) | DisplayStartAddressLow) & 0x03FF;   // 0x00 / 0x28 / 0x50 ... (incrementing with 40 every vertical scrolling)
            Console.WriteLine($"R12|R13: {DisplayStartAddressHigh:X2}{DisplayStartAddressLow:X2} videoPage: {videoPage:X4} videoOffset: {videoOffset:X4}");
            CalcSyncs();
        }
    }
}
