using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Emulator;

namespace Z80Core
{
    [Flags]
    public enum Flags : byte
    {
        CY = 0x01,     // Carry flag (Bit 0)
        N = 0x02,     // Add/Subtract flag (Bit 1)
        PV = 0x04,     // Parity/Overflow flag (Bit 2, V=overflow)
        X1 = 0x08,     // Not used (Bit 3) TODO: should be bit3 of the result
        HC = 0x10,     // Half Carry flag (Bit 4)
        X2 = 0x20,     // Not used (Bit 5) TODO: should be bit5 of the result
        Z = 0x40,     // Zero flag (Bit 6)
        S = 0x80      // Sign flag (Bit 7)
    }

    public class Z80Registers : BaseRegisters
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool flagCY;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool flagN;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool flagPV;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool flagHC;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool flagZ;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool flagS;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool flagX1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool flagX2;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte a;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort bc;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort de;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort hl;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort ix;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort iy;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort sp;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort af_;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort bc_;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort de_;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ushort hl_;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool iff1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool iff2;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int im;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte i;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte r;


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool takeStatesLow;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Timing timing;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool maskInterruptsNext;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? nextOpcode;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool halted;

        public Z80Registers()
            : base()
        {
            sp = 0xFFFF;
            AF = 0xFFFF;
        }

        public void Reset()
        {
            AF = 0xFFFF;
            bc = 0;
            de = 0;
            hl = 0;

            sp = 0xFFFF;
            ix = 0;
            iy = 0;
            pc = 0;

            iff1 = false;
            iff2 = false;
            im = 0;
            i = 0;
            r = 0;
            takeStatesLow = false;
            maskInterruptsNext = false;
            nextOpcode = null;
            halted = false;
        }

        public void ExAf()
        {
            ushort saveAF = AF;
            AF = af_;
            af_ = saveAF;
        }

        public void Exx()
        {
            ushort save;
            save = bc; bc = bc_; bc_ = save;
            save = de; de = de_; de_ = save;
            save = hl; hl = hl_; hl_ = save;
        }

        public bool CY
        {
            get { return flagCY; }
            set { flagCY = value; }
        }

        public bool N
        {
            get { return flagN; }
            set { flagN = value; }
        }

        public bool PV
        {
            get { return flagPV; }
            set { flagPV = value; }
        }

        public bool HC
        {
            get { return flagHC; }
            set { flagHC = value; }
        }

        public bool Z
        {
            get { return flagZ; }
            set { flagZ = value; }
        }

        public bool S
        {
            get { return flagS; }
            set { flagS = value; }
        }

        public byte A
        {
            get { return a; }
            set { a = value; }
        }

        public byte F
        {
            get
            {
                return (byte)(
                    (flagCY ? (int)Flags.CY : 0) |
                    (flagN ? (int)Flags.N : 0) |
                    (flagPV ? (int)Flags.PV : 0) |
                    (flagHC ? (int)Flags.HC : 0) |
                    (flagZ ? (int)Flags.Z : 0) |
                    (flagS ? (int)Flags.S : 0) |
                    (flagX1 ? (int)Flags.X1 : 0) |
                    (flagX2 ? (int)Flags.X2 : 0)
                );
            }
            set
            {
                int val = (int)value;
                flagCY = (val & (int)Flags.CY) != 0;
                flagN = (val & (int)Flags.N) != 0;
                flagPV = (val & (int)Flags.PV) != 0;
                flagHC = (val & (int)Flags.HC) != 0;
                flagZ = (val & (int)Flags.Z) != 0;
                flagS = (val & (int)Flags.S) != 0;
                flagX1 = (val & (int)Flags.X1) != 0;
                flagX2 = (val & (int)Flags.X2) != 0;
            }
        }

        public ushort BC
        {
            get { return bc; }
            set { bc = value; }
        }

        public ushort DE
        {
            get { return de; }
            set { de = value; }
        }

        public ushort HL
        {
            get { return hl; }
            set { hl = value; }
        }

        public byte B
        {
            get { return (byte)(bc >> 8); }
            set { bc = (ushort)(bc & 0x00FF | (value << 8)); }
        }

        public byte C
        {
            get { return (byte)bc; }
            set { bc = (ushort)(bc & 0xFF00 | value); }
        }

