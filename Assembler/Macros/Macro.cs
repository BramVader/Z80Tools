using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assembler.Macros
{
    /// <summary>
    /// Represents a macro that contains the lines defined between "macro {name}" and "endm"
    /// </summary>
    public class Macro
    {
        protected readonly List<string> lines = new();

        /// <summary>
        /// The name of the macro
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parameter names of the macro
        /// </summary>
        public List<string> ParNames { get; set; }

        /// <summary>
        /// The list of lines that make up the macro
        /// </summary>
        public IEnumerable<string> Lines => lines;

        /// <summary>
        /// Add a line to the list of lines
        /// </summary>
        /// <param name="line"></param>        
        public void AddLine(string line)
        {
            lines.Add(line);
        }

        /// <summary>
        /// The type of the macro, can be either:
        /// - TokenType.Macro ("MACRO")
        /// - TokenType.Rept ("REPT")
        /// - TokenType.Irp ("IRP")
        /// - TokenType.Irpc ("IRPC")
        /// </summary>
        public virtual TokenType Type { get; }

        /// <summary>
        /// MACRO types run deferred, e.g. definition and running are separate
        /// the other macro types run immediately
        /// </summary>
        public virtual bool RunDeferred { get; } = true;

        public virtual async Task Expand(MacroAssembler assembler, string operands, State state, OutputCollector outputCollector)
        {
            var macroState = state.BeginMacroExpansion(this);
            var arguments = Compiler.Get(operands, state) ?? [];
            macroState.SetArguments(arguments);

            // Document the macro arguments
            if (state.Pass == 1)
            {
                await outputCollector.EmitComment(state.LineNr, $"; CALLING {Name}");
                foreach (var it in macroState.Macro.ParNames.Select((it, i) => new { Name = it, Value = i < arguments.Length ? arguments[i] : null }))
                {
                    await outputCollector.EmitComment(state.LineNr, $"; - {it.Name}: {OutputCollector.ValueToString(it.Value)}");
                }
            }
            foreach (var line in Lines)
            {
                await assembler.AssembleLine(line, state, outputCollector);
            }
            state.EndMacroExpansion();
        }
    }
}
