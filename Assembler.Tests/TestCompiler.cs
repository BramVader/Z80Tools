using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Assembler.Tests
{
    public class TestCompiler
    {
        private bool ExpressionsEqual(Expression a, Expression b)
        {
            // Ignore the operands a and b - they are defined differently
            if (a.NodeType == ExpressionType.Parameter || b.NodeType == ExpressionType.Parameter) return true;

            if (a.NodeType != b.NodeType) return false;
            if (a is BinaryExpression bina && b is BinaryExpression binb)
            {
                if (!ExpressionsEqual(bina.Left, binb.Left)) return false;
                if (!ExpressionsEqual(bina.Right, binb.Right)) return false;
            }
            else if (a is UnaryExpression una && b is UnaryExpression unb)
            {
                if (!ExpressionsEqual(una.Operand, unb.Operand)) return false;
            }
            return true;
        }

        private Expression Unwrap(Expression exp)
        {
            return ((exp as NewArrayExpression).Expressions[0] as UnaryExpression).Operand;
        }


        [Theory]
        [InlineData("15 * -(3+4)", 15 * -(3 + 4))]
        [InlineData("4 + 2 * 3", 4 + 2 * 3)]
        [InlineData("-1 * (12 + (5 - -(4 * 8 + 3) - 2) / 5 - 2)", -1 * (12 + (5 - -(4 * 8 + 3) - 2) / 5 - 2) )]
        public void Compiler_ShouldCompileSimpleExpression(string exprString, int value)
        {
            var tokens = Tokenizer.Tokenize(exprString, 10);
            var expr = Unwrap(Compiler.Compile(tokens, null));
            var lambda = Expression.Lambda<Func<int>>(expr);
            Assert.Equal(value, lambda.Compile()());
        }

        private static readonly List<(string, Expression<Func<int, int, bool>>)> condExpr = new()
        {
            ("a LT b", (a, b) => a < b),
            ("NOT a LT b", (a, b) => !(a < b)),
            ("NOT a LT b AND a GE b", (a, b) => !(a < b) && a>=b)
        };

        [Fact]
        public void Compiler_ShouldCompileConditional()
        {
            var state = new State();
            state.SetSymbol("a", 10);
            state.SetSymbol("b", 20);
            var statePar = Expression.Parameter(state.GetType(), "state");
            foreach (var item in condExpr)
            {
                var tokens = Tokenizer.Tokenize(item.Item1, 10);
                var expr = Unwrap(Compiler.Compile(tokens, statePar));
                Assert.True(ExpressionsEqual(item.Item2.Body, expr));

                var lambda1 = item.Item2;
                var fun1 = lambda1.Compile();
                var lambda2 = Expression.Lambda<Func<State, bool>>(expr, statePar);
                var fun2 = lambda2.Compile();
                Assert.Equal(fun1(10, 20), fun2(state));
            }

        }

        private static readonly List<(string, Expression<Func<int, int, object>>)> listExpr = new()
        {
            ("13, 14, a", (a, b) => new object[] { 13, 14, new object[] { a } }),
            ("<13, 14, a>", (a, b) => new object[] { 13, 14, new object[] { a } }),
            ("", (a, b) => null),
            ("<13>", (a, b) => new object[] { 13 }),
            ("<13, 14, <15, 16, a>, 17, <18, <19, <20, 21>>>>", (a, b) => new object[] { 13, 14, new object[] { 15, 16, new object[] {a} }, 17, new object[] { 18, new object[] { 19, new object[] { 20, 21 }}}} ),
            ("<3 * 4 + 2, 3 * (4 + 2)>", (a, b) => new object[] { 3 * 4 + 2, 3 * ( 4 + 2) })
        };


        [Fact]
        public void Compiler_ShouldCompileLists()
        {
            var state = new State();
            state.SetSymbol("a", 10);
            state.SetSymbol("b", 20);
            var statePar = Expression.Parameter(state.GetType(), "state");
            foreach (var item in listExpr)
            {
                var tokens = Tokenizer.Tokenize(item.Item1, 10);
                var expr1 = Compiler.Compile(tokens, statePar);
                var expr2 = item.Item2.Body;
                Assert.True(ExpressionsEqual(expr2, expr1));

                var lambda1 = item.Item2;
                var fun1 = lambda1.Compile();
                var lambda2 = Expression.Lambda<Func<State, object[]>>(expr1, statePar);
                var fun2 = lambda2.Compile();
                var expected = fun1(10, 20);
                var actual = fun2(state);
                Assert.Equal(expected, actual);
            }

        }

    }
}
