﻿using Disassembler;

namespace Emulator
{
    public abstract class HardwareModel
    {
        protected MemoryModel memoryModel;
        protected BaseEmulator emulator;
        protected bool[] memorySwitch;

        protected abstract byte ReadInput(int address);
        protected abstract void WriteOutput(int address, byte value);
        
        public abstract void Reset();
        public abstract void InterruptAcknowledged();
        public abstract void AfterInstruction(long stateCounter);
        public abstract byte GetDataOnBus();

        public MemoryModel MemoryModel
        {
            get { return memoryModel; }
        }

        public BaseEmulator Emulator
        {
            get { return emulator; }
        }

        public bool[] MemorySwitch
        {
            get { return memorySwitch; }
        }

        public abstract Symbols GetSymbols();
    }
}
