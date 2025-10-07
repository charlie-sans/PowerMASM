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

    [MetaLamaExtentions.IDebuggable] public void Call(MicroAsmVmState state, params object[] parameters)
    {
        if (parameters.Length != 2)
            throw new ArgumentException("MOV requires 2 parameters");

        var dest = parameters[0].ToString();
        var src = parameters[1].ToString();

        long value = 0;
        bool valueAssigned = false;

        // Try register first, then immediate
        if (state._intRegisterMap.TryGetValue(src, out var srcIdx))
        {
            value = state._intRegisters[srcIdx];
            valueAssigned = true;
        }
        else if (long.TryParse(src, out var imm))
        {
            value = imm;
            valueAssigned = true;
        }

        if (src.StartsWith("$"))
        {
            var addrStr = src.Substring(1);
            if (long.TryParse(addrStr, out var addr))
            {
                if (addr < 0 || addr + 8 > state.Memory.Length)
                    throw new ArgumentOutOfRangeException($"Memory address out of range: {addr}");
                value = BitConverter.ToInt64(state.Memory.Span.Slice((int)addr, 8));
                valueAssigned = true;
            }
            else
            {
                throw new ArgumentException($"Invalid memory address: {addrStr}");
            }
        }

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
                if (!valueAssigned)
                    throw new ArgumentException("MOV source value is not assigned");
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

        if (!valueAssigned)
            throw new ArgumentException("MOV source value is not assigned");

        state._intRegisters[destIdx] = value;
    }
}
