using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulator
{
    public abstract class BaseRegisters
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ushort pc;

        public ushort PC
        {
            get { return pc; }
            set { pc = value; }
        }
        
        public abstract void CloneTo(object regs);

        public BaseRegisters()
        {
            pc = 0;
        }
    }
}
