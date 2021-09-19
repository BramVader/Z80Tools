using System;
using System.Linq.Expressions;
using Xunit;

namespace Assembler.Tests
{
    public class TestCompiler
    {
        private bool ExpressionsEqual(Expression a, Expression b)
        {
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

        // Checks if exprString compiles to the same expression as checkExpr
        private void CheckExpression(string exprString, Expression<Func<int, int, object>> checkExpr)
        {
            CheckExpression(exprString, Expression.Lambda<Func<int, int, object[]>>
                (
                    Expression.NewArrayInit(typeof(object), checkExpr.Body),
                    checkExpr.Parameters
                )
            );
        }

        // Checks if exprString compiles to the same expression as checkExpr
        private void CheckExpression(string exprString, Expression<Func<int, int, object[]>> checkExpr)
        {
            var state = new State();
            state.SetSymbol("a", 10);
            state.SetSymbol("b", 20);
            var statePar = Expression.Parameter(state.GetType(), "state");
            var tokens = Tokenizer.Tokenize(exprString, 10);
            var exprActual = Compiler.Compile(tokens, statePar);
            var exprExpected = new ReplaceParVisitor(statePar).Visit(checkExpr.Body);
            Assert.True(ExpressionsEqual(exprExpected, exprActual));

            var lambdaActual = Expression.Lambda<Func<State, object[]>>(exprActual, statePar);
            var funcActual = lambdaActual.Compile();
            var lambdaExpected = Expression.Lambda<Func<State, object[]>>(exprExpected, statePar);
            var funcExpected = lambdaExpected.Compile();
            object actual = funcActual(state);
            object expected = funcExpected(state);
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Compiler_ShouldCompileSimpleExpression()
        {
            CheckExpression("15 * -(3+4)", (a, b) => 15 * -(3 + 4));
            CheckExpression("4 + 2 * 3", (a, b) => 4 + 2 * 3);
            CheckExpression("-1 * (12 + (5 - -(4 * 8 + 3) - 2) / 5 - 2)", (a, b) => -1 * (12 + (5 - -(4 * 8 + 3) - 2) / 5 - 2));
        }

        [Fact]
        public void Compiler_ShouldCompileStringExpression()
        {
            CheckExpression("'' + 1", (a, b) => 1);
            CheckExpression("'A' + 1", (a, b) => 66);
            CheckExpression("'AA' + 1", (a, b) => 16706);
            CheckExpression("'AA'", (a, b) => "AA");
            CheckExpression("'AAA'", (a, b) => "AAA");

            Assert.Throws<InvalidOperationException>(() =>
                CheckExpression("'AAA' + 1", (a, b) => 16706)
            );
        }

        [Fact]
        public void Compiler_ShouldCompileConditional()
        {
            CheckExpression("a LT b", (a, b) => a < b);
            CheckExpression("NOT a LT b", (a, b) => !(a < b));
            CheckExpression("NOT a LT b AND a GE b", (a, b) => !(a < b) && a >= b);
        }

        [Fact]
        public void Compiler_ShouldCompileLists()
        {
            CheckExpression("13, 14, a", (a, b) => new object[] { 13, 14, a });
            CheckExpression("<13, 14, a>", (a, b) => new object[] { 13, 14, a });
            CheckExpression("", (a, b) => null);
            CheckExpression("<13>", (a, b) => new object[] { 13 });
            CheckExpression("<13, 14, <15, 16, a>, 17, <18, <19, <20, 21>>>>", (a, b) => new object[] { 13, 14, new object[] { 15, 16, a }, 17, new object[] { 18, new object[] { 19, new object[] { 20, 21 } } } });
            CheckExpression("<3 * 4 + 2, 3 * (4 + 2)>", (a, b) => new object[] { 3 * 4 + 2, 3 * (4 + 2) });
        }

    }
}
