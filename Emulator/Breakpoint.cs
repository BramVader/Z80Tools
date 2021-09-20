using System;
using System.Collections.Generic;

namespace Emulator
{
    public class Breakpoint
    {
        public class Comparer : IComparer<Breakpoint>
        {
            public int Compare(Breakpoint x, Breakpoint y)
            {
                return x.Address.CompareTo(y.Address);
            }
        }

        public int Address { get; set; }
        public Func<bool> Condition { get; set; }

        // You can override this function to handle the breakpoint
        // Return true if it should pause the CPU
        public virtual bool Handle(BaseEmulator emulator)
        {
            return true;
        }
    }
}
