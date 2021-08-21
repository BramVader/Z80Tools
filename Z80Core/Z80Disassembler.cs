using Assembler;
using Disassembler;
using System;
using System.Linq;
using Symbol = Disassembler.Symbol;

namespace Z80Core
{
    public class Z80Disassembler: BaseDisassembler
    {

        public static string FormatByte(byte b)
        {
            return "0x" + b.ToString("X2");
        }

        public static string FormatDisp(byte b)
        {
            int value = (sbyte)b;
            return value < 0 ?
                "- " + (-value).ToString() :
                "+ " + value.ToString();
        }

        public string FormatWord(int w)
        {
            if (symbols != null)
            {
                var symbol = symbols.FindSymbols(w).FirstOrDefault();
                if (symbol != null)
                    return symbol.Name;
            }
            return "0x" + w.ToString("X4");
        }

        private readonly string[,] reg = new string[,]
        {
            { "B", "C", "D", "E", "H", "L", "(HL)", "A" },
            { "B", "C", "D", "E", "IXH", "IXL", "(IX d)", "A" },
            { "B", "C", "D", "E", "IYH", "IYL", "(IY d)", "A" }
        };
        private readonly  string[,] rp = new string[,]
        {
            {"BC", "DE", "HL", "SP" },
            {"BC", "DE", "IX", "SP" },
            {"BC", "DE", "IY", "SP" }
        };
        private readonly string[] cc = new string[] { "NZ", "Z", "NC", "C", "PO", "PE", "P", "M" };
        private readonly string[] alu = new string[] { "ADD A,", "ADC A,", "SUB", "SBC A,", "AND", "XOR", "OR", "CP" };
        private readonly string[] rot = new string[] { "RLC", "RRC", "RL", "RR", "SLA", "SRA", "SLL", "SRL" };
        private readonly string[] im = new string[] { "0", "0", "1", "2", "0", "0", "1", "2" };

        // b = 0...3
        private readonly string[,] bli = new string[,] {
            /* a=4 */    { "LDI", "CPI", "INI", "OUTI" },
            /* a=5 */    { "LDD", "CPD", "IND", "OUTD" },
            /* a=6 */    { "LDIR", "CPIR", "INIR", "OTIR" },
            /* a=7 */    { "LDDR", "CPDR", "INDR", "OTDR" }
            };
        private readonly bool document;

        private readonly Symbols symbols;

        public Z80Disassembler(bool document = false, Symbols symbols = null)
        {
            this.document = document;
            this.symbols = symbols;
        }

