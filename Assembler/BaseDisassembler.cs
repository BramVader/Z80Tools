using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    public abstract class BaseDisassembler
    {
        public abstract DisassemblyResult Disassemble(Func<int, byte> memory, int address);
    }
}
