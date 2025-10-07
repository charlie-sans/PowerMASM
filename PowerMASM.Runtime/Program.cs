using System;
using System.IO;
using System.Reflection;
using PowerMASM.Core;
using PowerMASM.Core.MASMBase;
using PowerMASM.Core.Runtime;

namespace PowerMASM.Runtime
{
    public class Program
    {
        public static ConsoleInWrapper _in = new ConsoleInWrapper();
        public static ConsoleOutWrapper _out = new ConsoleOutWrapper();
        public static void Main(string[] args)
        {
            Console.WriteLine("=PowerMASM Runtime Environment=");
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: PowerMASM.Runtime <path to .masm / .masi file>");
                return;
            }
            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }
            //Console.WriteLine(filePath);
            string code = File.ReadAllText(filePath);
            var masmCore = MASMCore.PreProcess(code);

            var location = Assembly.GetExecutingAssembly().Location;
            var conf = Path.Combine(Path.GetDirectoryName(location), "Settings.json");
            if (File.Exists(conf))
            {
                var config = Config.Load(conf);
                masmCore.Config = config;
            }
            else
            {
                Console.WriteLine("Config file not found, using default settings.");
            }

            var vm = new VM(masmCore.Config.memSize);
            vm.Program = masmCore;
      
            vm.LoadProgram(null,_in,_out);
            //Console.WriteLine("Initial VM State:");
            //Console.WriteLine(vm.State);
            if (vm.State.Exceptions.Count > 0)
            {
                Console.WriteLine("Errors during execution:");
                foreach (var ex in vm.State.Exceptions)
                    Console.WriteLine(ex);
                Console.WriteLine(vm.State);
            }
            else
            {
     
                Console.WriteLine("Program executed successfully.");
                Console.WriteLine("output of [termianl]");
                Console.WriteLine(_out.ToString());
            }
        }
    }
}