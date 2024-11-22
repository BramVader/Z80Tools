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
        private int lastLineNumber = 1;

        public OutputCollector(TextWriter listWriter)
        {
            this.listWriter = listWriter;
        }

        private async Task EmitLineNr(int lineNumber)
        {
            if (lineNumber != lastLineNumber)
            {
                await listWriter.WriteAsync($"{lineNumber,5} ");
                lastLineNumber = lineNumber;
            }
            else
            {
                await listWriter.WriteAsync($"{' ',5} ");
            }
        }

        public async Task EmitComment(int lineNr, string comment)
        {
            await EmitLineNr(lineNr);
            await ((String.IsNullOrWhiteSpace(comment))
                ? listWriter.WriteLineAsync()
                : listWriter.WriteLineAsync($"{' ',-13}{comment}"));
        }

        public async Task Emit(int lineNr, string label, int address, byte[] bytes, string opcode, string operands, string comment)
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
            for (int n = 0; n < (bytes?.Length).GetValueOrDefault(); n+=4)
            {
                string byteFmt = bytes != null
                    ? String.Join(" ", bytes.Skip(n).Take(4).Select(it => it.ToString("X2")))
                    : null;
                await EmitLineNr(lineNr);
                if (n == 0)
                {
                    await listWriter.WriteLineAsync(
                        $"{label,-12} {addr,-4:X4}: {byteFmt,-11}  {opcode,-5} {operands,-12}  {comment}"
                    );
                }
                else
                {
                    await listWriter.WriteLineAsync(
                        $"{' ',-12} {addr + n,-4:X4}: {byteFmt,-11}"
                    );
                }
            }
        }

        internal async Task WrapUp(State state)
        {
            await listWriter.WriteLineAsync();
            await listWriter.WriteLineAsync("Symbols");
            await listWriter.WriteLineAsync("-------");
            foreach (var symbol in state.Symbols)
            {
                await listWriter.WriteLineAsync(
                    $"{' ',-12} {ValueToString(symbol.Value), -4}: {symbol.Name,-11}"
                ); ;
            }
            listWriter.Close();
        }

        public static string ValueToString(object val)
        {
            return val switch
            {
                null => "NULL",
                int v => v.ToString("X4"),
                string v => $"\"{v}\"",
                object[] arr => String.Join(", ", arr.Select(it => ValueToString(it))),
                _ => val.ToString()
            };
        }

        public IEnumerable<MemorySegment> Segments => segments;
    }

}
