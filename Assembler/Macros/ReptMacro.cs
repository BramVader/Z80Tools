using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assembler.Macros
{
    /// <summary>
    /// Represents a macro that contains the lines defined between "macro {name}" and "endm"
    /// </summary>
    public class ReptMacro: Macro
    {
        public override TokenType Type => TokenType.Rept;

        public override bool RunDeferred => false;

        public override async Task Expand(MacroAssembler assembler, string operands, State state, OutputCollector outputCollector)
        {
            var macroState = state.BeginMacroExpansion(this);
            int? cnt = null;
            if (ParNames.Count == 1)
            {
                cnt = Assembler.Compiler.GetInt(ParNames[0], state);
            }
            if (cnt == null)
            {
                throw new InvalidOperationException("Expected one parameter expressing the number of REPT repetitions");
            }

            for (int i = 0; i < cnt.Value; i++)
            {
                foreach (var line in Lines)
                {
                    await assembler.AssembleLine(line, state, outputCollector);
                }
            }
            state.EndMacroExpansion();
        }
    }
}
