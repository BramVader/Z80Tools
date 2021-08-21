using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assembler
{
    public abstract class MacroAssembler
    {
        private static readonly Regex lineRegex = new(@"^\s*([^\s:]+:)?\s*(\.?\w+)?\s*(\S+)?\s*(;.*)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Dictionary<string, TokenType> macroMap = new()
        {
            ["MACRO"] = TokenType.Macro,
            ["REPT"] = TokenType.Rept,
            ["IRP"] = TokenType.Irp,
            ["IRPC"] = TokenType.Irpc
        };

        public async Task Assemble(StreamReader sr)
        {
            var state = new State();
            var outputCollector = new OutputCollector();
            await Initialize();
            while (!sr.EndOfStream)
            {
                state.LineNr++;
                string line = await sr.ReadLineAsync();
                AssembleLine(line, state, outputCollector);
            }
        }

        // Trims the line. When there's whitespace in strings, they will
        // be replaced by their ASCII values 254 (tab) and 255 (space)
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

        protected abstract bool TryOpcode(State state, OutputCollector outputCollector, string opcode, string operands);

        protected abstract Task Initialize();

        protected static int ParseInt(State state, string data)
        {
            if (String.IsNullOrEmpty(data))
                throw new InvalidOperationException($"Integer expression expected in line {state.LineNr}");

            var fun = Compiler.Compile<int>(data, state.Radix);
            return fun(state);
        }

        protected static int[] ParseIntArray(State state, string data)
        {
            if (String.IsNullOrEmpty(data))
                return Array.Empty<int>();

            var fun = Compiler.Compile<int[]>(data, state.Radix);
            return fun(state);
        }


        private void ExpandMacro(Macro macro, string operands, State state, OutputCollector outputCollector)
        {
            var tokens = Tokenizer.Tokenize(operands, state.Radix);
            foreach (var line in macro.Lines)
            {
                AssembleLine(line, state, outputCollector);
            }
        }


        private void AssembleLine(string line, State state, OutputCollector outputCollector)
        {
            if (String.IsNullOrWhiteSpace(line))
                return;

            if (state.Mode == Mode.BlockComment)
            {
                state.AddLineToBlockComment(line);
                return;
            }

            if (line.StartsWith(';'))
            {
                outputCollector.AddComment(line);
                return;
            }

            line = SantizeLine(line);

            var match = lineRegex.Match(line);
            string label = match.Groups[1].Value;
            string opcode = match.Groups[2].Value.ToUpper();
            string operands = match.Groups[3].Value.Replace((char)254, '\t').Replace((char)255, ' ');
            string comment = match.Groups[4].Value;

            if (macroMap.TryGetValue(opcode, out TokenType macroType))
            {
                state.BeginMacro(label.TrimEnd(':'), operands, macroType);
            }
            else if (opcode == "ENDM")
            {
                state.CurrentMacro.AddLine(line);

                // If macro has no name apply it immediately (inline)
                if (String.IsNullOrWhiteSpace(state.CurrentMacro.Name))
                {
                    ExpandMacro(state.CurrentMacro, operands, state, outputCollector);
                }

                state.EndMacro();
                return;
            }

            if (state.CurrentMacro != null)
            {
                state.CurrentMacro.AddLine(line);
                return;
            }

            if (!match.Success)
                throw new InvalidOperationException($"Syntax error at line {state.LineNr}");

            var macro = state.GetMacro(opcode);
            if (macro != null)
            {
                ExpandMacro(macro, operands, state, outputCollector);
            }

            if (label != String.Empty && label.EndsWith(':'))
            {
                state.SetLabel(label, state.SymbolType);
            }

            switch (opcode)
            {
                case ".TITLE":
                    // TODO
                    break;

                case "ASEG":
                    state.SymbolType = SymbolType.Absolute;
                    break;

                case "CSEG":
                    state.SymbolType = SymbolType.CodeRelative;
                    break;

                case "DSEG":
                    state.SymbolType = SymbolType.DataRelative;
                    break;

                case ".RADIX":
                    if (!Int32.TryParse(operands, out int rdx) || rdx < 2 || rdx > 16)
                        throw new InvalidOperationException($"Invalid value {operands} for .RADIX");
                    state.Radix = rdx;
                    break;

                case "ORG":
                    state.Address = ParseInt(state, operands);
                    break;

                case "EQU":
                    state.SetSymbol(label, ParseInt(state, operands), true);
                    break;

                case "DS":
                case "DEFS":
                    state.Address += ParseInt(state, operands);
                    break;

                case "DB":
                case "DEFB":
                    var tokens = Tokenizer.Tokenize(operands, state.Radix);
                    // TODO
                    break;

                case "DW":
                case "DEFW":
                    // TODO
                    break;

                case "PUBLIC":
                    state.SetPublic(operands);
                    break;

                default:
                    if (!String.IsNullOrEmpty(opcode) && !TryOpcode(state, outputCollector, opcode, operands))
                    {
                        throw new InvalidOperationException($"Unknown opcode {opcode} in line {state.LineNr}");
                    }
                    break;
            }
        }

    }
}
