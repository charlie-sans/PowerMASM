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

        if (!state._intRegisterMap.TryGetValue(dest, out var destIdx))
            throw new ArgumentException($"Unknown MOV destination: {dest}");

        state._intRegisters[destIdx] = value;
    }
}
