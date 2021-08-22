using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public class MemorySegment
    {
        public int Address { get; set; }

        public List<byte> Memory { get; set; }
    }
}
