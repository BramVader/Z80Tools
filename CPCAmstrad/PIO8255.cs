using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPCAmstrad
{
    public class PIO8255
    {
        public enum Mode : int
        {
            BasicIO = 0,
            StrobedIO = 1,
            Bidirectional = 2
        }

        public enum Direction: int
        {
            Output = 0,
            Input = 1
        }

        private Mode groupAMode, groupBMode;
        private Direction portADirection, portBDirection, portCLowDirection, portCHighDirection;
        private byte portA, portB, portC;

        public byte PortA
        {
            get { return portA; }
            set { portA = value; }
        }

        public byte PortB
        {
            get { return portB; }
            set { portB = value; }
        }

        public byte PortC
        {
            get { return portC; }
            set { portC = value; }
        }

        public byte Read(int address)
        {
            switch (address)
            {
                case 0:
                    return portA;
                case 1:
                    return portB;
                case 2:
                    return portC;
            }
            return 0;
        }

        public void Write(int address, byte data)
        {
            int idata = data;
            switch (address)
            {
                case 0:
                    portA = data;
                    break;
                case 1:
                    portB = data;
                    break;
                case 2:
                    portC = data;
                    break;
                case 3:
                    if ((idata & 0x80) == 0)    // BSR mode
                    {
                        // Bit Set/Reset of portC
                        if ((idata & 0x01) == 0)
                            portC = (byte)(portC & ~(1 << ((idata & 0xE0) >> 1)));
                        else
                            portC = (byte)(portC | (1 << ((idata & 0xE0) >> 1)));
                    }
                    else  // IO mode
                    {
                        // Group B
                        portCLowDirection = (Direction)(idata & 0x01);
                        portBDirection = (Direction)((idata & 0x02) >> 1);
                        groupBMode = (Mode)((idata & 0x04) >> 2);

                        // Group A
                        portCHighDirection = (Direction)((idata & 0x08) >> 3);
                        portADirection = (Direction)((idata & 0x10) >> 4);
                        groupAMode = (Mode)(Math.Min((idata & 0x60) >> 5, 2));

                    }
                    break;
            }
        }

    }
}
