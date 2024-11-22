using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Assembler.Macros;

namespace Assembler
{
    public enum Mode
    {
        Default,
        BlockComment
    }

    public class State
    {
        private static readonly Regex symbolRegex = new(@"^([_A-Z$?@.][_A-Z0-9$?@.]*?)?(?:&([_A-Z$?@.][_A-Z0-9$?@.]*?))*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Split the label into fixed and substitute parts (each starting with '&')
        // e.g.
        //  - ERR    is split into ("ERR", []), 
        //  - ERR&Y  is split into ("ERR", ["Y"]), 
        //  - &ERR&Y is split into ("", ["ERR", "Y"])
        public static (string, string[]) SplitLabel(string operands)
        {
            operands = operands.TrimEnd(':');
            var match = symbolRegex.Match(operands);
            if (!match.Success)
                throw new InvalidOperationException("Symbol expression expected");
            return (match.Groups[1].Value, match.Groups[2].Captures.Select(it => it.Value).ToArray());
        }

        private readonly List<string> blockCommentCollector = new();
        private readonly Dictionary<string, Symbol> symbols = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Macro> macros = new(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<MacroState> macroStates = new();
        private readonly Stack<bool> ifStates = new();

        private int dummyCounter = 0;

        public int LineNr { get; set; }

        public int Address { get; set; }

        public int Radix { get; set; } = 10;

        public SymbolType SymbolType { get; set; } = SymbolType.CodeRelative;

        public Mode Mode { get; set; }

        public Macro CurrentMacro { get; set; }

        public int MacroDepth { get; set; } = 0;

        public int Pass { get; set; } = 1;


        public void ThrowException(string message)
        {
            throw new InvalidOperationException($"{message} in line {LineNr}");
        }

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

        public string GetUniqueLabel()
        {
            return $"..{Interlocked.Increment(ref dummyCounter):X4}";
        }

        public Symbol SetLabel(string name, SymbolType symbolType, bool isPublic) =>
            SetLabel(name, Address, symbolType, isPublic);

        public Symbol SetLabel(string name, int address, SymbolType symbolType, bool isPublic)
        {
            name = name.TrimEnd(':');
            var symbol = SetSymbol(name, address);
            symbol.IsPublic = isPublic;
            symbol.Type = symbolType;
            return symbol;
        }

        public object[] GetSymbol(string name)
        {
            Symbol symbol = null;
            foreach (var expansion in macroStates)
            {
                symbol = expansion.GetSymbol(name);
                if (symbol != null) break;
            }

            if (symbol == null && (!symbols.TryGetValue(name, out symbol) || symbol.Value == null))
            {
                if (Pass == 1)
                    ThrowException($"Unknown symbol {name}");
                return new object[] { null };
            }

            return symbol.Value;
        }

        public int? GetSymbolAsWord(string name)
        {
            var objArr = GetSymbol(name);
            if (objArr.Length != 1)
                throw new InvalidOperationException("A single value expected");
            return (int?)objArr[0];
        }

        public Symbol SetSymbol(Symbol symbol)
        {
            var curr = CurrentExpansion;
            if (curr != null)
            {
                return curr.SetSymbol(symbol);
            }
            else
            {
                if (!symbols.TryGetValue(symbol.Name, out var sm))
                {
                    symbols.Add(symbol.Name, symbol);
                    return symbol;
                }
                else
                {
                    if (sm.Readonly && sm.Value != null && !sm.HasEqualValue(symbol))
                        throw new InvalidOperationException($"Symbol {symbol.Name} is already set");
                    sm.Value = symbol.Value;
                    return sm;
                }
            }
        }

        public IEnumerable<Symbol> Symbols => symbols.Values.OrderBy(it => it.Name);

        public Symbol SetSymbol(string name, object value, bool @readonly = false)
        {
            return SetSymbol(new Symbol
            {
                Name = name,
                Value = new object[] { value },
                Readonly = @readonly,
            });
        }

        public Symbol SetSymbol(string name, object[] value, bool @readonly = false)
        {
            return SetSymbol(new Symbol
            {
                Name = name,
                Value = value,
                Readonly = @readonly,
            });
        }

        public int GetLocationCounter()
        {
            // TODO: Apply relocation
            return Address;
        }

        public void SetPublic(string name)
        {
            if (!symbols.TryGetValue(name, out var sm))
            {
                sm = new Symbol { Name = name };
                symbols.Add(name, sm);
            }
            sm.IsPublic = true;
        }

        // Macro support

        public Macro BeginMacro(string name, string args, TokenType macroType)
        {
            var macro = macroType switch
            {
                TokenType.Macro => new Macro(),
                TokenType.Rept => new ReptMacro(),
                //TokenType.Irp => new IrpMacro(),
                //TokenType.Irpc => new IrpcMacro(),
                _ => throw new NotImplementedException($"A macro type {macroType} is not implemented")
            };
            macro.Name = name;
            macro.ParNames = args.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(it => it.Trim()).ToList();
            if (!String.IsNullOrWhiteSpace(name)) macros.Add(name, macro);
            MacroDepth++;
            if (MacroDepth == 1) CurrentMacro = macro;
            return macro;
        }

        public void EndMacro()
        {
            MacroDepth--;
            if (MacroDepth == 0)
                CurrentMacro = null;
        }

        public void ClearExeptSymbols()
        {
            blockCommentCollector.Clear();
            macros.Clear();
            macroStates.Clear();
            ifStates.Clear();
            dummyCounter = 0;
            LineNr = 0;
            Address = 0;
            Radix = 10;
            SymbolType = SymbolType.CodeRelative;
            Mode = Mode.Default;
            CurrentMacro = null;
            MacroDepth = 0;
        }

        public MacroState BeginMacroExpansion(Macro macro)
        {
            var expansion = new MacroState(macro, symbols);
            macroStates.Push(expansion);
            return expansion;
        }
        public void EndMacroExpansion()
        {
            macroStates.Pop();
        }

        public MacroState CurrentExpansion => macroStates.Count > 0 ? macroStates.Peek() : null;

        public Macro GetMacro(string name) =>
            macros.TryGetValue(name, out var macro) ? macro : null;

        // IF - conditions

        public void HandleIf(bool cond)
        {
            ifStates.Push(cond);
        }
        public void HandleElse()
        {
            if (ifStates.Count == 0)
                throw new InvalidOperationException("ELSE without IF");
            bool cond = ifStates.Pop();
            ifStates.Push(!cond);
        }

        public void HandleEndIf()
        {
            if (ifStates.Count == 0)
                throw new InvalidOperationException("ENDIF without IF");
            ifStates.Pop();
        }

        public bool HasCondition =>
            ifStates.Count == 0 || ifStates.All(it => it);
    }
}
