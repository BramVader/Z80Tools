using System;
using System.Collections.Generic;
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

        private static readonly Dictionary<TokenType, Func<Expression, Expression, BinaryExpression>> binaryExprMapper = new()
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

        private static readonly MethodInfo stateGetSymbolMethod = typeof(State).GetMethod(nameof(State.GetSymbol));
        private static readonly MethodInfo stateGetLocationCounterMethod = typeof(State).GetMethod(nameof(State.GetLocationCounter));

        private static string TokenName(TokenType type) => type.ToString().ToUpper();

        public static int ExpectNumber(object data, bool allowNull = false)
        {
            if (data == null)
                if (!allowNull)
                    throw new InvalidOperationException($"Number expression evaluates to null");
                else
                    return 0;

            switch (data)
            {
                case int v:
                    return v;
                case string s:
                    return s.Length switch
                    {
                        0 => 0,
                        1 => s[0],
                        2 => s[0] | (s[1] << 8),
                        _ => throw new InvalidOperationException("To be able to interpret a string as a number, it must be 0, 1 or 2 chars long"),
                    };
                    ;
                case object[] arr:
                    if (arr.Length == 0 && allowNull)
                        return 0;
                    if (arr.Length != 1)
                        throw new InvalidOperationException("To be able to interpret an array as a number, it contain exactly one element");
                    return ExpectNumber(arr[0], allowNull);
                default:
                    throw new InvalidOperationException($"Unable to convert a {data.GetType().Name} to a number");
            }
        }

        public static bool ExpectBool(object data, bool allowNull = false)
        {
            if (data == null)
                if (!allowNull)
                    throw new InvalidOperationException($"Boolean expression evaluates to null");
                else
                    return false;

            switch (data)
            {
                case bool b:
                    return b;
                case int v:
                    return v != 0;
                case string s:
                    int w = s.Length switch
                    {
                        0 => 0,
                        1 => s[0],
                        2 => s[0] | (s[1] << 8),
                        _ => throw new InvalidOperationException("To be able to interpret a string as a boolean, it must be 0, 1 or 2 chars long"),
                    };
                    return w != 0;
                case object[] arr:
                    if (arr.Length == 0 && allowNull)
                        return false;
                    if (arr.Length != 1)
                        throw new InvalidOperationException("To be able to interpret an array as a boolean, it contain exactly one element");
                    return ExpectBool(arr[0], allowNull);
                default:
                    throw new InvalidOperationException($"Unable to convert a {data.GetType().Name} to a boolean");
            }
        }

        private static readonly Expression<Action> expectNumberMethodFinder = () => ExpectNumber((object)null, false);
        private static readonly Expression<Action> expectBoolMethodFinder = () => ExpectBool((object)null, false);
        private static readonly MethodInfo expectNumberMethod = (expectNumberMethodFinder.Body as MethodCallExpression).Method;
        private static readonly MethodInfo expectBoolMethod = (expectBoolMethodFinder.Body as MethodCallExpression).Method;

        private static Expression ExpectNumber(Expression expr, ParameterExpression statePar)
        {
            if (expr.Type == typeof(int)) return expr;
            return Expression.Call(null, expectNumberMethod, expr.Type == typeof(object) ? expr : Expression.Convert(expr, typeof(object)), Expression.Equal(Expression.Property(statePar, nameof(State.Pass)), Expression.Constant(0)));
        }

        private static Expression ExpectBool(Expression expr, ParameterExpression statePar)
        {
            if (expr.Type == typeof(bool)) return expr;
            return Expression.Call(null, expectBoolMethod, expr.Type == typeof(object) ? expr : Expression.Convert(expr, typeof(object)), Expression.Equal(Expression.Property(statePar, nameof(State.Pass)), Expression.Constant(0)));
        }

        private static Expression HandleUnary(Token token, Expression o, ParameterExpression statePar)
        {
            switch (token.Type)
            {
                case TokenType.Nul:
                    return Expression.Equal(o, Expression.Constant((int?)null));
                case TokenType.Low:
                    return Expression.And(ExpectNumber(o, statePar), Expression.Constant(0x00FF));
                case TokenType.High:
                    return Expression.RightShift(ExpectNumber(o, statePar), Expression.Constant(8));
                case TokenType.Neg:
                    return Expression.Negate(ExpectNumber(o, statePar));
                case TokenType.Not:
                    if (o == null || o.Type != typeof(bool))
                        throw new InvalidOperationException("Boolean expression missing to the right of NOT");
                    return Expression.Not(o);
                default:
                    throw new InvalidOperationException($"Unknown unary expression: {token.Type}");
            }
        }

        private static Expression HandleBinary(Token token, Expression b, Expression a, ParameterExpression statePar)
        {
            switch (token.Type)
            {
                case TokenType.And:
                    if (a.Type == typeof(bool) &&
                        b.Type == typeof(bool))
                        return Expression.AndAlso(ExpectBool(a, statePar), ExpectBool(b, statePar));
                    else
                        return Expression.And(ExpectNumber(a, statePar), ExpectNumber(b, statePar));

                case TokenType.Or:
                    if (a.Type == typeof(bool) &&
                        b.Type == typeof(bool))
                        return Expression.OrElse(ExpectBool(a, statePar), ExpectBool(b, statePar));
                    else
                        return Expression.Or(ExpectNumber(a, statePar), ExpectNumber(b, statePar));

                default:
                    if (!binaryExprMapper.TryGetValue(token.Type, out var func))
                        throw new InvalidOperationException($"Unexpected token {TokenName(token.Type)}");
                    if (a.Type == typeof(bool) &&
                        b.Type == typeof(bool))
                        return func(ExpectBool(a, statePar), ExpectBool(b, statePar));
                    else
                        return func(ExpectNumber(a, statePar), ExpectNumber(b, statePar));
            }
        }

        private static Expression HandleList(Expression b, Expression a)
        {
            List<Expression> list;
            if (a is NewArrayExpression arrx)
            {
                list = arrx.Expressions.ToList();
            }
            else
            {
                list = new List<Expression>();
                if (a.Type != typeof(object))
                    a = Expression.Convert(a, typeof(object));
                list.Add(a);
            }
            if (b.Type != typeof(object))
                b = Expression.Convert(b, typeof(object));
            list.Add(b);
            return Expression.NewArrayInit(typeof(object), list);
        }

        private static Expression HandleSingleValueList(Expression o)
        {
            if (!(o is NewArrayExpression))
            {
                if (o.Type != typeof(object))
                    o = Expression.Convert(o, typeof(object));
                o = Expression.NewArrayInit(typeof(object), o);
            }
            return o;
        }

        private static void HandleTopStackOperator(Stack<Expression> valueStack, Stack<Token> operatorStack, ParameterExpression statePar)
        {
            var topToken = operatorStack.Pop();
            switch (topToken.Type)
            {
                case TokenType.OpenListParen:
                    valueStack.Push(HandleSingleValueList(
                        valueStack.Pop()
                    ));
                    break;

                case TokenType.Comma:
                    valueStack.Push(HandleList(
                        valueStack.Pop(),
                        valueStack.Pop()
                    ));
                    break;

                default:
                    if (unaryOperators.Contains(topToken.Type))
                    {
                        valueStack.Push(HandleUnary(
                            topToken,
                            valueStack.Pop(),
                            statePar
                        ));
                    }
                    else
                    {
                        valueStack.Push(HandleBinary(
                            topToken,
                            valueStack.Pop(),
                            valueStack.Pop(),
                            statePar
                        ));
                    }
                    break;
            }
        }

        // Closes a possible list by wrapping it into an object (so it will not grow anymore
        // and possibly become an element of a parent list).
        private static void MaybeCloseList(Stack<Expression> valueStack, Stack<Token> operatorStack)
        {
            if (operatorStack.TryPeek(out var topToken) && topToken.Type == TokenType.OpenListParen &&
                valueStack.TryPeek(out Expression topValue) && topValue is NewArrayExpression)
            {
                valueStack.Pop();
                valueStack.Push(Expression.Convert(topValue, typeof(object)));
            }
        }

        // Using the Shunting Yard algorithm using two stacks
        public static Expression Compile(IEnumerable<Token> tokens, ParameterExpression statePar)
        {
            var valueStack = new Stack<Expression>();
            var operatorStack = new Stack<Token>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        valueStack.Push(Expression.Constant((int)token.Value));
                        break;
                    case TokenType.String:
                        valueStack.Push(Expression.Constant((string)token.Value));
                        break;
                    case TokenType.Symbol:
                        valueStack.Push(Expression.Call(statePar, stateGetSymbolMethod, Expression.Constant((string)token.Value)));
                        break;
                    case TokenType.LocationCounter:
                        valueStack.Push(Expression.Call(statePar, stateGetLocationCounterMethod));
                        break;
                    case TokenType.OpenParen:
                    case TokenType.OpenListParen:
                        operatorStack.Push(token);
                        break;
                    case TokenType.CloseParen:
                    case TokenType.CloseListParen:
                        while (
                            operatorStack.Peek().Type != TokenType.OpenParen &&
                            operatorStack.Peek().Type != TokenType.OpenListParen
                        )
                        {
                            HandleTopStackOperator(valueStack, operatorStack, statePar);
                        }
                        MaybeCloseList(valueStack, operatorStack);
                        operatorStack.Pop(); // Pop and discard the opening paren
                        break;
                    default:    // operators
                        // If the token has precedence specified, we know its an operator
                        if (precedence.TryGetValue(token.Type, out int prec))
                        {
                            while (operatorStack.TryPeek(out Token topToken) &&
                                topToken.Type != TokenType.OpenParen &&
                                topToken.Type != TokenType.OpenListParen &&
                                (!precedence.TryGetValue(topToken.Type, out int topPrec) ||
                                topPrec <= prec))
                            {
                                HandleTopStackOperator(valueStack, operatorStack, statePar);
                            }
                            operatorStack.Push(token);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unexpected operator {TokenName(token.Type)}");
                        }
                        break;

                }
            }
            while (operatorStack.Count > 0)
            {
                HandleTopStackOperator(valueStack, operatorStack, statePar);
            }

            // Check the value stack
            // In the end it should be reduced to just one element and the operator stack should be empty
            if (valueStack.Count > 1)
                throw new InvalidOperationException("Expression not terminated correctly");
            if (valueStack.Count == 0)
                return Expression.Constant(null, typeof(object[]));

            var expr2 = valueStack.Pop();

            // Try to return the final element as an object[] array
            if (expr2 is ConstantExpression cexpr2 && cexpr2.Value == null)
                return Expression.NewArrayBounds(typeof(object), Expression.Constant(null));
            if (expr2.Type == typeof(object[]))
                return expr2;
            if (expr2 is UnaryExpression conv && conv.Operand is NewArrayExpression expr3)
                return expr3;
            return
                Expression.NewArrayInit(typeof(object), expr2.Type != typeof(object) ? Expression.Convert(expr2, typeof(object)) : expr2);
        }

        public static Func<State, object[]> Compile(string exprString, int radix)
        {
            var tokens = Tokenizer.Tokenize(exprString, radix);
            var statePar = Expression.Parameter(typeof(State), "state");
            var expr = Compile(tokens, statePar);
            var lambda = Expression.Lambda<Func<State, object[]>>(expr, statePar);
            return lambda.Compile();
        }

        public static object[] Get(string exprString, State state)
        {
            var func = Compile(exprString, state.Radix);
            return func(state);
        }

        public static bool? GetBool(string exprString, State state)
        {
            var func = Compile(exprString, state.Radix);
            return Compiler.ExpectBool(func(state));
        }

        public static int? GetInt(string exprString, State state)
        {
            var func = Compile(exprString, state.Radix);
            return Compiler.ExpectNumber(func(state));
        }
    }
}