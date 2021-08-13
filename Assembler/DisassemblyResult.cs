using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    public class DisassemblyResult : BaseResult
    {
        public byte[] Opcodes { get; set; }
        public string Mnemonic { get; set; }
        public string Operands { get; set; }
        public int States { get; set; }
        public int Cycles { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }

        public DisassemblyResult(string mnemomic, Func<int, byte> memory, int startAddress, int currentAddress, string name, string comment) :
            this(mnemomic, null, memory, startAddress, currentAddress, name, comment)
        {
        }

        public DisassemblyResult(string mnemomic, string operands, Func<int, byte> memory, int startAddress, int currentAddress, string name, string comment)
        {
            Address = startAddress;
            Mnemonic = mnemomic;
            Operands = operands;
            Opcodes = new byte[(ushort)(currentAddress - startAddress)];
            for (var adr = startAddress; adr < currentAddress; adr++)
                Opcodes[adr - startAddress] = memory(adr);
            Name = name;
            Comment = comment;
        }
    }

}
