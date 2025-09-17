using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class Out : ICallable {
    public string Name => "OUT";

    public int ParameterCount => 2;

    public void Call(MicroAsmVmState state, params object[] parameters) {
        var IOOutput = int.Parse((string)parameters[0]);
        object param = parameters[1];
        string content = null;

        if (param is string s) {
            // Try integer register
            if (state._intRegisterMap != null && state._intRegisterMap.TryGetValue(s.ToUpper(), out var regIdx)) {
                content = state._intRegisters[regIdx].ToString();
            }
            // Try float register
            else if (state._floatRegisterMap != null && state._floatRegisterMap.TryGetValue(s.ToUpper(), out var fregIdx)) {
                content = state._floatRegisters[fregIdx].ToString();
            }
            // Try label (data label: print as string if DB, otherwise as bytes/values)
            else if (state.Labels != null) {
                var label = state.Labels.FirstOrDefault(l => l.Name == s);
                if (label != null && !string.IsNullOrEmpty(label.DataDirective)) {
                    // Only handle DB as string for now
                    if (label.DataDirective == "DB") {
                        if (int.TryParse(label.DataValues, out int addr)) {
                            var mem = state.Memory.Span;
                            int end = addr;
                            while (end < mem.Length && mem[end] != 0) end++;
                            // check if contents contain any \n, \t or \r and replace them with actual characters
                            content = Encoding.UTF8.GetString(mem.Slice(addr, end - addr)).Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\r", "\r");
                        }
                        else {
                            content = $"[DB {label.DataValues}]";
                        }
                    } else {
                        content = $"[{label.DataDirective} {label.DataValues}]";
                    }
                } else if (label != null && label.Instructions != null && label.Instructions.Length > 0) {
                    content = string.Join("\n", label.Instructions);
                }
            }
            // Fallback: treat as string literal
            if (content == null)
                content = s;
        }
        else if (param is int i) {
            content = i.ToString();
        }
        else {
            content = param?.ToString() ?? string.Empty;
        }
        //Console.WriteLine($"[Debug] out called with IOOutput={IOOutput}, content='{content}'");
        if (IOOutput == 0 && state.Memory.Slice(1020, 10).ToArray()[3] == 1)
        {
            Console.WriteLine(content);
        }
        else if (IOOutput == 1 && state.Memory.Slice(1020, 10).ToArray()[4] == 1)
        {
            Console.Error.WriteLine(content);
        }
        else if (IOOutput == 0)
        {
            Console.Write(content);
        }
        else if (IOOutput == 1)
        {
            Console.Error.Write(content);
        }
        else
        {
            throw new ArgumentException("Invalid IOOutput parameter for out");
        }
    }
}
