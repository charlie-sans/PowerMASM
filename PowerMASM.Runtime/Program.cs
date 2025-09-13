using PowerMASM.Core.MASMBase;
namespace PowerMASM.Runtime;
public class Program {
    public static void Main(string[] args) {
        Console.WriteLine("PowerMASM Runtime Environment");


        MASMLabel label = new MASMLabel()
        {
            Name = "example",
            Instructions = new[] { "mov rax 1", "add rax 2" },
            modifiers = new MASMAcessorModifiers.ObjectiveModifiers
            {
                IsPointer = false,
                IsReference = false,
                IsArray = false,
                ArraySize = 0,
                IsFunction = true,
                ReturnType = "int",
                Parameters = new List<string> { "int a", "int b" },
                Modifiers = new List<MASMAcessorModifiers.AccessorModifier>
                {
                    MASMAcessorModifiers.AccessorModifier.Public,
                    MASMAcessorModifiers.AccessorModifier.Static
                }
            }
        };


        Console.WriteLine($"Label Name: {label.Name}");
        Console.WriteLine($"Instructions: {string.Join(", ", label.Instructions)}");
        // serialise to JSON
        var serialized = System.Text.Json.JsonSerializer.Serialize(label);
        Console.WriteLine(serialized);
    }
}