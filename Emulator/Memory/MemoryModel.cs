﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Emulator
{
    public class MemoryModel
    {
        protected MemoryDescriptorList descriptors;
        protected int addressSpace = 0; // 64KiB
        protected int chunkSize;
        protected int chunkShift;
        protected int chunkCount;

        protected class MemoryChunk
        {
            public int offset;
            public byte[] currentRead;
            public byte[] currentWrite;
            public byte[][] memory;
            public int[] indices;
            public int currentIndexRead;
            public int currentIndexWrite;
        }

        protected MemoryChunk[] memoryChunks;

        public MemoryModel(params MemoryDescriptor[] descriptors)
        {
            this.descriptors = new MemoryDescriptorList(descriptors);
        }

        public Func<int, byte> ReadMemory { get; set; }
        public Action<int, byte> WriteMemory { get; set; }

        public int AddressSpace
        {
            get { return addressSpace; }
            set
            {
                addressSpace = value;
                InitMemory();
            }
        }

        public MemoryDescriptorList Descriptors
        {
            get { return descriptors; }
        }

        protected void InitMemory()
        {
            // Find memory boundaries
            var boundaries = new List<int>();
            for (int index1 = 0; index1 < descriptors.Count; index1++)
            {
                var descriptor = descriptors[index1];
                descriptor.Index = index1;
                int index2 = boundaries.BinarySearch(descriptor.Offset);
                if (index2 < 0)
                    boundaries.Insert(~index2, descriptor.Offset);
                index2 = boundaries.BinarySearch(descriptor.Offset + descriptor.Length);
                if (index2 < 0)
                    boundaries.Insert(~index2, descriptor.Offset + descriptor.Length);
            }

            // Find chunk size (powers of 2) and shift level 
            chunkShift = 16;
            for (int index = 0; index < boundaries.Count; index++)
            {
                int sh2 = 0;
                while (sh2 < 16 && ((boundaries[index] >> sh2) & 1) == 0) sh2++;
                chunkShift = Math.Min(chunkShift, sh2);
            }
            chunkSize = 1 << chunkShift;

            // Initialize memory chunks
            chunkCount = addressSpace / chunkSize;
            memoryChunks = new MemoryChunk[chunkCount];
            for (int m = 0; m < chunkCount; m++)
            {
                var chunk = new MemoryChunk
                {
                    offset = m * chunkSize
                };
                chunk.indices = descriptors.Where(mem => chunk.offset >= mem.Offset && chunk.offset < mem.Offset + mem.Length).Select(mem => mem.Index).ToArray();
                chunk.memory = new byte[chunk.indices.Length][];
                for (int n = 0; n < chunk.indices.Length; n++)
                {
                    chunk.memory[n] = new byte[chunkSize];
                }
                memoryChunks[m] = chunk;
            }

            ReadMemory = (adr) =>
            {
                adr &= 0xFFFF;
                return memoryChunks[adr >> chunkShift].currentRead[adr & chunkSize - 1];
            };
            WriteMemory = (adr, value) =>
            {
                adr &= 0xFFFF;
                if (descriptors[memoryChunks[adr >> chunkShift].currentIndexWrite].Type == MemoryType.Rom)
                {
                }
                memoryChunks[adr >> chunkShift].currentWrite[adr & chunkSize - 1] = value;
            };
        }

        public void SwitchMemory(params bool[] enabled)
        {
            foreach (MemoryChunk chunk in memoryChunks)
            {
                for (int n = 0; n < chunk.indices.Length; n++)
                    if (enabled[chunk.indices[n]])
                    {
                        chunk.currentRead = chunk.memory[n];
                        chunk.currentIndexRead = chunk.indices[n];
                        break;
                    }

                for (int n = 0; n < chunk.indices.Length; n++)
                    if (enabled[chunk.indices[n]] && descriptors[chunk.indices[n]].Type != MemoryType.Rom)
                    {
                        chunk.currentWrite = chunk.memory[n];
                        chunk.currentIndexWrite = chunk.indices[n];
                        break;
                    }
            }
        }

        public byte Read(int address, params bool[] enabled)
        {
            var chunk = memoryChunks[address >> chunkShift];
            for (int i = 0; i < chunk.indices.Length; i++)
            {
                if (enabled[chunk.indices[i]])
                {
                    return chunk.memory[i][address & (chunkSize - 1)];
                }
            }
            return 0;
        }

        public byte[] Read(int address, int length, params bool[] enabled)
        {
            if (length == chunkSize && address % chunkSize == 0)
            {
                var chunk = memoryChunks[address >> chunkShift];
                for (int i = 0; i < chunk.indices.Length; i++)
                    if (enabled[chunk.indices[i]])
                        return chunk.memory[i];
            }
            byte[] result = new byte[length];
            int offset1 = 0;
            int chunkIndex = address >> chunkShift;
            do
            {
                var chunk = memoryChunks[chunkIndex++];
                for (int i = 0; i < chunk.indices.Length; i++)
                {
                    if (enabled[chunk.indices[i]])
                    {
                        int offset2 = Math.Max(address - chunk.offset - offset1, 0);
                        int size = Math.Min(length - offset1, chunkSize);
                        Array.Copy(chunk.memory[i], offset2, result, offset1, size);
                        offset1 += size;
                        break;
                    }
                }
            }
            while (offset1 < length);
            return result;
        }

        public void Write(int address, byte value, params bool[] enabled)
        {
            var chunk = memoryChunks[address >> chunkShift];
            for (int i = 0; i < chunk.indices.Length; i++)
            {
                if (enabled[chunk.indices[i]])
                {
                    chunk.memory[i][address & (chunkSize - 1)] = value;
                }
            }
        }

        public void Write(byte[] data, int address, params bool[] enabled)
        {
            int offset1 = 0;
            int length = data.Length;
            int chunkIndex = address >> chunkShift;
            do
            {
                var chunk = memoryChunks[chunkIndex++];
                for (int i = 0; i < chunk.indices.Length; i++)
                {
                    if (enabled[chunk.indices[i]])
                    {
                        int offset2 = Math.Max(address - chunk.offset - offset1, 0);
                        int size = Math.Min(length - offset1, chunkSize);
                        Array.Copy(data, offset1, chunk.memory[i], offset2, size);
                        offset1 += size;
                        break;
                    }
                }
            }
            while (offset1 < length);
        }

        public MemoryDescriptor GetMemoryDescriptor(int address, params bool[] enabled)
        {
            var chunk = memoryChunks[address >> chunkShift];
            for (int i = 0; i < chunk.indices.Length; i++)
            {
                if (enabled[chunk.indices[i]])
                    return descriptors[i];
            }
            return null;
        }

    }
}
