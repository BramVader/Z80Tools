using System;
using System.Collections.Generic;
using System.Linq;
using Assembler.Macros;

namespace Assembler
{

    public class MacroState
    {
        private readonly Dictionary<string, Symbol> parentSymbols;
        private readonly Macro macro;

        public MacroState(Macro macro, Dictionary<string, Symbol> parentSymbols)
        {
            this.macro = macro;
            this.parentSymbols = parentSymbols;
        }

        private readonly Dictionary<string, Symbol> dummies = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> substitutes = new(StringComparer.OrdinalIgnoreCase);

        public Macro Macro => macro;

        public void SetArguments(object[] arguments)
        {
            foreach (var it in Macro.ParNames.Select((it, i) => new { Name = it, Value = i < arguments.Length ? arguments[i] : null }))
                SetSymbol(it.Name, it.Value, true);
        }

        public string SetSubstitute(string local, State state)
        {
            string substitute = state.GetUniqueLabel();
            this.substitutes.Add(local, substitute);
            return substitute;
        }

        public Symbol GetSymbol(string value)
        {
            (var symbol, var parts) = State.SplitLabel(value);
            foreach (var part in parts)
            {
                symbol += GetSubstitute(part);
            }
            return dummies.TryGetValue(symbol, out Symbol sym) ? sym : 
                parentSymbols.TryGetValue(symbol, out sym) ? sym :
                null;
        }

        public string GetSubstitute(string value) =>
            substitutes.TryGetValue(value, out string local) ? local : value;

        internal Symbol SetSymbol(Symbol symbol)
        {
            if (!dummies.TryGetValue(symbol.Name, out var sm) && !parentSymbols.TryGetValue(symbol.Name, out sm))
            {
                dummies.Add(symbol.Name, symbol);
                return symbol;
            }
            else
            {
                if (sm.Readonly && sm.Value != null)
                    throw new InvalidOperationException($"Symbol {sm.Name} is already set");
                sm.Value = symbol.Value;
                return sm;
            }
        }

        public Symbol SetSymbol(string name, object value, bool @readonly = false)
        {
            return SetSymbol(new Symbol
            {
                Name = name,
                Value = [value],
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
    }
}
