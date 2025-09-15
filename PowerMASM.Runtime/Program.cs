using System;
using System.IO;
using PowerMASM.Core.MASMBase;
using PowerMASM.Core.Runtime;

namespace PowerMASM.Runtime
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("PowerMASM Runtime Environment");

            string filePath = args.Length > 0 ? args[0] : "program.masm";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            string code = File.ReadAllText(filePath);
            var masmCore = MASMCore.PreProcess(code);

            var vm = new VM(32768);
            vm.Program = masmCore;
            vm.LoadProgram(null);

            if (vm.State.Exceptions.Count > 0)
            {
                Console.WriteLine("Errors during execution:");
                foreach (var ex in vm.State.Exceptions)
                    Console.WriteLine(ex);
            }
            else
            {
                Console.WriteLine("Program executed successfully.");
                Console.WriteLine(vm.State);
            }
        }
    }
}