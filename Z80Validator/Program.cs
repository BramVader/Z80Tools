using System;
using System.IO;
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

                using var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Z80Validator.zexall.a80"));
                var assembler = new Z80Assembler();
                await assembler.Assemble(sr);
            }
            catch (Exception e)
            {
                Console.Write("Failed with error: ");
                Console.WriteLine(e.Message);
            }
        }
    }
}
