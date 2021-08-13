using System;
using System.Linq.Expressions;
using System.Threading;
using Emulator;
using Assembler;

namespace Z80Core
{
    public class Z80Emulator: BaseEmulator
    {
        private static Z80EmulatorBuilder builder;
        private static Action<Z80Emulator>[] microCode = null;
        private static bool[] parityTable = null;
        private static Action<Z80Emulator, byte> handleInt, handleNmi;

        private bool interrupt = false;
        private bool nonMaskableInterrupt = false;
        private byte interruptDataBus = 0xFF;

        private Z80Registers z80Registers;

        public Z80Emulator()
        {
            if (microCode == null)
            {
                builder = new Z80EmulatorBuilder();
                microCode = builder.MicroCode;
                parityTable = builder.ParityTable;
                handleInt = builder.HandleInt;
                handleNmi = builder.HandleNmi;
            }
            z80Registers = new Z80Registers();
            registers = z80Registers;
            breakpoints = new Breakpoint[0x10000];
        }

        /// <summary>
        /// Simulates low state of INT-input (not edge-triggered!)
        /// </summary>
        public bool Interrupt 
        {
            get { return interrupt; }
            set { interrupt = value; }
        }

        /// <summary>
        /// Simulates a value on the databus during execution of interrupt
        /// </summary>
        public byte InterruptDataBus
        {
            get { return interruptDataBus; }
            set { interruptDataBus = value; }
        }

        /// <summary>
        /// Simulates falling edge of NMI-input (edge-triggered)
        /// </summary>
        public bool NonMaskableInterrupt
        {
            get { return nonMaskableInterrupt; }
            set { nonMaskableInterrupt = value; }
        }

        /// <summary>
        /// Called before a maskable interrupt is granted
        /// </summary>
        public event EventHandler OnInterruptAcknowledged;

        public override void Emulate()
        {
            microCode[z80Registers.NextOpcode ?? ReadMemory(z80Registers.PC++)](this);
            z80Registers.NextOpcode = null;
            if (nonMaskableInterrupt)
            {
                nonMaskableInterrupt = false;
                handleNmi(this, interruptDataBus);
            }
            if (interrupt && z80Registers.Iff1 && !z80Registers.MaskInterruptsNext)
            {
                if (OnInterruptAcknowledged != null)
                    OnInterruptAcknowledged(this, new EventArgs());
                handleInt(this, interruptDataBus);
            }
            z80Registers.MaskInterruptsNext = false;
            totalStates += z80Registers.States;
        }

        public Expression GetExpression(byte[] opcodes)
        {
            switch (opcodes[0])
            {
                case 0xCB:
                    return builder.MicroExprCB[opcodes[1]];
                case 0xED:
                    return builder.MicroExprED[opcodes[1]];
                case 0xDD:
                    if (opcodes[1] == 0xCB)
                        return builder.MicroExprDDCB[opcodes[3]];
                    else
                        return builder.MicroExprDD[opcodes[1]];
                case 0xFD:
                    if (opcodes[1] == 0xCB)
                        return builder.MicroExprFDCB[opcodes[3]];
                    else
                        return builder.MicroExprFD[opcodes[1]];        
                default:
                    return builder.MicroExpr[opcodes[0]];
            }
        }

        public override void Reset()
        {
            z80Registers.Reset();
            interrupt = false;
            nonMaskableInterrupt = false;
            interruptDataBus = 0xFF;
        }
    }
}
