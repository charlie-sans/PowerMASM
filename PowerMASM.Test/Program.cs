using System;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
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
            ICallable moveCallable;
            try
            {
                moveCallable = callableCollector.GetCallableByName("mov");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Move callable: {ex.Message}");
                return;
            }
            Console.WriteLine("Testing Move Callable...");
            if (moveCallable == null)
            {
                Console.WriteLine("Move callable not found.");
                return;
            }
            Console.WriteLine($"Testing Move callable: {moveCallable.Name} with {moveCallable.ParameterCount} parameters.");
            vmState.Reset();
            moveCallable.Call(vmState, "EAX", 5);
        }
    }
}