using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
using PowerMASM.Core.MASMExtentions;
using PowerMASM.Core.Runtime;

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

        // Use VM.ResolveOperandValue for source value
        long value = VM.ResolveOperandValue(state, src);

        // If dest is a register
        if (state._intRegisterMap.ContainsKey(dest))
        {
            state.SetIntRegister(dest, value);
            return;
        }
        // If dest is a memory address (e.g., $1234, $RAX, $[RBP+4])
        else if (dest.StartsWith("$"))
        {
            long addr = VM.ResolveOperandValue(state, dest);
            BitConverter.GetBytes(value).CopyTo(state.Memory.Span.Slice((int)addr, 8));
            return;
        }
        throw new ArgumentException($"Unknown MOV destination: {dest}");
    }
}
