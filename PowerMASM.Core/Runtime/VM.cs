using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.MASMBase;
using PowerMASM.Core.Interfaces;

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
            else if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString().Trim());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }
        if (sb.Length > 0)
            result.Add(sb.ToString().Trim());
        return result;
    }

    public void LoadProgram(string[]? program = null, ConsoleInWrapper? _in = null, ConsoleOutWrapper? _out = null )
    {
        State.ConsoleIn = _in ?? null;
        State.ConsoleOut = _out ?? null;

        // --- Data label memory layout ---
        if (Program?.Labels != null)
        {
            int dataPtr = 0x1000; // Start data at offset 0x1000 (arbitrary, after code)
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
        // Find _start or main label
        var startLabel = Program?.Labels?.FirstOrDefault(l => l.Name == "_start")
            ?? Program?.Labels?.FirstOrDefault(l => l.Name == "main");
        if (startLabel != null)
            ExecuteInstructions(startLabel.Instructions);
        else if (program != null)
            ExecuteInstructions(program);
    }

    public void ExecuteInstructions(string[] instructions)
    {
        if (State.ConsoleOut != null)
        {
            State.ConsoleOut.WriteLine("Starting execution...");
        }
        int ip = 0;
        while (ip < instructions.Length)
        {
            var line = instructions[ip].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
            {
                ip++;
                continue;
            }
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var opcode = parts[0].ToUpperInvariant();
            var args = parts.Skip(1).ToArray();
            try
            {
                var callable = _collector.GetCallableByName(opcode);
                callable.Call(State, args);
            }
            catch (Exception ex)
            {
                State.Exceptions.Add(new MASMException.MASMException($"Error executing '{line}': {ex.Message}", ip, line, ex));
                if (State.ConsoleOut != null)
                    State.ConsoleOut.WriteLine($"Error executing '{line}': {ex.Message}");
                break;
            }
            ip++;
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
}
