using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public async Task Assemble(OutputCollector outputCollector, StreamReader sr)
        {
            var state = new State();
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

        protected abstract byte[] ParseOpcode(State state, OutputCollector outputCollector, string label, string opcode, string operands, string comment);

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
            var macroState = state.BeginMacroExpansion(macro);
            var arguments = Compiler.Compile<object[]>(operands, state.Radix)(state) ?? Array.Empty<object>();
            macroState.SetArguments(arguments);

            foreach (var line in macro.Lines)
            {
                AssembleLine(line, state, outputCollector);
            }
        }

        private static readonly Regex symbolRegex = new(@"^([_A-Z\$.][_A-Z0-9\$.]*)?(?:\&([_A-Z\$.][_A-Z0-9\$.]*))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static string ComposeLabel(string operands, State state)
        {
            operands = operands.TrimEnd(':');
            var match = symbolRegex.Match(operands);
            if (!match.Success)
                throw new InvalidOperationException("Symbol expression expected");

            string symbol = match.Groups[1].Value.ToUpper();
            if (match.Groups[2].Value != "")
            {
                if (state.CurrentExpansion == null)
                    throw new InvalidOperationException("Symbol expansion with '&' only allowed wihtin macro definition");
                var replacementName = state.CurrentExpansion.GetLocal(match.Groups[2].Value);
                if (replacementName == null)
                    throw new InvalidOperationException($"Unknown local symbol in macro {state.CurrentExpansion.Macro.Name}");

                symbol += replacementName;
            }

            return symbol;
        }

        private void AssembleLine(string line, State state, OutputCollector outputCollector)
        {
            if (String.IsNullOrWhiteSpace(line))
            {
                outputCollector.Emit(line);
                return;
            }

            if (state.Mode == Mode.BlockComment)
            {
                state.AddLineToBlockComment(line);
                return;
            }

            if (line.StartsWith(';'))
            {
                outputCollector.Emit(line);
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
                return;
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
                state.SetLabel(ComposeLabel(label, state), state.SymbolType);
            }

            byte[] bytes = null;
            switch (opcode)
            {
                case ".TITLE":
                    // TODO
                    break;

                case ".RADIX":
                    if (!Int32.TryParse(operands, out int rdx) || rdx < 2 || rdx > 16)
                        throw new InvalidOperationException($"Invalid value {operands} for .RADIX");
                    state.Radix = rdx;
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

                case "LOCAL":
                    var macroState = state.CurrentExpansion;
                    if (macroState == null)
                        throw new InvalidOperationException("LOCAL only allowed within a macro");
                    macroState.SetLocal(ComposeLabel(operands, state), state);
                    break;

                case "ORG":
                    state.Address = ParseInt(state, operands);
                    break;

                case "EQU":
                    state.SetSymbol(label, ParseInt(state, operands), true);
                    break;

                case "DS":
                case "DEFS":
                    bytes = HandleDs(state, opcode, operands);
                    break;

                case "DB":
                case "DEFB":
                    bytes = HandleDb(state, opcode, operands);
                    break;

                case "DW":
                case "DEFW":
                    // TODO
                    break;

                case "PUBLIC":
                    state.SetPublic(operands);
                    break;

                default:
                    if (!String.IsNullOrEmpty(opcode))
                    {
                        bytes = ParseOpcode(state, outputCollector, label, opcode, operands, comment);
                        if (bytes == null)
                            throw new InvalidOperationException($"Unknown opcode {opcode} in line {state.LineNr}");
                    }
                    break;
            }
            outputCollector.Emit(label, state.Address, bytes, opcode, operands, comment);
            state.Address += bytes?.Length ?? 0;
        }

        private static byte[] HandleDs(State state, string opcode, string operands)
        {
            var par = Compiler.Compile<object[]>(operands, state.Radix)(state);
            if (par == null || par.Length < 1 || par.Length > 2)
                throw new InvalidOperationException($"{opcode} expects 1 or 2 integer parameters");
            int count = (int)par[0];
            int value = par.Length == 2 ? (int)par[1] : 0;
            if (count < 0 || count > 65535)
                throw new InvalidOperationException($"Invalid count for {opcode}");
            if (value < -127 || value > 255)
                throw new InvalidOperationException($"Invalid value for {opcode}");
            var bytes = new byte[count];
            Array.Fill(bytes, (byte)value);
            return bytes;
        }

        private static byte[] HandleDb(State state, string opcode, string operands)
        {
            // TODO: Handle strings of 1 or 2 chars (translate to lo/hi bytes
            // TODO: Handle longer strings

            var par = Compiler.Compile<object[]>(operands, state.Radix)(state);
            if (par == null || par.Length < 1)
                throw new InvalidOperationException($"{opcode} expects 1 or more integer parameters");
            var bytes = par.Select(it =>
            {
                int value = (int)it;
                if (value < -127 || value > 255)
                    throw new InvalidOperationException($"Invalid value for {opcode}");
                return (byte)value;
            }).ToArray();
            return bytes;
        }
    }
}
