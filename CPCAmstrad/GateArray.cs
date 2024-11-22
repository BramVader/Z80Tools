using Emulator;
using System.Drawing;

namespace CPCAmstrad
{
    public class GateArray
    {
        private readonly int lrom;
        private readonly int urom;
        private readonly int ram;

        private readonly Color[] colorPalette = new[] 
        {
            Color.FromArgb(0x6E7D6B),
            Color.FromArgb(0x6E7B6D),
            Color.FromArgb(0x00F36B),
            Color.FromArgb(0xF3F36D),
            Color.FromArgb(0x00026B),
            Color.FromArgb(0xF00268),
            Color.FromArgb(0x007868),
            Color.FromArgb(0xF37D6B),
            Color.FromArgb(0xF30268),
            Color.FromArgb(0xF3F36B),
            Color.FromArgb(0xF3F30D),
            Color.FromArgb(0xFFF3F9),
            Color.FromArgb(0xF30506),
            Color.FromArgb(0xF302F4),
            Color.FromArgb(0xF37D0D),
            Color.FromArgb(0xFA80F9),
            Color.FromArgb(0x000268),
            Color.FromArgb(0x02F36B),
            Color.FromArgb(0x02F001),
            Color.FromArgb(0x0FF3F2),
            Color.FromArgb(0x000201),
            Color.FromArgb(0x0C02F4),
            Color.FromArgb(0x027801),
            Color.FromArgb(0x0C7BF4),
            Color.FromArgb(0x690268),
            Color.FromArgb(0x71F36B),
            Color.FromArgb(0x71F504),
            Color.FromArgb(0x71F3F4),
            Color.FromArgb(0x6C0201),
            Color.FromArgb(0x6C02F2),
            Color.FromArgb(0x6E7B01),
            Color.FromArgb(0x6E7BF6)
        };

        private readonly int[] colors = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        private readonly MemoryModel memoryModel;
        private readonly bool[] memorySwitch;

        private int fnReg = 0;
        private int mode = 1;
        private int borderColor;
        private int interruptCounter;
        private bool interruptState;

        public GateArray(MemoryModel memoryModel, bool[] memorySwitch)
        {
            lrom = 0;
            urom = 1;
            ram = 2;
            this.memoryModel = memoryModel;
            this.memorySwitch = memorySwitch;
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

        public Color ColorToRgb(int color)
        {
            return colorPalette[color & 0x1F];
        }

        public void InterruptAcknowledged()
        {
            // When the interrupt is acknowledged, this is sensed by the Gate-Array.
            // The top bit (bit 5), of the counter is set to "0" and the interrupt request is cleared.
            // This prevents the next interrupt from occuring closer than 32 HSYNCs time.
            interruptCounter &= 0x1F;
            interruptState = false;
        }

        private int hSyncCountAfterVSync;
        
        public bool HSyncFallingEdge(bool vSync)
        {
            if (vSync)
                hSyncCountAfterVSync++;
            else
                hSyncCountAfterVSync = 0;
            
            interruptCounter = (interruptCounter + 1) % 52;
            if (interruptCounter == 0 || (hSyncCountAfterVSync == 2 && interruptCounter < 32))
            {
                interruptState = true;
            }
            if (hSyncCountAfterVSync == 2 )
            {
                interruptCounter = 0;
            }
            return interruptState;
        }

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
                        interruptCounter = 0;
                    }
                    break;

            }
        }
    }
}
