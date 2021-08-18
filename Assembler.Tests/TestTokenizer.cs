using System;
using System.Linq.Expressions;
using Xunit;

namespace Assembler.Tests
{
    public class TestTokenizer
    {
        [Theory]
        [InlineData("15 * -(3+4)", 15 * -(3 + 4))]
        [InlineData("4 + 2 * 3", 4 + 2 * 3)]
        [InlineData("-1 * (12 + (5 - -(4 * 8 + 3) - 2) / 5 - 2)", -1 * (12 + (5 - -(4 * 8 + 3) - 2) / 5 - 2) )]
        public void Tokenizer_ShouldTokenizeSimpleExpression(string exprString, int value)
        {
            var tokens = Tokenizer.Tokenize(exprString, 10);
            var expr = Compiler.Compile(tokens);
            var lambda = Expression.Lambda<Func<int>>(expr);
            Assert.Equal(value, lambda.Compile()());
        }

        [Fact]
        public void Tokenizer_ShouldTokenizeNotSymbolOperators()
        {
            var tokens = Tokenizer.Tokenize("10 NE 15", 10);
        }
    }
}
