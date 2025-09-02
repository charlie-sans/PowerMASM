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
    public string Name => "mov";

    public int ParameterCount => 2;

    public static void Call(MicroAsmVmState state, params object[] parameters)
    {
        var dest = parameters[0];
        var src = parameters[1] switch
        {
            int i => i,
            string s => s.AsRegister(state),
            _ => throw new ArgumentException("Invalid parameter type for mov")
        };
        state.SetIntRegister((string)dest, src);
    }
}
