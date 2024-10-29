using Emulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Z80Core
{
    public class Z80Emulator : BaseEmulator
    {
        private Z80EmulatorBuilder builder;
        private Action<Z80Emulator>[] microCode = null;
        private Action<Z80Emulator>[] microCodeCB = null;
        private Action<Z80Emulator>[] microCodeDD = null;
        private Action<Z80Emulator>[] microCodeED = null;
        private Action<Z80Emulator>[] microCodeFD = null;
        private Action<Z80Emulator, int>[] microCodeDDCB = null;
        private Action<Z80Emulator, int>[] microCodeFDCB = null;
        private bool[] parityTable = null;
        private Action<Z80Emulator, byte> handleInt, handleNmi;

        private bool interrupt = false;
        private bool nonMaskableInterrupt = false;
        private byte interruptDataBus = 0xFF;

        private readonly Z80Registers z80Registers;

        public Z80Emulator()
        {
            if (microCode == null)
            {
                builder = new Z80EmulatorBuilder();
                builder.Build();
                microCode = builder.MicroCode;
                microCodeCB = builder.MicroCodeCB;
                microCodeDD = builder.MicroCodeDD;
                microCodeED = builder.MicroCodeED;
                microCodeFD = builder.MicroCodeFD;
                microCodeDDCB = builder.MicroCodeDDCB;
                microCodeFDCB = builder.MicroCodeFDCB;
                parityTable = builder.ParityTable;
                handleInt = builder.HandleInt;
                handleNmi = builder.HandleNmi;
            }
            z80Registers = new Z80Registers();
            registers = z80Registers;
            breakpoints = new Breakpoint[0x10000];
        }

        public TextWriter Debugger { get; set; }

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
            var opCode = z80Registers.NextOpcode ?? ReadMemory(z80Registers.PC++);
            microCode[opCode](this);
            z80Registers.NextOpcode = null;
            if (nonMaskableInterrupt)
            {
                nonMaskableInterrupt = false;
                handleNmi(this, interruptDataBus);
            }
            if (interrupt && z80Registers.Iff1 && !z80Registers.MaskInterruptsNext)
            {
                OnInterruptAcknowledged?.Invoke(this, new EventArgs());
                handleInt(this, interruptDataBus);
            }
            z80Registers.MaskInterruptsNext = false;
            totalStates += z80Registers.States;
        }

        public IEnumerable<(string, Expression)> GetExpressions(byte[] opcodes)
        {
            switch (opcodes[0])
            {
                case 0xCB:
                    return new[] { 
                        ($"microCode[0x{0xCB:X2}]", builder.MicroExpr[0xCB]), 
                        ($"microCodeCB[0x{opcodes[1]:X2}]", builder.MicroExprCB[opcodes[1]]) 
                    };
                case 0xED:
                    return new [] {
                        ($"microCode[0x{0xED:X2}]", builder.MicroExpr[0xED]),
                        ($"microCodeED[0x{opcodes[1]:X2}]", builder.MicroExprED[opcodes[1]])
                    };
                case 0xDD:
                    if (opcodes[1] == 0xCB)
                        return new [] {
                            ($"microCode[0x{0xDD:X2}]", builder.MicroExpr[0xDD]),
                            ($"microCodeDD[0x{0xCB:X2}]", builder.MicroExprDD[0xCB]),
                            ($"microCodeDDCB[0x{opcodes[3]:X2}]", builder.MicroExprDDCB[opcodes[3]])
                        };
                    else
                        return new [] {
                            ($"microCode[0x{0xDD:X2}]", builder.MicroExpr[0xDD]),
                            ($"microCodeDD[0x{opcodes[1]:X2}]", builder.MicroExprDD[opcodes[1]])
                        };
                case 0xFD:
                    if (opcodes[1] == 0xCB)
                        return new [] {
                            ($"microCode[0x{0xFD:X2}]", builder.MicroExpr[0xFD]),
                            ($"microCodeFD[0x{0xCB:X2}]", builder.MicroExprFD[0xCB]),
                            ($"microCodeFDCB[0x{opcodes[3]:X2}]", builder.MicroExprFDCB[opcodes[3]])
                        };
                    else
                        return new [] {
                            ($"microCode[0x{0xFD:X2}]", builder.MicroExpr[0xFD]),
                            ($"microCodeFD[0x{opcodes[1]:X2}]", builder.MicroExprFD[opcodes[1]])
                        };
                default:
                    return new [] {
                        ($"microCode[0x{opcodes[0]:X2}]", builder.MicroExpr[opcodes[0]])
                    };
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
