using Disassembler;
using Emulator;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Z80Core;

namespace CPCAmstrad
{
    public class CPC464Model : HardwareModel
    {
        protected int upperRomBankNumber;
        protected long hsyncCounter;

        // Hardware devices
        protected readonly PIO8255 pio8255;
        protected readonly CRTC6845 crtc6845;
        protected readonly CPCScreen screen;
        protected readonly CPCKeyboard keyboard;
        protected readonly PrinterPort printerPort;
        protected readonly GateArray gateArray;
        protected readonly AY3_8912 ay3_8912;
        protected readonly Scope scope;

        // Hardware Settings
        protected readonly VideoSystem videoSystem;
        protected readonly CompanyName companyname;

        public CPC464Model()
        {
            // Create memory model
            memoryModel = new MemoryModel(
                new MemoryDescriptor("LROM", MemoryType.Rom, 0x0000, 0x4000),
                new MemoryDescriptor("UROM", MemoryType.Rom, 0xC000, 0x4000),
                new MemoryDescriptor("RAM", MemoryType.Ram, 0x0000, 0x10000))
            {
                AddressSpace = 0x10000 // 64KiB
            };

            memorySwitch = new bool[] { true, true, true };     // All enabled
            memoryModel.SwitchMemory(memorySwitch);

            memoryModel.Write(LoadRom("LOWER.ROM"), 0x0000, true, false, false);
            memoryModel.Write(LoadRom("UPPER.ROM"), 0xC000, false, true, false);

            // Create Z80 emulator
            var emulator = new Z80Emulator(this)
            {
                ClockFrequency = 4.0, // MHz
                ReadMemory = memoryModel.ReadMemory,
                WriteMemory = memoryModel.WriteMemory,
                ReadInput = ReadInput,
                WriteOutput = WriteOutput
            };
            this.emulator = emulator;

            // Set hardware settings
            videoSystem = VideoSystem.PAL;
            companyname = CompanyName.Schneider;

            // Create hardware devices
            pio8255 = new PIO8255();
            crtc6845 = new CRTC6845(this);
            ay3_8912 = new AY3_8912();

            screen = new CPCScreen(this);
            keyboard = new CPCKeyboard();
            printerPort = new PrinterPort();
            gateArray = new GateArray(memoryModel, memorySwitch);
            scope = new Scope();
            scope.AddBitChannel("HSync");
            scope.AddBitChannel("VSync");
            scope.AddIntChannel("RamAddr", -1, 64 * 312);

            screen.Show();
            keyboard.Show();
            //scope.Show();
        }

        public override void Reset()
        {
            memorySwitch = new bool[] { true, true, true };     // All enabled
            memoryModel.SwitchMemory(memorySwitch);
            emulator.Reset();
        }

        private bool lastHSync, lastVSync;

        public override void AfterInstruction(long stateCounter)
        {
            crtc6845.Simulate(stateCounter);
            bool hSync = crtc6845.HSync;
            bool vSync = crtc6845.VSync;
            // Detect falling edge of HSYNC

            if (!hSync && lastHSync)
            {
                gateArray.HSyncFallingEdge(vSync);
            }
            ((Z80Emulator)emulator).Interrupt = gateArray.InterruptState;
            lastHSync = hSync;
            lastVSync = vSync;
        }

        public override void InterruptAcknowledged()
        {
            gateArray.InterruptAcknowledged();
        }

        // Can be used to return an interrupt vector in interrupt mode IM0/IM2
        public override byte GetDataOnBus()
        {
            return 0xFF;
        }

        public PIO8255 PIO8255
        {
            get { return pio8255; }
        }

        public CRTC6845 CRTC6845
        {
            get { return crtc6845; }
        }

        public CPCScreen CPCScreen
        {
            get { return screen; }
        }

        public CPCKeyboard Keyboard
        {
            get { return keyboard; }
        }

        public PrinterPort PrinterPort
        {
            get { return printerPort; }
        }

        public GateArray GateArray
        {
            get { return gateArray; }
        }

        public AY3_8912 AY3_8912
        {
            get { return ay3_8912; }
        }

        private static byte[] LoadRom(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string fullName = assembly.GetManifestResourceNames().FirstOrDefault(nm => nm.Contains(name));
            if (!String.IsNullOrEmpty(fullName))
            {
                var resourceStream = assembly.GetManifestResourceStream(fullName);
                using var rd = new BinaryReader(resourceStream);
                return rd.ReadBytes(0x4000);
            }
            return null;
        }

        protected override byte ReadInput(int address)
        {
            switch (address & 0xFF00)
            {
                case 0xF400:        // PIO Port A
                    ay3_8912.Input = keyboard.ReadKey(pio8255.PortC & 0x0F);
                    pio8255.PortA = ay3_8912.Read((pio8255.PortC & 0x40) == 0x40, true, (pio8255.PortC & 0x80) == 0x80);
                    return pio8255.Read(0);
                case 0xF500:        // PIO Port B
                    pio8255.PortB = (byte)(
                        (lastVSync ? 0x01 : 0x00) |                         // bit 0
                        (((int)companyname) << 1) |                         // bits 1..3
                        (videoSystem == VideoSystem.PAL ? 0x10 : 0x00) |    // bit 4
                        /* set EXP here */                                  // bit 5
                        (printerPort.Busy ? 0x40 : 0x00)                    // bit 6
                        /* set recorder signal here */                      // bit 7
                    );
                    return pio8255.Read(1);
                case 0xF600:        // PIO Port C
                    return pio8255.Read(2);
                case 0xBF00:        // Select 6845 register 
                    return (byte)crtc6845.RegisterValue;
            }
            return 0;
        }

        protected override void WriteOutput(int address, byte value)
        {
            if ((address & 0xC000) == 0x4000)       // Gate Array
            {
                gateArray.Write(value);
            }
            else
            {
                switch (address & 0xFF00)
                {
                    case 0xBC00:        // Select 6845 register 
                        crtc6845.RegisterSelect = value;
                        break;
                    case 0xBD00:        // Write 6845 register data
                        crtc6845.RegisterValue = value;
                        break;
                    case 0xDF00:        // Write Upper ROM Bank Number 
                        upperRomBankNumber = value;
                        break;
                    case 0xF400:        // PIO Port A
                        pio8255.Write(0, value);
                        ay3_8912.Write((pio8255.PortC & 0x40) == 0x40, true, (pio8255.PortC & 0x80) == 0x80, pio8255.PortA);
                        break;
                    case 0xF500:        // PIO Port B
                        pio8255.Write(1, value);
                        break;
                    case 0xF600:        // PIO Port C
                        pio8255.Write(2, value);
                        ay3_8912.Write((pio8255.PortC & 0x40) == 0x40, true, (pio8255.PortC & 0x80) == 0x80, pio8255.PortA);
                        break;
                    case 0xEF00:        // Printerport
                        printerPort.Data = (byte)(value & 0x7F);
                        printerPort.Strobe = (value & 0x80) == 0;
                        break;
                }
            }
        }

        public override Symbols GetSymbols()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string fullName = assembly.GetManifestResourceNames().FirstOrDefault(nm => nm.Contains("Jumptable.txt"));
            if (!String.IsNullOrEmpty(fullName))
            {
                return Symbols.Load(assembly.GetManifestResourceStream(fullName));
            }
            return null;
        }
    }
}
