using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembler
{
    public enum Mode
    {
        Default,
        BlockComment
    }

    public class State
    {
        private readonly List<string> blockCommentCollector = new();
        private readonly Dictionary<string, Symbol> symbols = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Macro> macros = new(StringComparer.OrdinalIgnoreCase);

        private readonly int dummyCounter = 0;

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

        public void SetLabel(string name, SymbolType symbolType) =>
            SetLabel(name, Address, symbolType);

        public void SetLabel(string name, int address, SymbolType symbolType)
        {
            bool isPublic = name.EndsWith("::");
            name = name.TrimEnd(':');
            if (symbols.TryGetValue(name, out var lb))
            {
                if (lb.Value.HasValue)
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

        public int? GetSymbol(string name)
        {
            return symbols.TryGetValue(name, out Symbol value) ? value.Value : null;
        }

        public void SetSymbol(string name, int value, bool @readonly = false)
        {
            if (!symbols.TryGetValue(name, out var sm))
            {
                sm = new Symbol { Name = name };
                symbols.Add(name, sm);
            }
            if (@readonly && sm.Value.HasValue)
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
                ArgNames = args.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(it => it.Trim()).ToList(),
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

        public Macro GetMacro(string name) =>
            macros.TryGetValue(name, out var macro) ? macro : null;
    }
}
