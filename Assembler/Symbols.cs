using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assembler
{
    public class Symbols: List<Symbol>
    {
        public Symbols() : base()
        {
        }
        
        public Symbols(IEnumerable<Symbol> collection)
            : base(collection)
        {
        }

        public IEnumerable<Symbol> FindSymbols(int address)
        {
            var check = new Symbol() { Value = address };
            var comparer = new Symbol.Comparer();
            int index = this.BinarySearch(check, comparer);
            var results = new List<Symbol>();
            if (index >= 0)
            {
                while (index >= 0 && comparer.Compare(check, this[index]) == 0)
                    index--;
                index++;
                while (index < Count && comparer.Compare(check, this[index]) == 0)
                    results.Add(this[index++]);
            }
            return results;
        }

        public static Symbols Load(string filename)
        {
            var list = new Symbols();
            foreach (string st in File.ReadAllLines(filename))
            {
                string line = st.Replace('\t', ' ').Trim();
                if (line.Length > 0)
                {
                    if (!line.StartsWith(";"))
                    {
                        int index1 = line.IndexOf(' ');
                        if (index1 != -1)
                        {
                            int index2 = line.IndexOf(';', index1);
                            if (index2 == -1)
                                index2 = line.Length;

                            var symbol = new Symbol
                            {
                                Value = Convert.ToInt32(line.Substring(0, index1), 16),
                                Name = line[(index1 + 1) .. (index2 - 1)].Trim()
                            };
                            if (index2 < line.Length)
                                symbol.Comment = line[(index2 + 1)..].Trim();
                            else
                                symbol.Comment = String.Empty;

                            list.Add(symbol);
                        }
                    }
                }
            }
            list.Sort(new Symbol.Comparer());
            return list;
        }
    }
}
