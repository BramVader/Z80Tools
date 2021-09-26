using Assembler;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Z80Core;

namespace Z80Validator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var ass = Assembly.GetExecutingAssembly();
                var streamName = ass.GetManifestResourceNames().First(it => it.Contains("zexall.a80"));
                //var streamName = ass.GetManifestResourceNames().First(it => it.Contains("Inc8bit.a80"));
                using var sr = new StreamReader(ass.GetManifestResourceStream(streamName));
                using var sw = new StreamWriter(new FileStream("output.lst", FileMode.Create, FileAccess.Write, FileShare.Read));
                var assembler = new Z80Assembler();
                var outputCollector = new OutputCollector(sw);
                await assembler.Assemble(outputCollector, sr);

                using var fs = new FileStream("output.bin", FileMode.Create, FileAccess.Write, FileShare.Read);
                var mem = outputCollector.Segments.First().Memory.ToArray();
                fs.Write(mem, 0, mem.Length);
            }
            catch (Exception e)
            {
                Console.Write("Failed with error: ");
                Console.WriteLine(e.Message);
            }
        }
    }
}
