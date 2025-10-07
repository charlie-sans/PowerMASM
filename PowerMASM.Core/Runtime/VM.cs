using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.MASMBase;
using PowerMASM.Core.Interfaces;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PowerMASM.Core.Runtime;
public class VM
{
    public MicroAsmVmState State { get; private set; }
    public MASMCore? Program { get; set; }
    private ICallableCollector _collector = new ICallableCollector();

    public VM(int memorySize = 65536)
    {
        State = new MicroAsmVmState(memorySize);
        _collector.Collect();
    }

    public VM(int memorySize = 65536, MASMCore? program = null)
        : this(memorySize)
    {
        Program = program;
    }

    public override string ToString() => State.ToString();
    public void Reset() => State = new MicroAsmVmState(State.Memory.Length);

    /// <summary>
    /// Splits a delimited string into a list of values, using commas as separators and ignoring commas that appear
    /// within double quotes.Helper to split data values on commas, ignoring commas inside quotes
    /// </summary>
    /// <remarks>This method does not remove surrounding quotes from values. Empty values between consecutive
    /// commas are returned as empty strings. Useful for parsing CSV-like data where quoted fields may contain
    /// commas.</remarks>
    /// <param name="input">The input string containing comma-separated values. Commas inside double quotes are treated as part of the value
    /// and not as delimiters.</param>
    /// <returns>A list of strings representing the separated values from the input. Each value is trimmed of leading and
    /// trailing whitespace.</returns>
    private static List<string> SplitDataValues(string input)
    {
        // Console.WriteLine($"SplitDataValues input: '{input}'");
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
                sb.Append(c);
            }
            //if (c == ';')
            //{
            //    continue; // Ignore comments
            //}
            // Split on comma or space, but only outside quotes
            else if ((c == ',' || c == ' ') && !inQuotes)
            {
                if (sb.Length > 0)
                {
                    result.Add(sb.ToString().Trim());
                    sb.Clear();
                }
            }
            else
            {
                sb.Append(c);
            }
        }
        if (sb.Length > 0)
            result.Add(sb.ToString().Trim());
        // Console.WriteLine($"SplitDataValues output: [{string.Join(", ", result.Select(x => $"'{x}'"))}]");
        return result;
    }

    public void LoadProgram(string[]? program = null, ConsoleInWrapper? _in = null, ConsoleOutWrapper? _out = null )
    {
        State.ConsoleIn = _in ?? new ConsoleInWrapper();
        State.ConsoleOut = _out ?? new ConsoleOutWrapper();

        // --- Data label memory layout ---
        if (Program?.Labels != null)
        {
            int dataPtr = 0x1000; // Start data at offset 0x1000 (arbitrary, after code) should really make this based on memory size and code size
            foreach (var label in Program.Labels)
            {
                if (!string.IsNullOrEmpty(label.DataDirective) && !string.IsNullOrEmpty(label.DataValues))
                {
                    int labelStartAddr = dataPtr; // Store the starting address before writing data
                    // Parse data values and write to memory
                    var values = SplitDataValues(label.DataValues);
                    if (label.DataDirective == "DB")
                    {
                        foreach (var val in values)
                        {
                            if (val.StartsWith("\"") && val.EndsWith("\""))
                            {
                                // String literal
                                var str = val.Substring(1, val.Length - 2);
                                var bytes = Encoding.UTF8.GetBytes(str);
                                bytes.CopyTo(State.Memory.Span.Slice(dataPtr));
                                dataPtr += bytes.Length;
                            }
                            else if (byte.TryParse(val, out var b))
                            {
                                State.Memory.Span[dataPtr++] = b;
                            }
                            else if (int.TryParse(val, out var ib))
                            {
                                State.Memory.Span[dataPtr++] = (byte)ib;
                            }
                        }
                    }
                    else if (label.DataDirective == "DW")
                    {
                        foreach (var val in values)
                        {
                            if (short.TryParse(val, out var w))
                            {
                                BitConverter.GetBytes(w).CopyTo(State.Memory.Slice(dataPtr, 2));
                                dataPtr += 2;
                            }
                        }
                    }
                    else if (label.DataDirective == "DD")
                    {
                        foreach (var val in values)
                        {
                            if (int.TryParse(val, out var d))
                            {
                                BitConverter.GetBytes(d).CopyTo(State.Memory.Slice(dataPtr, 4));
                                dataPtr += 4;
                            }
                        }
                    }
                    else if (label.DataDirective == "DQ")
                    {
                        foreach (var val in values)
                        {
                            if (long.TryParse(val, out var q))
                            {
                                BitConverter.GetBytes(q).CopyTo(State.Memory.Slice(dataPtr, 8));
                                dataPtr += 8;
                            }
                        }
                    }
                    else if (label.DataDirective == "RESB")
                    {
                        if (int.TryParse(values[0], out var count))
                        {
                            dataPtr += count;
                        }
                    }
                    // Store label address in a register map or label map for later reference
                    if (State.Labels != null)
                    {
                        var labelCopy = new MASMLabel {
                            Name = label.Name,
                            DataDirective = label.DataDirective,
                            DataValues = labelStartAddr.ToString(), // Use the starting address
                            Instructions = label.Instructions,
                            modifiers = label.modifiers
                        };
                        State.Labels.Add(labelCopy);
                    }
                }
                else
                {
                    // Add code labels as well
                    if (State.Labels != null)
                        State.Labels.Add(label);
                }
            }
        }
        // Set instruction pointer (RIP) to start of program
        State.SetIntRegister("RIP", 0);
        // Execute all instructions in the MASM file if available
        if (Program?.AllInstructions != null)
        {
            Debug.WriteLine($"Calling ExecuteInstructions with AllInstructions (length: {Program.AllInstructions.Length})");
            for (int i = 0; i < Math.Min(5, Program.AllInstructions.Length); i++)
                Debug.WriteLine($"Instruction[{i}]: {Program.AllInstructions[i]}");
            ExecuteInstructions(Program.AllInstructions);
        }
        else if (program != null)
        {
            Debug.WriteLine($"Calling ExecuteInstructions with program (length: {program.Length})");
            for (int i = 0; i < Math.Min(5, program.Length); i++)
                Debug.WriteLine($"Instruction[{i}]: {program[i]}");
            ExecuteInstructions(program);
        }
    }

    public void ExecuteInstructions(string[] instructions)
    {
        if (State.ConsoleOut != null)
        {
            State.ConsoleOut.WriteLine("Starting execution...");
        }
        // we gotta set RBP to the top of the memory for stack operations
        State.SetIntRegister("RBP", State.Memory.Length -16);
        // Set RSP to the top of the memory (stack grows downward)
        State.SetIntRegister("RSP", State.Memory.Length -16);
        // Debug: print total instructions
        Debug.WriteLine($"Total instructions: {instructions.Length}");

        // Build label-to-index map before execution
        State.LabelIndices.Clear();
        try
        {
            for (int i = 0; i < instructions.Length; i++)
            {
                var line = instructions[i].Trim();
                Debug.WriteLine($"Processing line {i}: '{line}'");
                if (line.StartsWith("LBL ") || line.StartsWith("lbl "))
                {
                    var labelName = line.Substring(4).Trim();
                    Debug.WriteLine($"Found label '{labelName}' at instruction index {i}");
                    // Map label to the next instruction index
                    State.LabelIndices[labelName] = i + 1;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception in label-finding loop: {ex.Message}");
        }

        while (State.GetIntRegister("RIP") < instructions.Length)
        {
            var rip = (int)State.GetIntRegister("RIP");
            var line = instructions[rip].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
            {
                State.SetIntRegister("RIP", rip + 1);
                continue;
            }
            if (line.StartsWith("LBL "))
            {
                // Skip label lines during execution
                State.SetIntRegister("RIP", rip + 1);
                continue;
            }
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            // foreach (var part in parts) 
            //     Console.WriteLine($"Part: '{part}'");
            var opcode = parts[0].ToUpperInvariant();
            var args = SplitDataValues(line.Substring(opcode.Length).Trim()).ToArray();
            try
            {
                var callable = _collector.GetCallableByName(opcode);
                var originalRip = State.GetIntRegister("RIP");
                callable.Call(State, args);
                var newRip = State.GetIntRegister("RIP");
                if (newRip == originalRip)
                {
                    State.SetIntRegister("RIP", rip + 1);
                }
                else
                {
                    continue;
                }
            }
            catch (Exception ex)
            {
                State.Exceptions.Add(new MASMException.MASMException($"Error executing '{line}': {ex.Message}", rip, line, ex));
                if (State.ConsoleOut != null)
                    State.ConsoleOut.WriteLine($"Error executing '{line}': {ex.Message}");
                break;
            }
        }
    }


    public object? InvokeFunction(string name, params object[] args)
    {
        if (Program == null)
            throw new InvalidOperationException("No program loaded.");
        var func = Program.GetFunction(name);
        if (func == null)
            throw new Exception($"Function '{name}' not found.");
        // Map arguments to registers (RDI, RSI, RDX, RCX, R8, R9)
        var argRegs = new[] { "RDI", "RSI", "RDX", "RCX", "R8", "R9" };
        for (int i = 0; i < func.Parameters.Count && i < argRegs.Length; i++)
        {
            State.SetIntRegister(argRegs[i], Convert.ToInt64(args[i]));
        }
        ExecuteInstructions(func.Instructions);
        return State.GetIntRegister("RAX");
    }

    /// <summary>
    /// Resolves an operand string to its value from registers or memory.
    /// Supports: register names (RAX), direct memory (1234), indirect ($RAX), offset ([RBP+4]), computed ($[RBP+4])
    /// </summary>
    public static long ResolveOperandValue(MicroAsmVmState state, string operand)
    {
        operand = operand.Trim();
        // Register (e.g., RAX)
        if (state._intRegisterMap.ContainsKey(operand))
            return state.GetIntRegister(operand);
        // Indirect register (e.g., $RAX)
        if (operand.StartsWith("$") && state._intRegisterMap.ContainsKey(operand.Substring(1)))
        {
            var addr = state.GetIntRegister(operand.Substring(1));
            return BitConverter.ToInt64(state.Memory.Span.Slice((int)addr, 8));
        }
        // Offset (e.g., [RBP+4])
        var offsetMatch = Regex.Match(operand, @"\[(\w+)([+-]\d+)?\]");
        if (offsetMatch.Success)
        {
            var reg = offsetMatch.Groups[1].Value;
            var offset = offsetMatch.Groups[2].Success ? int.Parse(offsetMatch.Groups[2].Value) : 0;
            var baseAddr = state.GetIntRegister(reg);
            var addr = baseAddr + offset;
            if (addr < 0 || addr + 8 > state.Memory.Length)
                throw new IndexOutOfRangeException($"Memory access out of bounds at address {addr}");
            return BitConverter.ToInt64(state.Memory.Span.Slice((int)addr, 8));
        }
        // Computed memory address (e.g., $[RBP+4])
        var computedMatch = Regex.Match(operand, @"\$\[(\w+)([+-]\d+)?\]");
        if (computedMatch.Success)
        {
            var reg = computedMatch.Groups[1].Value;
            var offset = computedMatch.Groups[2].Success ? int.Parse(computedMatch.Groups[2].Value) : 0;
            var baseAddr = state.GetIntRegister(reg);
            var addr = baseAddr + offset;
            return BitConverter.ToInt64(state.Memory.Span.Slice((int)addr, 8));
        }
        // Direct memory address with $ (e.g., $1234)
        if (operand.StartsWith("$") && int.TryParse(operand.Substring(1), out var addrs))
        {
            return BitConverter.ToInt64(state.Memory.Span.Slice(addrs, 8));
        }
        // Number literal
        if (long.TryParse(operand, out var num))
            return num;
        throw new ArgumentException($"Unknown operand format: {operand}");
    }
}
