using Emulator;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace CPCAmstrad
{
    public class GateArray
    {
        private readonly int lrom;
        private readonly int urom;
        private readonly int ram;

        private readonly int[] colorPalette =
        [
            0x6E7D6B,
            0x6E7B6D,
            0x00F36B,
            0xF3F36D,
            0x00026B,
            0xF00268,
            0x007868,
            0xF37D6B,
            0xF30268,
            0xF3F36B,
            0xF3F30D,
            0xFFF3F9,
            0xF30506,
            0xF302F4,
            0xF37D0D,
            0xFA80F9,
            0x000268,
            0x02F36B,
            0x02F001,
            0x0FF3F2,
            0x000201,
            0x0C02F4,
            0x027801,
            0x0C7BF4,
            0x690268,
            0x71F36B,
            0x71F504,
            0x71F3F4,
            0x6C0201,
            0x6C02F2,
            0x6E7B01,
            0x6E7BF6
        ];

        private readonly int[] colors = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        private readonly MemoryModel memoryModel;
        private readonly bool[] memorySwitch;

        private int fnReg = 0;
        private int mode = 1;
        private int borderColor;
        private int r52;
        private bool interruptState;
        private RenderDlg[] renders;
        
        public RenderDlg Render { get; private set; }

        public GateArray(MemoryModel memoryModel, bool[] memorySwitch)
        {
            lrom = 0;
            urom = 1;
            ram = 2;
            this.memoryModel = memoryModel;
            this.memorySwitch = memorySwitch;
            this.renders = [RenderMode0, RenderMode1, RenderMode2];
            Render = RenderMode1;
        }

        public int[] Colors
        {
            get { return colors; }
        }

        public int BorderColor
        {
            get { return borderColor; }
        }

        public int Mode
        {
            get { return mode; }
        }

        public bool InterruptState => interruptState;

        public int ColorToRgb(int color)
        {
            return colorPalette[color & 0x1F];
        }

        public void InterruptAcknowledged()
        {
            // When the interrupt is acknowledged, this is sensed by the Gate-Array.
            // The top bit (bit 5), of the counter is set to "0" and the interrupt request is cleared.
            // This prevents the next interrupt from occuring closer than 32 HSYNCs time.
            r52 &= 0x1F;
            interruptState = false;
        }

        private int hSyncCountAfterVSync;

        public bool HSyncFallingEdge(bool vSync)
        {
            if (vSync)
                hSyncCountAfterVSync++;
            else
                hSyncCountAfterVSync = 0;

            r52++;
            if (r52 > 51 || (hSyncCountAfterVSync == 2 && r52 < 32))
            {
                interruptState = true;
                r52 = 0;
            }
            return interruptState;
        }

        public static int[][] mode0BitMasks =
            [
                Enumerable.Range(0, 256).Select(value => ((value & 0x80) >> 7) | ((value & 0x08) >> 2) | ((value & 0x20) >> 3) | ((value & 0x02) << 2)).ToArray(),
                Enumerable.Range(0, 256).Select(value => ((value & 0x40) >> 6) | ((value & 0x04) >> 1) | ((value & 0x10) >> 2) | ((value & 0x01) << 3)).ToArray()
            ];
        public static int[][] mode1BitMasks =
            [
                Enumerable.Range(0, 256).Select(value => ((value & 0x80) >> 7) | ((value & 0x08) >> 2)).ToArray(),
                Enumerable.Range(0, 256).Select(value => ((value & 0x40) >> 6) | ((value & 0x04) >> 1)).ToArray(),
                Enumerable.Range(0, 256).Select(value => ((value & 0x20) >> 5) | ((value & 0x02) >> 0)).ToArray(),
                Enumerable.Range(0, 256).Select(value => ((value & 0x10) >> 4) | ((value & 0x01) << 1)).ToArray()
            ];
        public static int[][] mode2BitMasks =
            [
                Enumerable.Range(0, 256).Select(value => (value & 0x80) >> 7).ToArray(),
                Enumerable.Range(0, 256).Select(value => (value & 0x40) >> 6).ToArray(),
                Enumerable.Range(0, 256).Select(value => (value & 0x20) >> 5).ToArray(),
                Enumerable.Range(0, 256).Select(value => (value & 0x10) >> 4).ToArray(),
                Enumerable.Range(0, 256).Select(value => (value & 0x08) >> 3).ToArray(),
                Enumerable.Range(0, 256).Select(value => (value & 0x04) >> 2).ToArray(),
                Enumerable.Range(0, 256).Select(value => (value & 0x02) >> 1).ToArray(),
                Enumerable.Range(0, 256).Select(value => (value & 0x01) >> 0).ToArray()
            ];

        public void RenderMode0(int value, int[] buffer, int addr)
        {
            // One byte = 2 pixels in Mode 0 => 8 pixels on result
            for (int n = 0; n < 2; n++)
            {
                int v = value < 0
                    ? colorPalette[BorderColor & 0x1F]
                    : colorPalette[Colors[mode0BitMasks[n][value]] & 0x1F];
                buffer[addr++] = v;
                buffer[addr++] = v;
                buffer[addr++] = v;
                buffer[addr++] = v;
            }
        }

        public void RenderMode1(int value, int[] buffer, int addr)
        {
            // One byte = 4 pixels in Mode 1 => 8 pixels on result
            for (int n = 0; n < 4; n++)
            {
                int v = value < 0
                    ? colorPalette[BorderColor & 0x1F]
                    : colorPalette[Colors[mode1BitMasks[n][value]] & 0x1F];
                buffer[addr++] = v;
                buffer[addr++] = v;
            }
        }

        public void RenderMode2(int value, int[] buffer, int addr)
        {
            // One byte = 8 pixels in Mode 2 => 8 pixels on result
            for (int n = 0; n < 8; n++)
            {
                int v = value < 0
                    ? colorPalette[BorderColor & 0x1F]
                    : colorPalette[Colors[mode2BitMasks[n][value]] & 0x1F];
                buffer[addr++] = v;
            }
        }

        public delegate void RenderDlg(int value, int[] buffer, int addr);


        public void Write(byte value)
        {
            switch (value >> 6)
            {
                case 0:         // Write FN-Reg
                    if ((value & 0x10) == 0)
                        fnReg = value & 0x0F;
                    else
                        fnReg = -1;
                    break;

                case 1:         // Write color to FW-Reg
                    if (fnReg == -1)
                        borderColor = value & 0x1F;
                    else
                        colors[fnReg] = value & 0x1F;
                    break;

                case 2:         // Write to MF-Reg
                    mode = (value & 0x03) % 3;
                    Render = renders[mode];
                    bool lromEnable = (value & 0x04) == 0;
                    bool uromEnable = (value & 0x08) == 0;
                    if (memorySwitch[lrom] != lromEnable || memorySwitch[urom] != uromEnable)
                    {
                        memorySwitch[lrom] = lromEnable;
                        memorySwitch[urom] = uromEnable;
                        memoryModel.SwitchMemory(memorySwitch);
                    }
                    // Reset HSYNC-counter
                    if ((value & 0x10) == 1)
                    {
                        r52 = 0;
                    }
                    break;

            }
        }
    }
}
