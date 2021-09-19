using Disassembler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BdosCpm
{
    public class ListFileReader
    {
        private static Regex addressMatch = new Regex(@"([0-9A-F]{4}):");
        private static Regex symbolMatch = new Regex(@"^\s*([0-9A-F]{4}):\s(\w+?)\s*$");
        private static Regex inlineCommentMatch = new Regex(@"([0-9A-F]{4}):.*?;\s*(.*?)$");
        private static Regex commentMatch = new Regex(@"^\s*(?:\d+)?\s*;\s*(.+?)$");

        public Symbols Read(StreamReader sr)
        {
            var symbols = new Dictionary<int, Symbol>();

            bool symbolMode = false;
            int addr = -1;
            Match match;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line == "Symbols")
                {
                    symbolMode = true;
                    continue;
                }
                if (!symbolMode)
                {
                    match = addressMatch.Match(line);
                    if (match.Success) addr = Int32.Parse(match.Groups[1].Value, NumberStyles.HexNumber);

                    match = inlineCommentMatch.Match(line);
                    if (match.Success)
                    {
                        addr = Int32.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                        if (!symbols.TryGetValue(addr, out var val))
                        {
                            val = new Symbol { Value = addr, Type = SymbolType.Label, MemoryMask = new[] { true } };
                            symbols.Add(addr, val);
                        }
                        val.Comment = match.Groups[2].Value;
                    }

                    match = commentMatch.Match(line);
                    if (match.Success)
                    {
                        if (!symbols.TryGetValue(addr, out var val))
                        {
                            val = new Symbol { Value = addr, Type = SymbolType.Comment, MemoryMask = new[] { true } };
                            symbols.Add(addr, val);
                        }
                        val.Comment = val.Comment == null ? match.Groups[2].Value : val.Comment + "\r\n" + match.Groups[2].Value;
                    }
                }
                else
                {
                    match = symbolMatch.Match(line);
                    if (match.Success)
                    {
                        addr = Int32.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                        if (!symbols.TryGetValue(addr, out var val))
                        {
                            val = new Symbol { Value = addr, Type = SymbolType.Constant };
                            symbols.Add(addr, val);
                        }
                        val.Name = match.Groups[2].Value;
                    }
                }
            }
            return new Symbols(symbols.OrderBy(it => it.Key).Select(it => it.Value));
        }
    }
}
