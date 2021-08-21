using System.Collections.Generic;

namespace Assembler
{
    public class Macro
    {
        public string Name { get; set; }
        public List<string> ArgNames { get; set; }

        private readonly List<string> lines = new();

        public void AddLine(string line)
        {
            lines.Add(line);
        }

        public IEnumerable<string> Lines => lines;

        public TokenType MacroType { get; set; }
    }
}
