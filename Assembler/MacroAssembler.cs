using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler
{
    public abstract class MacroAssembler
    {
        public class Symbol
        {
            public int? Value { get; set; }

            public bool Public { get; set; }    // When declared with double colon (::)

            public string Name { get; set; }
        }

        public class Macro
        {
            public string Name { get; set; }
            public List<string> ArgNames { get; set; }
        }

        public enum Mode
        {
            Default,
            BlockComment
        }

        public class State
        {
            private List<string> blockCommentCollector = new List<string>();
            private readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();
            private readonly Dictionary<string, Macro> macros = new Dictionary<string, Macro>();

            public int LineNr { get; set; }

            public int Address { get; set; }

            public int Radix { get; set; } = 10;

            public Mode Mode { get; set; }

            public void AddLineToBlockComment(string st)
            {
                blockCommentCollector.Add(st);
            }

            public string FinalizeBlockComment()
            {
                string st = blockCommentCollector.ToString();
                blockCommentCollector.Clear();

                return st;
            }

            public void SetLabel(string name) =>
                SetLabel(name, Address);

            public void SetLabel(string name, int address)
            {
                bool isPublic = name.EndsWith("::");
                name = name.TrimEnd(':');
                isPublic |= symbols.TryGetValue(name, out var lb) && lb.Public;
                symbols[name] = new Symbol { Value = address, Public = isPublic, Name = name };
            }

            public int? GetSymbol(string name)
            {
                return symbols.TryGetValue(name, out Symbol value) ? value.Value : null;
            }

            public void SetPublic(string name)
            {
                if (!symbols.TryGetValue(name, out var lb))
                {
                    lb = new Symbol { Name = name };
                }
                lb.Public = true;
            }

            public Macro CheckMacro(string name)
            {
                return macros.TryGetValue(name, out var macro) ? macro : null;
            }

            public void SetMacro(string name, string args)
            {
                var macro = new Macro
                {
                    Name = name
                };
                macros.Add(name, macro);
            }
        }

        protected class OutputCollector
        {
            public void AddComment(string st)
            {

            }

            public void Emit(byte v)
            {

            }
        }

        public Task Assemble(StreamReader sr)
        {
            return Assemble(sr, new State(), new OutputCollector());
        }

        // Trims the line. When there's whitespace in strings, they will
        // be replaced by their ASCII value + 128 (137 and 160)
        private static string SantizeLine(string line)
        {
            line = line.Trim();
            var chars = line.ToCharArray();
            char mode = '\0';
            bool modified = false;
            for (int n = 0; n < chars.Length; n++)
            {
                char ch = chars[n];
                switch (ch)
                {
                    case '\'':
                    case '"':
                        if (mode == '\0')
                            mode = ch;
                        else if (mode == ch)
                            mode = '\0';
                        break;
                    case '\t':
                        if (mode != '\0')
                        {
                            chars[n] = (char)254;
                            modified = true;
                        }
                        break;
                    case ' ':
                        if (mode != '\0')
                        {
                            chars[n] = (char)255;
                            modified = true;
                        }
                        break;
                }
            }
            if (modified)
                return new string(chars);
            return line;
        }

        private static readonly Regex lineRegex = new(@"^\s*([^\s:]+:)?\s*(\.?\w+)?\s*(\S+)?\s*(;.*)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        protected abstract bool TryOpcode(State state, OutputCollector outputCollector, string opcode, string operands);

        protected abstract Task Initialize();

        protected static int ParseInt(State state, string data)
        {
            if (String.IsNullOrEmpty(data))
                throw new InvalidOperationException($"Integer expression expected in line {state.LineNr}");

            var tokens = Tokenizer.Tokenize(data, state.Radix);

            // A numeric constant
            if (Char.IsDigit(data[0]))
            {
                char last = data[^1];
                int radix = Char.ToUpper(last) switch
                {
                    'B' => 2,
                    'D' => 10,
                    'H' => 16,
                    _ => 0
                };
                if (radix != 0)
                {
                    data = data[0..^1];
                }
                else
                {
                    radix = state.Radix;
                }
                return Convert.ToInt32(data, radix);
            }

            return 0;
            //throw new InvalidOperationException($"Cannot parse integer in line {state.LineNr}");
        }

        private async Task Assemble(StreamReader sr, State state, OutputCollector outputCollector)
        {
            await Initialize();
            while (!sr.EndOfStream)
            {
                state.LineNr++;
                string line = await sr.ReadLineAsync();
                if (String.IsNullOrWhiteSpace(line))
                    continue;

                if (state.Mode == Mode.BlockComment)
                {
                    state.AddLineToBlockComment(line);
                    continue;
                }

                if (line.StartsWith(';'))
                {
                    outputCollector.AddComment(line);
                    continue;
                }

                line = SantizeLine(line);

                var match = lineRegex.Match(line);
                if (!match.Success)
                    throw new InvalidOperationException($"Syntax error at line {state.LineNr}");
                string label = match.Groups[1].Value;
                string opcode = match.Groups[2].Value.ToUpper();
                string operands = match.Groups[3].Value.Replace((char)254, '\t').Replace((char)255, ' ');
                string comment = match.Groups[4].Value;

                if (label != String.Empty)
                {
                    state.SetLabel(label);
                }

                var macro = state.CheckMacro(opcode);
                if (macro != null)
                {

                    continue;
                }

                switch (opcode)
                {
                    case ".TITLE":
                    case "ASEG":
                        // TODO
                        break;

                    case ".RADIX":
                        if (!Int32.TryParse(operands, out int rdx) || rdx < 2 || rdx > 16)
                            throw new InvalidOperationException($"Invalid value {operands} for .RADIX");
                        state.Radix = rdx;
                        break;

                    case "ORG":
                        state.Address = ParseInt(state, operands);
                        break;

                    case "DS":
                    case "DEFS":
                        state.Address += ParseInt(state, operands);
                        break;

                    case "DB":


                    case "PUBLIC":
                        state.SetPublic(operands);
                        break;
                    default:
                        if (!TryOpcode(state, outputCollector, opcode, operands))
                        {
                            throw new InvalidOperationException($"Unknown opcode {opcode} in line {state.LineNr}");
                        }
                        break;
                }
            }
        }
    }
}
