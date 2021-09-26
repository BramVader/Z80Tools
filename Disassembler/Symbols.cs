using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Disassembler
{
    public class Symbols
    {
        IDictionary<string, IList<Symbol>> symbolsByName = new Dictionary<string, IList<Symbol>>();
        IDictionary<int, IList<Symbol>> symbolsByValue = new Dictionary<int, IList<Symbol>>();

        public Symbols() : base()
        {
        }

        public Symbols(IEnumerable<Symbol> collection)
        {
            foreach (var symbol in collection)
                Add(symbol);
        }

        public void Add(Symbol symbol)
        {
            IList<Symbol> list;
            if (symbol.Name != null)
            {
                if (!symbolsByName.TryGetValue(symbol.Name, out list))
                {
                    list = new List<Symbol>();
                    symbolsByName.Add(symbol.Name, list);
                }
                if (!list.Any(it => it.Value == symbol.Value))
                    list.Add(symbol);
            }

            if (!symbolsByValue.TryGetValue(symbol.Value, out list))
            {
                list = new List<Symbol>();
                symbolsByValue.Add(symbol.Value, list);
            }
            if (!list.Any(it => it.Name == symbol.Name))
                list.Add(symbol);
        }

        public IEnumerable<Symbol> FindSymbols(int address)
        {
            if (!symbolsByValue.TryGetValue(address, out var list))
                list = new List<Symbol>();
            return list;
        }

        public IEnumerable<Symbol> FindSymbols(string name)
        {
            if (!symbolsByName.TryGetValue(name, out var list))
                list = new List<Symbol>();
            return list;
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
            return list;
        }
    }
}
