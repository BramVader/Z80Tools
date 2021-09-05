using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Assembler
{
    public class OutputCollector
    {
        private readonly List<MemorySegment> segments = new();
        private readonly TextWriter listWriter;
        private int lineNumber = 1;

        public OutputCollector(TextWriter listWriter)
        {
            this.listWriter = listWriter;
        }

        private Task EmitLineNr() =>
            listWriter.WriteAsync($"{lineNumber++,5} ");

        public async Task EmitComment(string comment)
        {
            await EmitLineNr();
            await ((String.IsNullOrWhiteSpace(comment))
                ? listWriter.WriteLineAsync()
                : listWriter.WriteLineAsync($"{' ',-13}{comment}"));
        }

        public async Task Emit(string label, int address, byte[] bytes, string opcode, string operands, string comment)
        {
            // Add the bytes
            if (bytes != null)
            {
                var segment = segments.FirstOrDefault(it => it.Address + it.Memory.Count == address);
                if (segment == null)
                {
                    segment = new MemorySegment { Address = address, Memory = new List<byte>() };
                    segments.Add(segment);
                }
                segment.Memory.AddRange(bytes);
            }

            int? addr = bytes != null ? address : null;
            string byteFmt = bytes != null ? String.Join(" ", bytes.Select(it => it.ToString("X2"))) : null;
            await EmitLineNr();
            await listWriter.WriteLineAsync(
                $"{label,-12} {addr,-4:X4} {byteFmt,-11}  {opcode,-5} {operands,-12}  {comment}"
            );
        }
    }

}
