using System.Diagnostics;

namespace Emulator
{
    public abstract class BaseRegisters
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected int pc;

        public int PC
        {
            get { return pc; }
            set { pc = value & 0xFFFF; }
        }
        
        public abstract void CloneTo(object regs);

        public BaseRegisters()
        {
            pc = 0;
        }
    }
}