        public byte D
        {
            get { return (byte)(de >> 8); }
            set { de = (ushort)(de & 0x00FF | (value << 8)); }
        }

        public byte E
        {
            get { return (byte)de; }
            set { de = (ushort)(de & 0xFF00 | value); }
        }

        public byte H
        {
            get { return (byte)(hl >> 8); }
            set { hl = (ushort)(hl & 0x00FF | (value << 8)); }
        }

        public byte L
        {
            get { return (byte)hl; }
            set { hl = (ushort)(hl & 0xFF00 | value); }
        }

        public ushort AF
        {
            get { return (ushort)((int)a << 8 | (int)F); }
            set { a = (byte)(value >> 8); F = (byte)(value & 0x00FF); }
        }

        public ushort IX
        {
            get { return ix; }
            set { ix = value; }
        }

        public ushort IY
        {
            get { return iy; }
            set { iy = value; }
        }

        public byte IXH
        {
            get { return (byte)(ix >> 8); }
            set { ix = (ushort)(ix & 0x00FF | (value << 8)); }
        }

        public byte IXL
        {
            get { return (byte)ix; }
            set { ix = (ushort)(ix & 0xFF00 | value); }
        }

        public byte IYH
        {
            get { return (byte)(iy >> 8); }
            set { iy = (ushort)(iy & 0x00FF | (value << 8)); }
        }

        public byte IYL
        {
            get { return (byte)iy; }
            set { iy = (ushort)(iy & 0xFF00 | value); }
        }

        public ushort SP
        {
            get { return sp; }
            set { sp = value; }
        }

        /// <summary>
        /// Interrupt flip flop 1
        /// </summary>
        public bool Iff1
        {
            get { return iff1; }
            set { iff1 = value; }
        }

        /// <summary>
        /// Interrupt flip flop 2
        /// </summary>
        public bool Iff2
        {
            get { return iff2; }
            set { iff2 = value; }
        }

        /// <summary>
        /// Interrupt Mode
        /// </summary>
        public int IM
        {
            get { return im; }
            set { im = value; }
        }

        /// <summary>
        /// Interrupt Control Vector Register
        /// </summary>
        public byte I
        {
            get { return i; }
            set { i = value; }
        }

        /// <summary>
        /// Memory Refresh Register
        /// </summary>
        public byte R
        {
            get { return r; }
            set { r = value; }
        }

        /// <summary>
        /// Set by the HALT-instruction, Reset by interrupt
        /// </summary>
        public bool Halted
        {
            get { return halted; }
            set { halted = value; }
        }

        /// <summary>
        /// The timing of the current instruction
        /// </summary>
        internal Timing Timing
        {
            get { return timing; }
            set
            {
                takeStatesLow = false;
                timing = value;
            }
        }

        /// <summary>
        /// The actual number of states the instruction has taken
        /// </summary>
        public int States
        {
            get { return takeStatesLow ? timing.StatesLow : timing.StatesNormal; }
        }

        /// <summary>
        /// Indicates that the instruction took the lower value of states
        /// </summary>
        internal bool TakeStatesLow
        {
            get { return takeStatesLow; }
            set { takeStatesLow = value; }
        }

        /// <summary>
        /// Mask interrupt next instruction
        /// </summary>
        internal bool MaskInterruptsNext
        {
            get { return maskInterruptsNext; }
            set { maskInterruptsNext = value; }
        }

        /// <summary>
        /// Next opcode, generated by interrupt
        /// </summary>
        internal int? NextOpcode
        {
            get { return nextOpcode; }
            set { nextOpcode = value; }
        }

        /// <summary>
        /// Clones the register set to another, already created set
        /// </summary>
        /// <param name="regs"></param>
        public override void CloneTo(object regs)
        {
            if (regs is Z80Registers z80regs)
            {
                z80regs.ix = ix;
                z80regs.iy = iy;
                z80regs.sp = sp;
                z80regs.pc = pc;
                z80regs.iff1 = iff1;
                z80regs.iff2 = iff2;
                z80regs.im = im;
                z80regs.i = i;
                z80regs.r = r;

                z80regs.a = a;
                z80regs.bc = bc;
                z80regs.de = de;
                z80regs.hl = hl;
                z80regs.AF = AF;

                z80regs.af_ = af_;
                z80regs.bc_ = bc_;
                z80regs.de_ = de_;
                z80regs.hl_ = hl_;

            }
        }
    }
}
