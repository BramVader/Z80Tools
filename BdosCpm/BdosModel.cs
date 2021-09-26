using Emulator;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Z80Core;

namespace BdosCpm
{
    public class BdosModel : HardwareModel
    {
        private class BdosBreakpoint : Breakpoint
        {
            private readonly MemoryModel memoryModel;
            private readonly Console console;

            public BdosBreakpoint(MemoryModel memoryModel, Console console)
            {
                this.memoryModel = memoryModel;
                this.console = console;
            }

            public override bool Handle(BaseEmulator emulator)
            {
                var reg = emulator.GetRegisters<Z80Registers>();
                if (reg != null)
                {
                    switch (reg.C)
                    {
                        case 1:     // C_READ
                            // Returns A=L=character.
                            // Wait for a character from the keyboard; then echo it to the screen and return it.
                            break;
                        case 2:     // C_WRITE
                            // E=ASCII character.
                            // Send the character in E to the screen. Tabs are expanded to spaces.
                            // Output can be paused with ^S and restarted with ^Q (or any key under
                            // versions prior to CP/M 3). While the output is paused, the program can
                            // be terminated with ^C.
                            console.Write(new String((char)reg.E, 1));
                            break;
                        case 9:     // C_WRITESTR
                            // DE=address of string.
                            // Display a string of ASCII characters, terminated with the $ character.
                            // Thus the string may not contain $ characters - so, for example, the VT52
                            // cursor positioning command ESC Y y+32 x+32 will not be able to use row 4.
                            var sb = new StringBuilder();
                            int addr = reg.DE;
                            int laddr = addr;
                            do
                            {
                                var chr = (char)memoryModel.Read(addr, true);
                                if (chr == '$') break;
                                sb.Append(chr);
                                addr = (addr + 1) & 0xFFFF;
                            }
                            while (addr != laddr);
                            if (sb.Length > 0)
                                console.Write(sb.ToString().Replace("\r", "").Replace("\n", "\r\n"));
                            break;
                        default:
                            break;
                    }
                }
                return false;   // Do not pause
            }
        }

        private readonly Console console;

        public Disassembler.Symbols Symbols { get; init; }

        public BdosModel()
        {
            // Create memory model
            this.memoryModel = new MemoryModel(
                new MemoryDescriptor("RAM", MemoryType.Ram, 0x0000, 0x10000))
            {
                AddressSpace = 0x10000 // 64KiB
            };

            memorySwitch = new bool[] { true };
            memoryModel.SwitchMemory(memorySwitch);

            // Create Z80 emulator
            this.emulator = new Z80Emulator
            {
                ClockFrequency = 100.0, // MHz
                ReadMemory = memoryModel.ReadMemory,
                WriteMemory = memoryModel.WriteMemory,
                ReadInput = ReadInput,
                WriteOutput = WriteOutput
            };
            //emulator.OnCpuStep += OnCpuStep;
            //emulator.OnInterruptAcknowledged += OnInterruptAcknowledged;

            var listFileReader = new ListFileReader();
            using var sr = new StreamReader(new FileStream(@"C:\Development\Private\Z80Tools\Z80Validator\bin\Debug\net5.0\output.lst", FileMode.Open, FileAccess.Read, FileShare.Read));
            this.Symbols = listFileReader.Read(sr);

            this.console = new Console();

            Reset();

            console.Show();
        }

        public override void Reset()
        {
            using var br = new BinaryReader(new FileStream(@"C:\Development\Private\Z80Tools\Z80Validator\bin\Debug\net5.0\output.bin", FileMode.Open, FileAccess.Read, FileShare.Read));
            var bytes = new byte[0x10000];
            int numRead = br.Read(bytes, 0, bytes.Length);
            Array.Resize(ref bytes, numRead);
            MemoryModel.Write(bytes, 0x0100, true);

            // Put a special breakpoint (one that doesn't pause) on address 5
            emulator.AddBreakpoint(new BdosBreakpoint(memoryModel, console) { Address = 0x0005 });

            // Return immediately from the BDOS call
            MemoryModel.Write(new byte[] { 0xC9 }, 0x0005, true);

            emulator.GetRegisters<Z80Registers>().PC = 0x100;
            console.Reset();
        }

        protected override byte ReadInput(int address)
        {
            return 0;
        }

        protected override void WriteOutput(int address, byte value)
        {
        }
    }
}
