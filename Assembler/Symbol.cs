using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    public enum SymbolType
    {
        Constant,
        Label
    }
    
    public class Symbol
    {
        public class Comparer : IComparer<Symbol>
        {
            public int Compare(Symbol x, Symbol y)
            {
                return x.Value.CompareTo(y.Value);
            }
        }
        
        public SymbolType Type { get; set; }
        public bool[] MemoryMask { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
    }
}
