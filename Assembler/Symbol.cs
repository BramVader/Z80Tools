namespace Assembler
{

    public class Symbol
    {
        public object Value { get; set; }

        public bool IsPublic { get; set; }    // When declared with double colon (::)

        public string Name { get; set; }

        public bool Readonly { get; set; }

        public SymbolType Type { get; set; }
    }
}
