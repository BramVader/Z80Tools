﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emulator;

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
    }
}
