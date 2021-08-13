using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
