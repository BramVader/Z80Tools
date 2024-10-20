﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                using var rd = new BinaryReader(resourceStream);
                return rd.ReadBytes(0x4000);
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
            using var sw = new StreamWriter("LOWER.LST");
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

        public static void CreateMatrix()
        {
            var memory = new byte[0x10000];
            var decompiler = new Z80Disassembler();

            using var writer = new StreamWriter("output.csv");
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

        public static void CheckEmulator()
        {

        }

        public static void TestEmulator()
        {
            var memory = new byte[0x10000];
            var decompiler = new Z80Disassembler(true);
            var emulator = new Z80Emulator();
            bool[] f = new bool[0x10];
            using var writer = new StreamWriter("output.html");
            writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\"><head>");
            writer.WriteLine("<title>All Z80 instructions</title><style>");
            writer.WriteLine("body { background-color: #fff; font-family: Verdana; font-size: 70%;}");
            writer.WriteLine("table { border-collapse: collapse; width: 100%;}");
            writer.WriteLine("th { background-color: #0a6cce; border: 1px solid silver; color: White; padding: 0.4em; text-align: left; vertical-align: top;}");
            writer.WriteLine("td { border: 1px solid silver; padding: 0.4em; vertical-align: top;}");
            writer.WriteLine("td:first-of-type { width:15em; }");
            writer.WriteLine("form div { background-color: #e0e0e0; border-radius: 4px; margin: 2px 2px 20px 2px; padding: 4px; }");
            writer.WriteLine(".Code { font-family: Consolas, \"courier new\"; color: #000000; }");  // Default code style
            writer.WriteLine(".Type { color: #2B91AF; }");
            writer.WriteLine(".String { color: #A31515; }");
            writer.WriteLine(".Number { color: #000000; }");
            writer.WriteLine(".Keyword  { color: #0000FF; }");
            writer.WriteLine(".Comment  { color: #008000; }");
            writer.WriteLine(".Identifier  { color: #000000; }");
            writer.WriteLine(".Quote  { background-color: #F0F0F0; display: inline; border-radius:3px; }");
            writer.WriteLine("</style></head>");
            writer.WriteLine("<body><table>");
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
                        var sb = new StringBuilder("<tr><td>");
                        for (int k = 0; k < line.Opcodes.Length; k++)
                        {
                            var bt = line.Opcodes[k];
                            if (f[k])
                                sb.Append("xx");
                            else
                                sb.Append(bt.ToString("X2"));
                        }
                        sb.Append("</td><td>");
                        sb.Append(line.Mnemonic);
                        if (!String.IsNullOrEmpty(line.Operands))
                        {
                            sb.Append(' ');
                            sb.Append(line.Operands);
                        }
                        sb.Append("</td><td>");
                        sb.Append(expr.Evaluate(null, true));
                        sb.Append("</td></tr>");
                        writer.WriteLine(sb.ToString());
                    }
                }
            }
            writer.WriteLine("</table></body>");
        }

        private static async Task TestInstruction()
        {
            var ass = @"
LD HL, 1234
LD BC, 5678
ADD HL,BC
";
            var asm = new Z80Assembler();
            var coll = new Assembler.OutputCollector(Console.Out);
            await asm.Assemble(coll, new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(ass))));
            var emu = new Z80Emulator();

            var memory = coll.Segments.First().Memory;
            var registers = emu.GetRegisters<Z80Registers>();

            emu.ReadMemory = adr => memory[adr];

            for (int n = 0; n < 65535; n += 319)
                for (int m = 0; m < 65535; m += 415)
                {
                    // Overwrite LD HL operand
                    memory[1] = (byte)(n & 255);
                    memory[1 + 1] = (byte)(n >> 8);
                    // Overwrite LD BC operand
                    memory[1 + 3] = (byte)(m & 255);
                    memory[1 + 4] = (byte)(m >> 8);
                    // Execute 3 instructions
                    emu.Emulate();
                    emu.Emulate();
                    emu.Emulate();
                    // Check the result
                    if (registers.HL != ((n + m) & 65535))
                    {
                    }
                    Console.WriteLine($"{n:X4} {m:X4} {registers.HL:X4} {registers.F}");
                    emu.Reset();
                }
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            //CreateMatrix();
            //TestEmulator();
            await TestInstruction();
            //TestRom();
        }
    }
}
