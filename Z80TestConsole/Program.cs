using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Z80Core;

namespace Z80TestConsole
{
    static class Program
    {
        public static byte[] LoadRom(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string fullName = assembly.GetManifestResourceNames().Where(nm => nm.Contains(name)).FirstOrDefault();
            if (!String.IsNullOrEmpty(fullName))
            {
                var resourceStream = assembly.GetManifestResourceStream(fullName);
                using (var rd = new BinaryReader(resourceStream))
                {
                    return rd.ReadBytes(0x4000);
                }
            }
            return null;
        }

        public static void TestSet()
        {
            var memory = new byte[0x10000];
            var decompiler = new Z80Disassembler();
            var lineCounter = 0;

            memory[0] = 0xFD;
            memory[1] = 0x10;
            memory[2] = 0x11;
            memory[3] = 0x22;
            memory[4] = 0x33;
            memory[5] = 0x44;

            for (int opc = 0; opc < 256; opc++)
            {
                memory[1] = (byte)opc;

                int rangeMin1 = 0x22;
                int rangeMax1 = 0x22;
                if (opc == 0xCB || opc == 0xED)
                {
                    rangeMin1 = 0x00;
                    rangeMax1 = 0xFF;
                }

                for (int opc2 = rangeMin1; opc2 <= rangeMax1; opc2++)
                {
                    memory[3] = (byte)opc2;

                    var line = decompiler.Disassemble(a => memory[a], 0);
                    if (line != null)
                    {
                        Console.Write(0.ToString("X4"));
                        Console.Write(" ");
                        foreach (byte opcode in line.Opcodes)
                        {
                            Console.Write(opcode.ToString("X2"));
                        }
                        Console.CursorLeft = 20;
                        Console.Write(line.Mnemonic);
                        Console.CursorLeft = 30;
                        Console.WriteLine(line.Operands);

                        lineCounter = (lineCounter + 1) % 30;
                        if (lineCounter == 0)
                            Console.ReadLine();
                    }
                }
            }
        }

        public static void TestRom()
        {
            var memory = LoadRom("LOWER");
            Decompile(memory, 0, 0x4000);
        }

        public static void Decompile(byte[] memory, ushort address, int length)
        {
            var decompiler = new Z80Disassembler();
            ushort adr1 = address;
            using (var sw = new StreamWriter("LOWER.LST"))
            {
                while (adr1 < address + length)
                {
                    var line = decompiler.Disassemble(a => memory[a], adr1);
                    if (line != null)
                    {
                        sw.Write(adr1.ToString("X4"));
                        sw.Write("\t");
                        foreach (byte opcode in line.Opcodes)
                        {
                            sw.Write(opcode.ToString("X2"));
                        }
                        for (int n = 0; n < Math.Max(1, 4 - line.Opcodes.Length); n++)
                            sw.Write("\t");
                        sw.Write(line.Mnemonic);
                        if (line.Mnemonic.Length < 3)
                            sw.Write("\t");
                        sw.Write("\t");
                        sw.WriteLine(line.Operands);
                        adr1 += (ushort)line.Opcodes.Length;
                    }
                    else
                        adr1++;
                }
            }
        }

        public static void CreateMatrix()
        {
            var memory = new byte[0x10000];
            var decompiler = new Z80Disassembler();

            using (var writer = new StreamWriter("output.csv"))
            {
                memory[0] = 0xED;
                memory[1] = 0xCB;
                memory[2] = 0x11;
                memory[3] = 0x22;
                memory[4] = 0x33;
                memory[5] = 0x44;

                for (int opc = 0; opc < 256; opc++)
                {
                    memory[1] = (byte)opc;
                    var line = decompiler.Disassemble(a => memory[a], 0);
                    if (line != null)
                    {
                        writer.Write("{0} {1};", line.Mnemonic, line.Operands);
                    }
                    else
                        writer.Write(";");
                    if ((opc & 15) == 15)
                        writer.WriteLine();
                }
            }
        }

        public static void CheckEmulator()
        {

        }

        public static void TestEmulator()
        {
            var memory = new byte[0x10000];
            var decompiler = new Z80Disassembler(true);
            var emulator = new Z80Emulator();
            bool[] f = new bool[0x10];
            using (var writer = new StreamWriter("output.csv"))
            {
                for (int m = 0; m < 7; m++)
                {
                    f[0] = true;
                    f[1] = true;
                    f[2] = true;
                    f[3] = true;
                    switch (m)
                    {
                        case 0:
                            f[0] = false;
                            break;
                        case 5:
                        case 6:
                            f[0] = false;
                            f[1] = false;
                            f[3] = false;
                            break;
                        default:
                            f[0] = false;
                            f[1] = false;
                            break;
                    }
                    for (int n = 0; n < 256; n++)
                    {
                        switch (m)
                        {
                            case 0:
                                if (n == 0xCB) continue;
                                memory[0] = (byte)n;
                                break;
                            case 1:
                                memory[0] = 0xCB;
                                memory[1] = (byte)n;
                                break;
                            case 2:
                                memory[0] = 0xED;
                                memory[1] = (byte)n;
                                break;
                            case 3:
                                if (n == 0xCB) continue;
                                memory[0] = 0xDD;
                                memory[1] = (byte)n;
                                break;
                            case 4:
                                if (n == 0xCB) continue;
                                memory[0] = 0xFD;
                                memory[1] = (byte)n;
                                break;
                            case 5:
                                memory[0] = 0xDD;
                                memory[1] = 0xCB;
                                memory[3] = (byte)n;
                                break;
                            case 6:
                                memory[0] = 0xFD;
                                memory[1] = 0xCB;
                                memory[3] = (byte)n;
                                break;
                        }
                        var line = decompiler.Disassemble(adr => memory[adr], 0);
                        if (m == 3 || m == 4)
                        {
                            var line2 = decompiler.Disassemble(adr => memory[adr], 1);
                            if (line.Operands == line2.Operands)
                                continue;
                        }
                        var expr = emulator.GetExpression(memory);
                        if (expr != null && line != null && !String.IsNullOrEmpty(line.Mnemonic))
                        {
                            var sb = new StringBuilder();
                            for (int k = 0; k < line.Opcodes.Length; k++)
                            {
                                var bt = line.Opcodes[k];
                                if (f[k])
                                    sb.Append("xx");
                                else
                                    sb.Append(bt.ToString("X2"));
                            }
                            sb.Append("\t");
                            sb.Append(line.Mnemonic);
                            if (!String.IsNullOrEmpty(line.Operands))
                            {
                                sb.Append(" ");
                                sb.Append(line.Operands);
                            }
                            sb.Append("\t");
                            // TODO: Find evaluator code somewhere -- sb.Append(expr.Evaluate(true, "\t\t"));
                            writer.WriteLine(sb.ToString());
                        }
                    }
                }
            }

        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //CreateMatrix();
            //TestEmulator();
            TestRom();
        }
    }
}
