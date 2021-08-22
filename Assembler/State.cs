using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Assembler
{
    public enum Mode
    {
        Default,
        BlockComment
    }

    public class State
    {
        public class MacroState
        {
            public MacroState(Macro macro)
            {
                this.Macro = macro;
            }

            private readonly Dictionary<string, string> locals = new(StringComparer.OrdinalIgnoreCase);
            private readonly Dictionary<string, Symbol> symbols = new(StringComparer.OrdinalIgnoreCase);

            public Macro Macro { get; }

            public void SetArguments(object[] arguments)
            {
                foreach (var it in Macro.ParNames.Select((it, i) => new { Name = it, Value = i < arguments.Length ? arguments[i] : null }))
                    symbols.Add(it.Name, new Symbol { Name = it.Name, Value = it.Value });
            }

            public string SetLocal(string local, State state)
            {
                string replacement = state.GetUniqueLabel();
                locals.Add(local, replacement);
                return replacement;
            }

            public Symbol GetSymbol(string value) =>
                symbols.TryGetValue(value, out Symbol sym) ? sym : null;

            public string GetLocal(string value) =>
                locals.TryGetValue(value, out string local) ? local : null;
        }

        private readonly List<string> blockCommentCollector = new();
        private readonly Dictionary<string, Symbol> symbols = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Macro> macros = new(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<MacroState> macroStates = new();

        private int dummyCounter = 0;

        public int LineNr { get; set; }

        public int Address { get; set; }

        public int Radix { get; set; } = 10;

        public SymbolType SymbolType { get; set; } = SymbolType.CodeRelative;

        public Mode Mode { get; set; }

        public Macro CurrentMacro { get; set; }

        public int MacroDepth { get; set; } = 0;

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

        public void SetLabel(string name, SymbolType symbolType) =>
            SetLabel(name, Address, symbolType);

        public void SetLabel(string name, int address, SymbolType symbolType)
        {
            bool isPublic = name.EndsWith("::");
            name = name.TrimEnd(':');
            if (symbols.TryGetValue(name, out var lb))
            {
                if (lb.Value != null)
                    throw new InvalidOperationException($"Label {name} already defined");
                isPublic |= lb.IsPublic;
            }
            symbols[name] = new Symbol
            {
                Value = address,
                IsPublic = isPublic,
                Name = name,
                Type = symbolType
            };
        }

        public int? GetSymbolAsWord(string name)
        {
            Symbol symbol = null;
            foreach (var expansion in macroStates)
            {
                symbol = expansion.GetSymbol(name);
                if (symbol != null) break;
            }

            if (symbol == null && (!symbols.TryGetValue(name, out symbol) || symbol.Value == null))
                return null;
            switch (symbol.Value)
            {
                case object[] arr:
                    if (arr.Length != 1)
                        throw new InvalidOperationException("A single value expected");
                    return (int?)arr[0];
                case int a:
                    return a;
                default:
                    throw new InvalidOperationException("Unexpected type");
            }
        }

        public void SetSymbol(string name, int value, bool @readonly = false)
        {
            if (!symbols.TryGetValue(name, out var sm))
            {
                sm = new Symbol { Name = name };
                symbols.Add(name, sm);
            }
            if (@readonly && sm.Value != null)
                throw new InvalidOperationException($"Symbol {name} is already set");
            sm.Value = value;
            sm.Readonly = @readonly;
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

        public Macro BeginMacro(string name, string args, TokenType macroType)
        {
            var macro = new Macro
            {
                Name = name,
                ParNames = args.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(it => it.Trim()).ToList(),
                MacroType = macroType
            };
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

        public MacroState BeginMacroExpansion(Macro macro)
        {
            var expansion = new MacroState(macro);
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
    }
}
