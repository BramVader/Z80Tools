using Emulator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Z80Core
{
    public class Z80Emulator : BaseEmulator
    {
        internal readonly Z80EmulatorBuilder builder;
        internal readonly Action<Z80Emulator>[] microCode;
        internal readonly Action<Z80Emulator>[] microCodeCB;
        internal readonly Action<Z80Emulator>[] microCodeDD;
        internal readonly Action<Z80Emulator>[] microCodeED;
        internal readonly Action<Z80Emulator>[] microCodeFD;
        internal readonly Action<Z80Emulator, int>[] microCodeDDCB;
        internal readonly Action<Z80Emulator, int>[] microCodeFDCB;
        internal readonly bool[] parityTable;

        private bool interrupt = false;
        private bool nonMaskableInterrupt = false;

        private readonly Z80Registers z80Registers;

        public Z80Emulator()
            : this(null)
        {
        }

        public Z80Emulator(HardwareModel hardwareModel)
            : base(hardwareModel)
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
        /// Simulates falling edge of NMI-input (edge-triggered)
        /// </summary>
        public bool NonMaskableInterrupt
        {
            get { return nonMaskableInterrupt; }
            set { nonMaskableInterrupt = value; }
        }

        public override void Emulate()
        {
            var opCode = z80Registers.NextOpcode ?? ReadMemory(z80Registers.PC++);
            if (opCode == 0xF3 /* DI */)
                z80Registers.MaskInterruptsNext = true;
            microCode[opCode](this);
            z80Registers.NextOpcode = null;
            if (nonMaskableInterrupt)
            {
                // detect edge
                nonMaskableInterrupt = false;
                byte dataOnBus = hardwareModel.GetDataOnBus();
                z80Registers.R = z80Registers.R + 1;

                if (z80Registers.Halted)
                {
                    z80Registers.PC = z80Registers.PC + 1;
                    z80Registers.Halted = false;
                }
                z80Registers.Iff2 = z80Registers.Iff1;
                z80Registers.Iff1 = false;
                WriteMemory.Invoke(--z80Registers.SP, (byte)(z80Registers.PC >> 8));
                WriteMemory.Invoke(--z80Registers.SP, (byte)(z80Registers.PC & 255));
                z80Registers.PC = 0x66;
                z80Registers.Timing = new Timing { StatesNormal = 11, StatesLow = 11 };

            }
            //Accept an incoming interrupt
            if (interrupt && z80Registers.Iff1 && !z80Registers.MaskInterruptsNext)
            {
                byte dataOnBus = hardwareModel.GetDataOnBus();
                hardwareModel.InterruptAcknowledged();

                z80Registers.R++;
                if (z80Registers.Halted)
                {
                    z80Registers.PC++;
                    z80Registers.Halted = false;
                }
                z80Registers.Iff1 = false;
                z80Registers.Iff2 = false;

                switch (z80Registers.IM)
                {
                    case 0:     // IM0
                        z80Registers.NextOpcode = dataOnBus;
                        z80Registers.Timing = new Timing { StatesNormal = 13, StatesLow = 13 };
                        break;
                    case 1:     // IM1
                        z80Registers.NextOpcode = 0xFF;  // RST 0x0038
                        z80Registers.Timing = new Timing { StatesNormal = 13, StatesLow = 13 };
                        break;
                    default:    // IM2
                        WriteMemory.Invoke(--z80Registers.SP, (byte)(z80Registers.PC >> 8));
                        WriteMemory.Invoke(--z80Registers.SP, (byte)(z80Registers.PC & 0xFF));
                        z80Registers.NextOpcode = ReadMemory.Invoke((z80Registers.I << 8) | (dataOnBus & 0xFE));
                        z80Registers.Timing = new Timing { StatesNormal = 19, StatesLow = 19 };
                        break;
                }
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
                    return new[] {
                        ($"microCode[0x{0xED:X2}]", builder.MicroExpr[0xED]),
                        ($"microCodeED[0x{opcodes[1]:X2}]", builder.MicroExprED[opcodes[1]])
                    };
                case 0xDD:
                    if (opcodes[1] == 0xCB)
                        return new[] {
                            ($"microCode[0x{0xDD:X2}]", builder.MicroExpr[0xDD]),
                            ($"microCodeDD[0x{0xCB:X2}]", builder.MicroExprDD[0xCB]),
                            ($"microCodeDDCB[0x{opcodes[3]:X2}]", builder.MicroExprDDCB[opcodes[3]])
                        };
                    else
                        return new[] {
                            ($"microCode[0x{0xDD:X2}]", builder.MicroExpr[0xDD]),
                            ($"microCodeDD[0x{opcodes[1]:X2}]", builder.MicroExprDD[opcodes[1]])
                        };
                case 0xFD:
                    if (opcodes[1] == 0xCB)
                        return new[] {
                            ($"microCode[0x{0xFD:X2}]", builder.MicroExpr[0xFD]),
                            ($"microCodeFD[0x{0xCB:X2}]", builder.MicroExprFD[0xCB]),
                            ($"microCodeFDCB[0x{opcodes[3]:X2}]", builder.MicroExprFDCB[opcodes[3]])
                        };
                    else
                        return new[] {
                            ($"microCode[0x{0xFD:X2}]", builder.MicroExpr[0xFD]),
                            ($"microCodeFD[0x{opcodes[1]:X2}]", builder.MicroExprFD[opcodes[1]])
                        };
                default:
                    return new[] {
                        ($"microCode[0x{opcodes[0]:X2}]", builder.MicroExpr[opcodes[0]])
                    };
            }
        }

        public override void Reset()
        {
            z80Registers.Reset();
            interrupt = false;
            nonMaskableInterrupt = false;
        }
    }
}
