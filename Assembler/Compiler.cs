using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Assembler
{
    internal static class Compiler
    {

        // NOTE: According to the M80 documentation, the precedence of the unary minus (TokenType.Neg)
        // should be placed lower than Multiply ... Shl, which is not possible. Unary operators should be
        // evaluated first. So precedence 2 and 3 are swapped
        private static readonly IDictionary<TokenType, int> precedence = new (int prec, TokenType[] tokens)[]
        {
            (0, new [] { TokenType.Nul }),
            (1, new [] { TokenType.Low, TokenType.High }),
            (2, new [] { TokenType.Neg }),
            (3, new [] { TokenType.Multiply, TokenType.Divide, TokenType.Mod, TokenType.Shr, TokenType.Shl }),
            (4, new [] { TokenType.Add, TokenType.Subtract }),
            (5, new [] { TokenType.Eq, TokenType.Ne, TokenType.Lt, TokenType.Le, TokenType.Gt, TokenType.Ge }),
            (6, new [] { TokenType.Not }),
            (7, new [] { TokenType.And }),
            (8, new [] { TokenType.Or, TokenType.Xor }),
            (9, new [] { TokenType.Comma })
        }
        .SelectMany(it => it.tokens, (row, tokenType) => (tokenType, row.prec))
        .ToDictionary(it => it.tokenType, it => it.prec);

        private static readonly HashSet<TokenType> unaryOperators = new()
        {
            TokenType.Nul,
            TokenType.Low,
            TokenType.High,
            TokenType.Neg,
            TokenType.Not
        };

        private class TokenAndExpr
        {
            public Token Token { get; set; }
            public Expression Expression { get; set; }

            public List<Expression> List { get; set; }

            public override string ToString()
            {
                var val = Token.Value == null ? "" : ": " + Token.Value.ToString();
                return $"{Token.Type}{val} - {Expression?.ToString() ?? "<null>"}";
            }
        }

        private static readonly MethodInfo stateGetSymbolMethod = typeof(State).GetMethod(nameof(State.GetSymbol));
        private static readonly MethodInfo stateGetLocationCounterMethod = typeof(State).GetMethod(nameof(State.GetLocationCounter));
        private static readonly MethodInfo nullableIntGetValueDefault = typeof(int?).GetMethod(nameof(Nullable<int>.GetValueOrDefault), Array.Empty<Type>());

        private static Expression GetSymbolExpression(ParameterExpression statePar, TokenAndExpr item)
        {
            if (item.Expression == null)
            {
                if (item.Token.Type != TokenType.Symbol)
                    throw new InvalidOperationException("Symbol expected");
                item.Expression = Expression.Call(statePar, stateGetSymbolMethod, Expression.Constant((string)item.Token.Value));
            }
            return item.Expression;
        }

        private static Expression GetWordExpression(ParameterExpression statePar, TokenAndExpr item)
        {
            if (item.Expression == null)
            {
                item.Expression = item.Token.Type switch
                {
                    TokenType.Symbol => Expression.Call(
                        Expression.Call(statePar, stateGetSymbolMethod, Expression.Constant((string)item.Token.Value)),
                        nullableIntGetValueDefault
                    ),
                    TokenType.Number => Expression.Constant((int)item.Token.Value),
                    TokenType.LocationCounter =>
                        Expression.Call(statePar, stateGetLocationCounterMethod),
                    _ => throw new InvalidOperationException("Integer constant or symbol expected"),

                };
            }
            return item.Expression;
        }

        private static readonly Dictionary<TokenType, Func<Expression, Expression, BinaryExpression>> tokenTypeMapper = new()
        {
            [TokenType.Multiply] = Expression.Multiply,
            [TokenType.Divide] = Expression.Divide,
            [TokenType.Mod] = Expression.Modulo,
            [TokenType.Shr] = Expression.RightShift,
            [TokenType.Shl] = Expression.LeftShift,
            [TokenType.Add] = Expression.Add,
            [TokenType.Subtract] = Expression.Subtract,
            [TokenType.Eq] = Expression.Equal,
            [TokenType.Ne] = Expression.NotEqual,
            [TokenType.Lt] = Expression.LessThan,
            [TokenType.Le] = Expression.LessThanOrEqual,
            [TokenType.Gt] = Expression.GreaterThan,
            [TokenType.Ge] = Expression.GreaterThanOrEqual,
            [TokenType.And] = Expression.And,
            [TokenType.Or] = Expression.Or,
            [TokenType.Xor] = Expression.ExclusiveOr
        };


        public static Expression Compile(IEnumerable<Token> tokens, ParameterExpression statePar = null)
        {
            static string tokenName(TokenType type) => type.ToString().ToUpper();

            statePar ??= Expression.Parameter(typeof(State), "state");
            var list = tokens.Select(it => new TokenAndExpr { Token = it }).ToList();

            // Eliminate brackets
            int level1 = 0;
            int level2 = 0;
            int j = -1;
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                switch (item.Token.Type)
                {
                    case TokenType.OpenParen:
                        if (++level1 == 1)
                            j = i;
                        break;
                    case TokenType.OpenListParen:
                        if (++level2 == 1)
                            j = i;
                        break;
                    case TokenType.CloseParen:
                        if (level1-- == 1)
                        {
                            var sublist = list.Skip(j + 1).Take(i - j - 1).Select(it => it.Token).ToList();
                            var expr = Compile(sublist, statePar);
                            list.RemoveRange(j, i - j);
                            i = j;
                            list[i] = new TokenAndExpr { Token = new Token { Type = TokenType.None }, Expression = expr };
                        }
                        break;
                    case TokenType.CloseListParen:
                        if (level2-- == 1)
                        {
                            var sublist = list.Skip(j + 1).Take(i - j - 1).Select(it => it.Token).ToList();
                            var expr = Compile(sublist, statePar);
                            list.RemoveRange(j, i - j);
                            i = j;
                            list[i] = new TokenAndExpr
                            {
                                Token = new Token { Type = TokenType.None },
                                Expression = expr is ConstantExpression cexpr && cexpr.Value == null
                                    ? Expression.NewArrayBounds(typeof(object), Expression.Constant(0))
                                    : expr
                            };
                        }
                        break;
                }
            }

            for (int prec1 = 0; prec1 < 10; prec1++)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];

                    if (precedence.TryGetValue(item.Token.Type, out int prec2) && prec2 == prec1)
                    {
                        bool isUnary = unaryOperators.Contains(item.Token.Type);

                        if (!isUnary && i == 0)
                            throw new InvalidOperationException($"Operand missing to the left of operator {tokenName(item.Token.Type)}");
                        if (i >= list.Count - 1)
                            throw new InvalidOperationException($"Operand missing to the right of operator {tokenName(item.Token.Type)}");

                        var itemLeft = !isUnary ? list[i - 1] : null;
                        var itemRight = i < list.Count - 1 ? list[i + 1] : null;

                        Expression expr = null;
                        switch (item.Token.Type)
                        {
                            /* Unary expressions */

                            case TokenType.Nul:
                                expr = Expression.Equal(
                                    GetSymbolExpression(statePar, itemRight),
                                    Expression.Constant((int?)null)
                                );
                                break;
                            case TokenType.Low:
                                expr = Expression.And(GetWordExpression(statePar, itemRight), Expression.Constant(0x00FF));
                                break;

                            case TokenType.High:
                                expr = Expression.RightShift(GetWordExpression(statePar, itemRight), Expression.Constant(8));
                                break;
                            case TokenType.Neg:
                                expr = Expression.Negate(GetWordExpression(statePar, itemRight));
                                break;
                            case TokenType.Not:
                                if (itemRight.Expression == null || itemRight.Expression.Type != typeof(bool))
                                    throw new InvalidOperationException("Boolean expressing missing to the right of NOT");
                                expr = Expression.Not(itemRight.Expression);
                                break;

                            /* All other binary expressions */
                            case TokenType.Comma:
                                var list1 = itemLeft.List ?? new List<Expression>() { GetWordExpression(statePar, itemLeft) };
                                var list2 = itemRight.List ?? new List<Expression>() { GetWordExpression(statePar, itemRight) };
                                list1.AddRange(list2);
                                item.List = list1;
                                break;

                            case TokenType.And:
                                if (itemLeft.Expression?.Type == typeof(bool) &&
                                    itemRight.Expression?.Type == typeof(bool))
                                    expr = Expression.AndAlso(itemLeft.Expression, itemRight.Expression);
                                else
                                    expr = Expression.And(GetWordExpression(statePar, itemLeft), GetWordExpression(statePar, itemRight));
                                break;

                            case TokenType.Or:
                                if (itemLeft.Expression?.Type == typeof(bool) &&
                                    itemRight.Expression?.Type == typeof(bool))
                                    expr = Expression.OrElse(itemLeft.Expression, itemRight.Expression);
                                else
                                    expr = Expression.Or(GetWordExpression(statePar, itemLeft), GetWordExpression(statePar, itemRight));
                                break;

                            default:
                                if (!tokenTypeMapper.TryGetValue(item.Token.Type, out var func))
                                    throw new InvalidOperationException($"Unexpected token {tokenName(item.Token.Type)}");

                                expr = func(GetWordExpression(statePar, itemLeft), GetWordExpression(statePar, itemRight));
                                break;
                        }
                        item.Expression = expr;
                        list.RemoveAt(i + 1);
                        if (!isUnary)
                        {
                            list.RemoveAt(i - 1);
                            i--;
                        }
                    }
                }
            }

            if (list.Count > 1)
                throw new InvalidOperationException("Expression not terminated correctly");
            if (list.Count == 0)
                return Expression.Constant(null, typeof(object[]));
            if (list[0].List != null)
                return
                    Expression.NewArrayInit(typeof(object), list[0].List.Select(
                        // Apply boxing where needed
                        it => it.Type != typeof(object) ? Expression.Convert(it, typeof(object)) : it
                    ));
            if (list[0].Expression != null)
                return list[0].Expression;
            return GetWordExpression(statePar, list[0]);
        }

        public static Func<State, T> Compile<T>(string exprString, int radix)
        {
            var tokens = Tokenizer.Tokenize(exprString, radix);
            var statePar = Expression.Parameter(typeof(State), "state");
            var expr = Compile(tokens, statePar);
            var lambda = Expression.Lambda<Func<State, T>>(expr, statePar);
            return lambda.Compile();
        }
    }
}