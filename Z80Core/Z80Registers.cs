using Emulator;
using System;
using System.Diagnostics;

namespace Z80Core
{
    [Flags]
    public enum Flags : int
    {
        CY = 0x01,     // Carry flag (Bit 0)
        N = 0x02,     // Add/Subtract flag (Bit 1)
        PV = 0x04,     // Parity/Overflow flag (Bit 2, V=overflow)
        X1 = 0x08,     // Not used (Bit 3) - Contains Bit 3 of the result
        HC = 0x10,     // Half Carry flag (Bit 4)
        X2 = 0x20,     // Not used (Bit 5) - Contains Bit 5 of the result 
        Z = 0x40,     // Zero flag (Bit 6)
        S = 0x80      // Sign flag (Bit 7)
    }

    public class Z80Registers : BaseRegisters
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Flags f;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int a;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int bc;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int de;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int hl;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int ix;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int iy;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int sp;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int af_;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int bc_;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int de_;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int hl_;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool iff1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool iff2;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int im;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int i;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int r;


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
            int saveAF = AF;
            AF = af_;
            af_ = saveAF;
        }

        public void Exx()
        {
            int save;
            save = bc; bc = bc_; bc_ = save;
            save = de; de = de_; de_ = save;
            save = hl; hl = hl_; hl_ = save;
        }

        public bool CY
        {
            get { return f.HasFlag(Flags.CY); }
            set { f = value ? f | Flags.CY : f & ~Flags.CY; }
        }

        public bool N
        {
            get { return f.HasFlag(Flags.N); }
            set { f = value ? f | Flags.N : f & ~Flags.N; }
        }

        public bool PV
        {
            get { return f.HasFlag(Flags.PV); }
            set { f = value ? f | Flags.PV : f & ~Flags.PV; }
        }

        public bool HC
        {
            get { return f.HasFlag(Flags.HC); }
            set { f = value ? f | Flags.HC : f & ~Flags.HC; }
        }

        public bool Z
        {
            get { return f.HasFlag(Flags.Z); }
            set { f = value ? f | Flags.Z : f & ~Flags.Z; }
        }

        public bool S
        {
            get { return f.HasFlag(Flags.S); }
            set { f = value ? f | Flags.S : f & ~Flags.S; }
        }

        public bool X1
        {
            get { return f.HasFlag(Flags.X1); }
            set { f = value ? f | Flags.X1 : f & ~Flags.X1; }
        }

        public bool X2
        {
            get { return f.HasFlag(Flags.X2); }
            set { f = value ? f | Flags.X2 : f & ~Flags.X2; }
        }

        public int A
        {
            get { return a; }
            set { a = value & 0xFF; }
        }

        public int F
        {
            get
            {
                return (int)f;
            }
            set
            {
                f = (Flags)value;
            }
        }

        public int BC
        {
            get { return bc; }
            set { bc = value & 0xFFFF; }
        }

        public int DE
        {
            get { return de; }
            set { de = value & 0xFFFF; }
        }

        public int HL
        {
            get { return hl; }
            set { hl = value & 0xFFFF; }
        }

        public int B
        {
            get { return bc >> 8; }
            set { bc = bc & 0x00FF | ((value & 0xFF) << 8); }
        }

        public int C
        {
            get { return bc & 0xFF; }
            set { bc = bc & 0xFF00 | (value & 0xFF); }
        }

        public int D
        {
            get { return de >> 8; }
            set { de = de & 0x00FF | ((value & 0xFF) << 8); }
        }

        public int E
        {
            get { return de & 0xFF; }
            set { de = de & 0xFF00 | (value & 0xFF); }
        }

        public int H
        {
            get { return hl >> 8; }
            set { hl = hl & 0x00FF | ((value & 0xFF) << 8); }
        }

        public int L
        {
            get { return hl & 0xFF; }
            set { hl = hl & 0xFF00 | (value & 0xFF); }
        }

        public int AF
        {
            get { return a << 8 | (int)f; }
            set { a = (value >> 8) & 0xFF; f = (Flags)(value & 0x00FF); }
        }

        public int IX
        {
            get { return ix; }
            set { ix = value & 0xFFFF; }
        }

        public int IY
        {
            get { return iy; }
            set { iy = value & 0xFFFF; }
        }

        public int IXH
        {
            get { return ix >> 8; }
            set { ix = ix & 0x00FF | ((value & 0xFF) << 8); }
        }

        public int IXL
        {
            get { return ix & 0xFF; }
            set { ix = ix & 0xFF00 | (value & 0xFF); }
        }

        public int IYH
        {
            get { return iy >> 8; }
            set { iy = iy & 0x00FF | ((value & 0xFF) << 8); }
        }

        public int IYL
        {
            get { return iy & 0xFF; }
            set { iy = iy & 0xFF00 | (value & 0xFF); }
        }

        public int SP
        {
            get { return sp; }
            set { sp = value & 0xFFFF; }
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
        public int I
        {
            get { return i; }
            set { i = value; }
        }

        /// <summary>
        /// Memory Refresh Register
        /// </summary>
        public int R
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
                z80regs.f = f;
                z80regs.bc = bc;
                z80regs.de = de;
                z80regs.hl = hl;

                z80regs.af_ = af_;
                z80regs.bc_ = bc_;
                z80regs.de_ = de_;
                z80regs.hl_ = hl_;

            }
        }


        // Z80 Registers Old

        public byte _A
        {
            get { return (byte)a; }
            set { a = value; }
        }

        public short _BC
        {
            get { return (short)bc; }
            set { bc = value; }
        }

        public short _DE
        {
            get { return (short)de; }
            set { de = value; }
        }

        public short _HL
        {
            get { return (short)hl; }
            set { hl = value; }
        }

        public byte _B
        {
            get { return (byte)(bc >> 8); }
            set { bc = bc & 0x00FF | (value << 8); }
        }

        public byte _C
        {
            get { return (byte)(bc & 0xFF); }
            set { bc = bc & 0xFF00 | (value & 0xFF); }
        }

        public byte _D
        {
            get { return (byte)(de >> 8); }
            set { de = de & 0x00FF | ((value & 0xFF) << 8); }
        }

        public byte _E
        {
            get { return (byte)(de & 0xFF); }
            set { de = de & 0xFF00 | (value & 0xFF); }
        }

        public byte _H
        {
            get { return (byte)(hl >> 8); }
            set { hl = hl & 0x00FF | ((value & 0xFF) << 8); }
        }

        public byte _L
        {
            get { return (byte)(hl & 0xFF); }
            set { hl = hl & 0xFF00 | (value & 0xFF); }
        }

        public short _AF
        {
            get { return (short)(a << 8 | (int)f); }
            set { a = (value >> 8) & 0xFF; f = (Flags)(value & 0x00FF); }
        }

        public short _IX
        {
            get { return (short)ix; }
            set { ix = value; }
        }

        public short _IY
        {
            get { return (short)iy; }
            set { iy = value; }
        }

        public byte _IXH
        {
            get { return (byte)(ix >> 8); }
            set { ix = ix & 0x00FF | ((value & 0xFF) << 8); }
        }

        public byte _IXL
        {
            get { return (byte)(ix & 0xFF); }
            set { ix = ix & 0xFF00 | (value & 0xFF); }
        }

        public byte _IYH
        {
            get { return (byte)(iy >> 8); }
            set { iy = iy & 0x00FF | ((value & 0xFF) << 8); }
        }

        public byte _IYL
        {
            get { return (byte)(iy & 0xFF); }
            set { iy = iy & 0xFF00 | (value & 0xFF); }
        }

        public short _SP
        {
            get { return (short)sp; }
            set { sp = value; }
        }

        public short _PC
        {
            get { return (short)pc; }
            set { pc = value; }
        }

    }
}
