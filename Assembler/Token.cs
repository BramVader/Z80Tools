namespace Assembler
{
    internal class Token
    {
        public TokenType Type { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            var val = Value == null ? "" : ": " + Value.ToString();
            return $"{Type}{val}";
        }
    }
}
