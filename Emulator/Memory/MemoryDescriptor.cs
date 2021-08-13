using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emulator
{
    public enum MemoryType
    {
        Ram,
        Rom
    }

    public class MemoryDescriptorList : List<MemoryDescriptor>
    {
        public MemoryDescriptorList(IEnumerable<MemoryDescriptor> descriptors): base(descriptors)
        {
        }
        
        public MemoryDescriptor this[string name]
        {
            get { return this.Where(md => md.Name == name).Single(); }
        }

        public bool[] GetEnabled(params MemoryDescriptor[] descriptors)
        {
            bool[] result = new bool[this.Count];
            foreach (MemoryDescriptor descriptor in descriptors)
                result[descriptor.Index] = true;
            return result;
        }
    }

    public class MemoryDescriptor
    {
        public MemoryType Type { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        public string Name { get; set; }

        public int Index { get; internal set; }

        public MemoryDescriptor(string name, MemoryType type, int offset, int length)
        {
            this.Name = name;
            this.Type = type;
            this.Offset = offset;
            this.Length = length;
        }

    }
}
