using System;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
using PowerMASM.Core.MASMFunctions;
namespace PowerMASM.Test
{
    public class Program
    {

        public static ICallableCollector callableCollector { get; set; } = new ICallableCollector();
        public static MicroAsmVmState vmState { get; set; } = new MicroAsmVmState();
        public static void Main(string[] args)
        {
            Console.WriteLine("PowerMASM Test Application");
            Console.WriteLine("This is a placeholder for testing PowerMASM functionalities.");
            callableCollector.Collect();
            if (callableCollector.Callables.Count == 0)
            {
                Console.WriteLine("No callables found.");
                return;
            }
            foreach (var callable in callableCollector.Callables)
            {

                try
                {
                    Console.WriteLine($"Found callable: {callable.Name} with {callable.ParameterCount} parameters.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during callable collection: {ex.Message}");
                }
            }
            TestMoveCallable();
        }
        public static void TestMoveCallable()
        {
            vmState.Reset();

            mov.Call(vmState, "RAX", 5);
            var raxValue = vmState.GetIntRegister("RAX");
            Console.WriteLine($"RAX after mov: {raxValue} (expected 5)");
        }
    }
}