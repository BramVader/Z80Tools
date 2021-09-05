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
        private static readonly Regex lineRegex = new(@"^\s*(\S+:)?\s*(\.?\w+)?\s*((?:(?:'.*?')|(?:`.*?`)|(?:[^'`;]+))+)?\s*(;.*)?$".Replace('`', '"'), RegexOptions.Compiled | RegexOptions.CultureInvariant);

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
                await AssembleLine(line, state, outputCollector);
            }
        }

        protected abstract byte[] ParseOpcode(State state, OutputCollector outputCollector, string label, string opcode, string operands, string comment);

        protected abstract Task Initialize();

        protected static int ParseInt(State state, string data)
        {
            if (String.IsNullOrEmpty(data))
                throw new InvalidOperationException($"Integer expression expected");

            int? val = Compiler.GetInt(data, state);
            if (!val.HasValue && state.Pass == 2)
                throw new InvalidOperationException($"Integer expression evaluates to null");
            return val.GetValueOrDefault();
        }

        protected static bool ParseBool(State state, string data)
        {
            if (String.IsNullOrEmpty(data))
                throw new InvalidOperationException($"Boolean expression expected");

            bool? val = Compiler.GetBool(data, state);
            if (!val.HasValue && state.Pass == 2)
                throw new InvalidOperationException($"Boolean expression evaluates to null");
            return val.GetValueOrDefault();
        }

        protected static int[] ParseIntArray(State state, string data)
        {
            if (String.IsNullOrEmpty(data))
                return Array.Empty<int>();

            var objArr = Compiler.Get(data, state);
            if (!objArr.All(it => it.GetType() == typeof(int)))
                throw new InvalidOperationException($"Array does not contain only integers");
            return objArr.Cast<int>().ToArray();
        }

        private async Task ExpandMacro(Macro macro, string operands, State state, OutputCollector outputCollector)
        {
            var macroState = state.BeginMacroExpansion(macro);
            var arguments = Compiler.Get(operands, state);
            macroState.SetArguments(arguments);

            foreach (var line in macro.Lines)
            {
                await AssembleLine(line, state, outputCollector);
            }
        }

        private static string ComposeLabel(string operands, State state)
        {
            var parts = State.SplitLabel(operands);

            string symbol = parts[0];
            if (parts[1] != "")
            {
                if (state.CurrentExpansion == null)
                    throw new InvalidOperationException("Symbol expansion with '&' only allowed wihtin macro definition");
                var replacementName = state.CurrentExpansion.GetSubstitute(parts[1]);
                symbol += replacementName ?? throw new InvalidOperationException($"Unknown local symbol in macro {state.CurrentExpansion.Macro.Name}");
            }

            return symbol;
        }

        private async Task AssembleLine(string line, State state, OutputCollector outputCollector)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    await outputCollector.EmitComment(line);
                    return;
                }

                if (state.Mode == Mode.BlockComment)
                {
                    state.AddLineToBlockComment(line);
                    return;
                }

                if (line.StartsWith(';'))
                {
                    await outputCollector.EmitComment(line);
                    return;
                }

                var match = lineRegex.Match(line);
                if (!match.Success)
                    throw new InvalidOperationException($"Line has no correct format");

                string label = match.Groups[1].Value;
                string opcode = match.Groups[2].Value.ToUpper();
                string operands = match.Groups[3].Value.Trim();
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
                        await ExpandMacro(state.CurrentMacro, operands, state, outputCollector);
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
                    await ExpandMacro(macro, operands, state, outputCollector);
                }

                if (label != String.Empty && label.EndsWith(':'))
                {
                    state.SetLabel(ComposeLabel(label, state), state.SymbolType);
                }

                byte[] bytes = null;

                bool handled = HandleIfStatement(state, opcode, operands);

                // If inside an IF or ELSE condition, skip the rest if needed
                if (!handled && state.HasCondition)
                {
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

                        case "ERROR":
                            state.ThrowException(operands);
                            break;

                        case "LOCAL":
                            var macroState = state.CurrentExpansion;
                            if (macroState == null)
                                throw new InvalidOperationException("LOCAL only allowed within a macro");
                            macroState.SetSubstitute(ComposeLabel(operands, state), state);
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
                            bytes = HandleDw(state, opcode, operands);
                            break;

                        case "PUBLIC":
                            state.SetPublic(operands);
                            break;

                        default:
                            if (!String.IsNullOrEmpty(opcode))
                            {
                                bytes = ParseOpcode(state, outputCollector, label, opcode, operands, comment);
                                if (bytes == null)
                                    throw new InvalidOperationException($"Unknown opcode {opcode}");
                            }
                            break;
                    }
                }

                await outputCollector.Emit(label, state.Address, bytes, opcode, operands, comment);
                state.Address += bytes?.Length ?? 0;
            }
            catch (Exception e)
            {
                state.ThrowException(e.Message);
            }
        }

        private static byte[] HandleDs(State state, string opcode, string operands)
        {
            var par = Compiler.Get(operands, state);
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

            var par = Compiler.Get(operands, state);
            if (par == null || par.Length < 1)
                throw new InvalidOperationException($"{opcode} expects 1 or more integer parameters");
            if (par.Length == 1 && par[0] is object[] parr)
                par = parr;
            var bytes = par.Select(it =>
            {
                int value = it == null ? 0 : (int)it;
                if (value < -127 || value > 255)
                    throw new InvalidOperationException($"Value out of range for {opcode}");
                return (byte)value;
            }).ToArray();
            return bytes;
        }

        private static byte[] HandleDw(State state, string opcode, string operands)
        {
            var par = Compiler.Get(operands, state);
            if (par == null || par.Length < 1)
                throw new InvalidOperationException($"{opcode} expects 1 or more integer parameters");
            if (par.Length == 1 && par[0] is object[] parr)
                par = parr;
            var bytes = par.SelectMany(it =>
            {
                int value = it == null ? 0 : (int)it;
                if (value < -32768 || value > 65535)
                    throw new InvalidOperationException($"Value out of range for {opcode}");

                // TODO: make endianness dependant on processor architecture
                return new byte[] { (byte)(value & 0xff), (byte)(value >> 8) };
            }).ToArray();
            return bytes;
        }

        private bool HandleIfStatement(State state, string opcode, string operands)
        {
            switch (opcode)
            {
                case "IF":
                    // Check HasCondition to prevent evaluation of ParseBool
                    state.HandleIf(state.HasCondition && ParseBool(state, operands));
                    return true;

                case "ELSE":
                    state.HandleElse();
                    return true;

                case "ENDIF":
                    state.HandleEndIf();
                    return true;
            }
            return false;
        }
    }
}
