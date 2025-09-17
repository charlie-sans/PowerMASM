using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
using PowerMASM.Core.MASMExtentions;

namespace PowerMASM.Core.MASMFunctions;

public class mov : ICallable
{
    public string Name => "MOV";
    public int ParameterCount => 2;

    public void Call(MicroAsmVmState state, params object[] parameters)
    {
        if (parameters.Length != 2)
            throw new ArgumentException("MOV requires 2 parameters");

        var dest = parameters[0].ToString();
        var src = parameters[1].ToString();

        // Try register first, then immediate
        long value;
        if (state._intRegisterMap.TryGetValue(src, out var srcIdx))
            value = state._intRegisters[srcIdx];
        else if (long.TryParse(src, out var imm))
            value = imm;
        else
            throw new ArgumentException($"Unknown MOV source: {src}");
        if (dest == null)
            throw new ArgumentException("MOV destination cannot be null");
        // if the dest or src is $<address>, handle as memory address
        if (dest.StartsWith("$"))
        {
            var addrStr = dest.Substring(1);
            if (long.TryParse(addrStr, out var addr))
            {
                if (addr < 0 || addr + 8 > state.Memory.Length)
                    throw new ArgumentOutOfRangeException($"Memory address out of range: {addr}");
                BitConverter.GetBytes(value).CopyTo(state.Memory.Span.Slice((int)addr, 8));
                return;
            }
            else
            {
                throw new ArgumentException($"Invalid memory address: {addrStr}");
            }
        }

        if (!state._intRegisterMap.TryGetValue(dest, out var destIdx))
            throw new ArgumentException($"Unknown MOV destination: {dest}");

        state._intRegisters[destIdx] = value;
    }
}