        public override DisassemblyResult Disassemble(Func<int, byte> memory, int address)
        {
            int startAddr = address;

            int prefix = 0;
            byte opcode = memory(address++);
            if (opcode == 0xDD || opcode == 0xFD)
            {
                prefix = opcode == 0xDD ? 1 : 2;
                opcode = memory(address++);
            }

            string name = "";
            string comment = "";
            if (symbols != null)
            {
                Symbol symbol = symbols.FindSymbols(startAddr).FirstOrDefault();
                if (symbol != null)
                {
                    name = symbol.Name;
                    comment = symbol.Comment;
                }
            }

            int x = opcode >> 6;
            int y = (opcode & 0x38) >> 3;
            int z = opcode & 0x07;
            int p = (opcode & 0x30) >> 4;
            int q = (opcode & 0x08) >> 3;

            string displacement()
            {
                sbyte disp = (sbyte)memory(address++);
                if (document)
                    return "e";
                else
                    return FormatWord(unchecked(address + (int)disp));
            }
            string n()
            {
                byte low = memory(address++);
                if (document)
                    return "n";
                else
                    return FormatByte(low);
            }
            string nn()
            {
                byte low = memory(address++);
                byte high = memory(address++);
                if (document)
                    return "nn";
                else
                    return FormatWord((ushort)((ushort)(high << 8) | (ushort)low));
            }
            string indirect()
            {
                if (document)
                {
                    nn();
                    return "(nn)";
                }
                else
                {
                    return "(" + nn() + ")";
                }
            }
            string port()
            {
                if (document)
                {
                    n();
                    return "(n)";
                }
                else
                {
                    return "(" + n() + ")";
                }
            }
            string r(int index)
            {
                string value = reg[prefix, index];
                if (index == 6 && prefix != 0)
                {
                    byte disp = memory(address++);
                    return document ? value.Replace("d", "+ d") : value.Replace("d", FormatDisp(disp));
                }
                return value;
            }

            switch (x)
            {
                case 0:
                    switch (z)
                    {
                        case 0:
                            switch (y)
                            {
                                case 0:
                                    return new DisassemblyResult("NOP", memory, startAddr, address, name, comment);
                                case 1:
                                    return new DisassemblyResult("EX", "AF, AF'", memory, startAddr, address, name, comment);
                                case 2:
                                    return new DisassemblyResult("DJNZ", displacement(), memory, startAddr, address, name, comment);
                                case 3:
                                    return new DisassemblyResult("JR", displacement(), memory, startAddr, address, name, comment);
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                    return new DisassemblyResult("JR", cc[y - 4] + ", " + displacement(), memory, startAddr, address, name, comment);
                            }
                            break;
                        case 1:
                            switch (q)
                            {
                                case 0:
                                    return new DisassemblyResult("LD", rp[prefix, p] + ", " + nn(), memory, startAddr, address, name, comment);
                                case 1:
                                    return new DisassemblyResult("ADD", rp[prefix, 2] + ", " + rp[prefix, p], memory, startAddr, address, name, comment);
                            }
                            break;
                        case 2:
                            switch (y)
                            {
                                case 0:
                                    return new DisassemblyResult("LD", "(BC), A", memory, startAddr, address, name, comment);
                                case 1:
                                    return new DisassemblyResult("LD", "A, (BC)", memory, startAddr, address, name, comment);
                                case 2:
                                    return new DisassemblyResult("LD", "(DE), A", memory, startAddr, address, name, comment);
                                case 3:
                                    return new DisassemblyResult("LD", "A, (DE)", memory, startAddr, address, name, comment);
                                case 4:
                                    return new DisassemblyResult("LD", indirect() + ", " + rp[prefix, 2], memory, startAddr, address, name, comment);
                                case 5:
                                    return new DisassemblyResult("LD", rp[prefix, 2] + ", " + indirect(), memory, startAddr, address, name, comment);
                                case 6:
                                    return new DisassemblyResult("LD", indirect() + ", A", memory, startAddr, address, name, comment);
                                case 7:
                                    return new DisassemblyResult("LD", "A, " + indirect(), memory, startAddr, address, name, comment);
                            }
                            break;
                        case 3:
                            switch (q)
                            {
                                case 0:
                                    return new DisassemblyResult("INC", rp[prefix, p], memory, startAddr, address, name, comment);
                                case 1:
                                    return new DisassemblyResult("DEC", rp[prefix, p], memory, startAddr, address, name, comment);
                            }
                            break;
                        case 4:
                            return new DisassemblyResult("INC", r(y), memory, startAddr, address, name, comment);
                        case 5:
                            return new DisassemblyResult("DEC", r(y), memory, startAddr, address, name, comment);
                        case 6:
                            return new DisassemblyResult("LD", r(y) + ", " + n(), memory, startAddr, address, name, comment);
                        case 7:
                            switch (y)
                            {
                                case 0:
                                    return new DisassemblyResult("RLCA", memory, startAddr, address, name, comment);
                                case 1:
                                    return new DisassemblyResult("RRCA", memory, startAddr, address, name, comment);
                                case 2:
                                    return new DisassemblyResult("RLA", memory, startAddr, address, name, comment);
                                case 3:
                                    return new DisassemblyResult("RRA", memory, startAddr, address, name, comment);
                                case 4:
                                    return new DisassemblyResult("DAA", memory, startAddr, address, name, comment);
                                case 5:
                                    return new DisassemblyResult("CPL", memory, startAddr, address, name, comment);
                                case 6:
                                    return new DisassemblyResult("SCF", memory, startAddr, address, name, comment);
                                case 7:
                                    return new DisassemblyResult("CCF", memory, startAddr, address, name, comment);
                            }
                            break;
                    }
                    break;
                case 1:
                    if (opcode == 0x76)
                        return new DisassemblyResult("HALT", memory, startAddr, address, name, comment);
                    return new DisassemblyResult("LD",
                        (prefix > 0 && z == 6 ? reg[0, y] : r(y)) + ", " +
                        (prefix > 0 && y == 6 ? reg[0, z] : r(z)), memory, startAddr, address, name, comment);
                case 2:
                    var aluv1 = alu[y].Split(' ');
                    return new DisassemblyResult(aluv1[0], (aluv1.Length == 2 ? "A, " : "") + r(z), memory, startAddr, address, name, comment);
                case 3:
                    switch (z)
                    {
                        case 0:
                            return new DisassemblyResult("RET", cc[y], memory, startAddr, address, name, comment);
                        case 1:
                            if (q == 0)
                                return new DisassemblyResult("POP", p == 3 ? "AF" : rp[prefix, p], memory, startAddr, address, name, comment);
                            else
                                switch (p)
                                {
                                    case 0:
                                        return new DisassemblyResult("RET", memory, startAddr, address, name, comment);
                                    case 1:
                                        return new DisassemblyResult("EXX", memory, startAddr, address, name, comment);
                                    case 2:
                                        return new DisassemblyResult("JP", "(" + rp[prefix, 2] + ")", memory, startAddr, address, name, comment);
                                    case 3:
                                        return new DisassemblyResult("LD", "SP, " + rp[prefix, 2], memory, startAddr, address, name, comment);
                                }
                            break;
                        case 2:
                            return new DisassemblyResult("JP", cc[y] + ", " + nn(), memory, startAddr, address, name, comment);
                        case 3:
                            switch (y)
                            {
                                case 0:
                                    return new DisassemblyResult("JP", nn(), memory, startAddr, address, name, comment);
                                case 1:     // CB Prefix
                                    string dsp = prefix > 0 ? r(6) : "";
                                    opcode = memory(address++);

                                    x = opcode >> 6;
                                    y = (opcode & 0x38) >> 3;
                                    z = opcode & 0x07;
                                    switch (x)
                                    {
                                        case 0:
                                            if (prefix > 0)
                                                if (z == 6)
                                                    return new DisassemblyResult(rot[y], dsp, memory, startAddr, address, name, comment);
                                                else
                                                    return new DisassemblyResult(rot[y], dsp + ", " + reg[0, z], memory, startAddr, address, name, comment);
                                            return new DisassemblyResult(rot[y], reg[0, z], memory, startAddr, address, name, comment);
                                        case 1:
                                            if (prefix > 0)
                                                return new DisassemblyResult("BIT", y + ", " + dsp, memory, startAddr, address, name, comment);
                                            return new DisassemblyResult("BIT", y + ", " + reg[0, z], memory, startAddr, address, name, comment);
                                        case 2:
                                            if (prefix > 0)
                                                if (z == 6)
                                                    return new DisassemblyResult("RES", y + ", " + dsp, memory, startAddr, address, name, comment);
                                                else
                                                    return new DisassemblyResult("RES", y + ", " + dsp + ", " + reg[0, z], memory, startAddr, address, name, comment);
                                            return new DisassemblyResult("RES", y + ", " + reg[0, z], memory, startAddr, address, name, comment);
                                        case 3:
                                            if (prefix > 0)
                                                if (z == 6)
                                                    return new DisassemblyResult("SET", y + ", " + dsp, memory, startAddr, address, name, comment);
                                                else
                                                    return new DisassemblyResult("SET", y + ", " + dsp + ", " + reg[0, z], memory, startAddr, address, name, comment);
                                            return new DisassemblyResult("SET", y + ", " + reg[0, z], memory, startAddr, address, name, comment);
                                    }
                                    break;
                                case 2:
                                    return new DisassemblyResult("OUT", port() + ", A", memory, startAddr, address, name, comment);
                                case 3:
                                    return new DisassemblyResult("IN", "A, " + port(), memory, startAddr, address, name, comment);
                                case 4:
                                    return new DisassemblyResult("EX", "(SP), " + rp[prefix, 2], memory, startAddr, address, name, comment);
                                case 5:
                                    return new DisassemblyResult("EX", "DE, HL", memory, startAddr, address, name, comment);
                                case 6:
                                    return new DisassemblyResult("DI", memory, startAddr, address, name, comment);
                                case 7:
                                    return new DisassemblyResult("EI", memory, startAddr, address, name, comment);
                            }
                            break;
                        case 4:
                            return new DisassemblyResult("CALL", cc[y] + ", " + nn(), memory, startAddr, address, name, comment);
                        case 5:
                            if (q == 0)
                                return new DisassemblyResult("PUSH", p == 3 ? "AF" : rp[prefix, p], memory, startAddr, address, name, comment);
                            else
                                switch (p)
                                {
                                    case 0:
                                        return new DisassemblyResult("CALL", nn(), memory, startAddr, address, name, comment);
                                    case 1: // DD prefix
                                        break;
                                    case 2: // ED prefix
                                        opcode = memory(address++);
                                        x = opcode >> 6;
                                        y = (opcode & 0x38) >> 3;
                                        z = opcode & 0x07;
                                        p = (opcode & 0x30) >> 4;
                                        q = (opcode & 0x08) >> 3;
                                        switch (x)
                                        {
                                            case 0:
                                                return new DisassemblyResult("NONI", memory, startAddr, address, name, comment);
                                            case 1:
                                                switch (z)
                                                {
                                                    case 0:
                                                        if (y == 6)
                                                            return new DisassemblyResult("IN", "(C)", memory, startAddr, address, name, comment);
                                                        return new DisassemblyResult("IN", r(y) + ", (C)", memory, startAddr, address, name, comment);
                                                    case 1:
                                                        if (y == 6)
                                                            return new DisassemblyResult("OUT", "(C), 0", memory, startAddr, address, name, comment);
                                                        return new DisassemblyResult("OUT", "(C), " + r(y), memory, startAddr, address, name, comment);
                                                    case 2:
                                                        return new DisassemblyResult(q == 0 ? "SBC" : "ADC",  rp[prefix, 2] + ", " + rp[prefix, p], memory, startAddr, address, name, comment);
                                                    case 3:
                                                        return new DisassemblyResult("LD", q == 0 ? indirect() + ", " + rp[prefix, p] : rp[prefix, p] + ", " + indirect(), memory, startAddr, address, name, comment);
                                                    case 4:
                                                        return new DisassemblyResult("NEG", memory, startAddr, address, name, comment);
                                                    case 5:
                                                        return new DisassemblyResult(y == 1 ? "RETI" : "RETN", memory, startAddr, address, name, comment);
                                                    case 6:
                                                        return new DisassemblyResult("IM", im[y], memory, startAddr, address, name, comment);
                                                    case 7:
                                                        switch (y)
                                                        {
                                                            case 0:
                                                                return new DisassemblyResult("LD", "I, A", memory, startAddr, address, name, comment);
                                                            case 1:
                                                                return new DisassemblyResult("LD", "R, A", memory, startAddr, address, name, comment);
                                                            case 2:
                                                                return new DisassemblyResult("LD", "A, I", memory, startAddr, address, name, comment);
                                                            case 3:
                                                                return new DisassemblyResult("LD", "A, R", memory, startAddr, address, name, comment);
                                                            case 4:
                                                                return new DisassemblyResult("RRD", memory, startAddr, address, name, comment);
                                                            case 5:
                                                                return new DisassemblyResult("RLD", memory, startAddr, address, name, comment);
                                                            case 6:
                                                            case 7:
                                                                return new DisassemblyResult("NOP", memory, startAddr, address, name, comment);
                                                        }
                                                        break;
                                                }
                                                break;
                                            case 2:
                                                if (z < 4 && y >= 4)
                                                    return new DisassemblyResult(bli[y - 4, z], memory, startAddr, address, name, comment);
                                                break;
                                            case 3:
                                                return new DisassemblyResult("NONI", memory, startAddr, address, name, comment);
                                        }
                                        break;
                                    case 3: // FD prefix

                                        break;
                                }
                            break;
                        case 6:
                            var aluv2 = alu[y].Split(' ');
                            return new DisassemblyResult(aluv2[0], (aluv2.Length == 2 ? "A, " : "") + n(), memory, startAddr, address, name, comment);
                        case 7:
                            return new DisassemblyResult("RST", FormatWord((ushort)(y * 8)), memory, startAddr, address, name, comment);
                    }
                    break;
            }

            return new DisassemblyResult("NOP", memory, startAddr, address, name, comment);
        }
    }
}
