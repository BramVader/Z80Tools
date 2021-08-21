using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assembler
{

    internal static class Tokenizer
    {
        private enum Mode
        {
            None,
            Number,
            String,
            Symbol
        }

        private static readonly Dictionary<string, TokenType> operatorMap = new()
        {
            ["NUL"]  = TokenType.Nul,
            ["LOW"]  = TokenType.Low,
            ["HIGH"]  = TokenType.High,
            ["*"]  = TokenType.Multiply,
            ["/"]  = TokenType.Divide,
            ["MOD"]  = TokenType.Mod,
            ["SHR"]  = TokenType.Shr,
            ["SHL"]  = TokenType.Shl,
            ["-"]  = TokenType.Subtract,  // Can also be TokenType.Neg in certain cases
            ["+"]  = TokenType.Add,
            ["EQ"]  = TokenType.Eq,
            ["NE"] = TokenType.Ne,
            ["LT"]  = TokenType.Lt,
            ["LE"]  = TokenType.Le,
            ["GT"]  = TokenType.Gt,
            ["GE"]  = TokenType.Ge,
            ["NOT"]  = TokenType.Not,
            ["AND"]  = TokenType.And,
            ["OR"]  = TokenType.Or,
            ["XOR"]  = TokenType.Xor
        };

        public static IEnumerable<Token> Tokenize(string data, int defaultRadix)
        {
            int i = 0;
            var chars = (data + '\0').ToCharArray();
            var buffer = new char[chars.Length];
            int bufferIndex = 0;
            var mode = Mode.None;
            var radix = defaultRadix;
            var tokens = new List<Token>();
            var stringDelimiter = '\'';
            bool expectingOperand = true;
            while (i < chars.Length)
            {
                char ch = chars[i++];
                switch (mode)
                {
                    case Mode.None:
                        switch (ch)
                        {
                            case char n when (n >= '0' && n <= '9') || (n == '-'):
                                mode = Mode.Number;
                                radix = defaultRadix;
                                bufferIndex = 0;
                                buffer[bufferIndex++] = ch;
                                break;
                            case '\'':
                            case '"':
                                mode = Mode.String;
                                stringDelimiter = ch;
                                bufferIndex = 0;
                                buffer[bufferIndex++] = ch;
                                break;
                            case '(':
                                tokens.Add(new Token { Type = TokenType.OpenParen });
                                expectingOperand = true;
                                break;
                            case ')':
                                tokens.Add(new Token { Type = TokenType.CloseParen });
                                expectingOperand = false;
                                break;
                            case '<':
                                tokens.Add(new Token { Type = TokenType.OpenListParen });
                                expectingOperand = true;
                                break;
                            case '>':
                                tokens.Add(new Token { Type = TokenType.CloseListParen });
                                expectingOperand = false;
                                break;
                            case ',':
                                tokens.Add(new Token { Type = TokenType.Comma });
                                expectingOperand = true;
                                break;
                            case char n2 when
                                (n2 >= 'a' && n2 <= 'z') ||
                                (n2 >= 'A' && n2 <= 'Z') ||
                                n2 == '$' || n2 == '.' || n2 == '?' || n2 == '@':
                                mode = Mode.Symbol;
                                bufferIndex = 0;
                                buffer[bufferIndex++] = ch;
                                break;
                            default:
                                if (operatorMap.TryGetValue(new string(ch, 1), out var it))
                                {
                                    tokens.Add(new Token { Type = it });
                                    expectingOperand = true;
                                }
                                else
                                {
                                    if (!Char.IsWhiteSpace(ch) && ch != '\0')
                                        throw new InvalidOperationException($"Unknown character {ch}");
                                }
                                break;
                        }
                        break;

                    case Mode.Number:
                        bool finish = false;
                        switch (Char.ToUpper(ch))
                        {
                            case char n when Char.IsLetterOrDigit(n):
                                buffer[bufferIndex++] = ch;
                                break;
                            default:
                                i--;
                                finish = true;
                                break;
                        }
                        if (finish)
                        {
                            // Just a minus sign?
                            if (bufferIndex == 1 && buffer[0] == '-')
                            {
                                if (expectingOperand)
                                {
                                    tokens.Add(new Token { Type = TokenType.Neg });
                                }
                                else
                                {
                                    tokens.Add(new Token { Type = TokenType.Subtract });
                                    expectingOperand = true;
                                }
                            }
                            else
                            {
                                string rawValue = new(buffer, 0, bufferIndex);
                                bool trimIt = true;
                                switch (Char.ToUpper(rawValue[^1]))
                                {
                                    case '.': radix = 10; break;
                                    case 'B': if (radix != 16) radix = 2; break;
                                    case 'D': if (radix != 16) radix = 10; break;
                                    case 'H': radix = 16; break;
                                    case 'O': radix = 8; break;
                                    case 'Q': radix = 8; break;
                                    default:
                                        trimIt = false;
                                        break;
                                }
                                if (trimIt) rawValue = rawValue[0..^1];
                                tokens.Add(new Token { Type = TokenType.Number, Value = Convert.ToInt32(rawValue, radix) });
                                expectingOperand = false;
                            }
                            mode = Mode.None;
                        }
                        break;

                    case Mode.String:
                        if (ch == stringDelimiter)
                        {
                            if (data[i] == stringDelimiter)
                            {
                                buffer[bufferIndex++] = ch;
                                i++;
                            }
                            else
                            {
                                tokens.Add(new Token { Type = TokenType.String, Value = new String(buffer, 0, bufferIndex) });
                                mode = Mode.None;
                                i--;
                                expectingOperand = false;
                            }
                        }
                        else
                        {
                            buffer[bufferIndex++] = ch;
                        }
                        break;

                    case Mode.Symbol:
                        bool finish2 = false;
                        switch (Char.ToUpper(ch))
                        {
                            case char n when
                                (n >= 'a' && n <= 'z') ||
                                (n >= 'A' && n <= 'Z') ||
                                (n >= '0' && n <= '9') ||
                                n == '$' || n == '.' || n == '?' || n == '@' || n == ':':
                                buffer[bufferIndex++] = ch;
                                break;
                            default:
                                i--;
                                finish2 = true;
                                break;
                        }
                        if (finish2)
                        {
                            string symbol = new(buffer, 0, bufferIndex);
                            if (symbol == "$")
                                tokens.Add(new Token { Type = TokenType.LocationCounter });
                            else if (operatorMap.TryGetValue(symbol, out var tokenType))
                                tokens.Add(new Token { Type = tokenType });
                            else
                                tokens.Add(new Token { Type = TokenType.Symbol, Value = symbol });
                            mode = Mode.None;
                            expectingOperand = false;
                        }
                        break;
                }
            }
            return tokens;
        }
    }
}
