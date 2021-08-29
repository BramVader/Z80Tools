namespace Assembler
{
    public enum TokenType
    {
        None,

        Number,
        String,
        Symbol,
        Comma,
        ExternalLabel,
        OpenParen,
        CloseParen,
        OpenListParen,
        CloseListParen,

        Nul,
        Low, High,
        Multiply, Divide, Mod, Shr, Shl,
        Neg,
        Add, Subtract,
        Eq, Ne, Lt, Le, Gt, Ge,
        Not,
        And,
        Or, Xor,

        LocationCounter,
        Macro,
        Rept,
        Irp,
        Irpc,
        Endm,
        Error,
        Label,
        Comment
    }
}
