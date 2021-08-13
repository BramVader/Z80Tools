using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPCAmstrad
{
    public class AY3_8912
    {
        private int regNr = 0;

        private byte[] registers = new byte[15];

        public byte Input 
        {
            get { return registers[14]; }
            set { registers[14] = value; }
        }

        public void Write(bool BC1, bool BC2, bool BDIR, byte data)
        {
            switch (
                (BDIR ? 4 : 0) |
                (BC2 ? 2 : 0) |
                (BC1 ? 1 : 0)
            )
            {
                case 1:
                case 4:
                case 7:     // Latch address
                    regNr = data;
                    break;

                case 3:     // Read from PSG

                    break;

                case 6:     // Write to PSG
                    if (regNr >= 0 && regNr < 15)
                        registers[regNr] = data;
                    break;
            }
        }

        public byte Read(bool BC1, bool BC2, bool BDIR)
        {
            switch (
                (BDIR ? 4 : 0) |
                (BC2 ? 2 : 0) |
                (BC1 ? 1 : 0)
            )
            {
                case 1:
                case 4:
                case 7:     // Latch address
                    break;

                case 3:     // Read from PSG
                    if (regNr >= 0 && regNr < 15)
                        return registers[regNr];
                    break;

                case 6:     // Write to PSG
                    break;
            }
            return 0xFF;
        }

        private void HandleSetting()
        {

        }
    }
}
