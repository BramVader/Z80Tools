using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Assembler.Tests
{
    public class TestMacro
    {
        private class TestAssembler : MacroAssembler
        {
            public TestAssembler()
                : base()
            {
            }

            protected override Task Initialize()
            {
                return Task.CompletedTask;
            }

            protected override byte[] ParseOpcode(State state, OutputCollector outputCollector, string label, string opcode, string operands, string comment)
            {
                throw new NotImplementedException();
            }
        }


        [Fact]
        public async Task Assembler_ShouldHandleREPT()
        {
            string source = @"
	X   SET 4
		REPT 10      ;generates DB 5 - DB 15
	X   SET X+1
		DB X
		ENDM
            ";

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(source));
            using var sw = new StringWriter();

            var assembler = new TestAssembler();
            var collector = new OutputCollector(sw);
            await assembler.Assemble(collector, new StreamReader(ms));

            var memoryBytes = collector.Segments.FirstOrDefault()?.Memory;
            Assert.Equal(
                new byte[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, 
                memoryBytes
            );
        }
    }
}
