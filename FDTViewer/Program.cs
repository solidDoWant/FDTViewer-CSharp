using System;
using System.IO;

namespace FDTTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine($"Usage: {System.Diagnostics.Process.GetCurrentProcess().ProcessName} <filename.dtb>");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File not found");
                return;
            }

            Console.WriteLine(libfdt.FDT.LoadFDT(File.ReadAllBytes(@".\ffaero.dtb")));
            Console.ReadKey();
        }
    }
}