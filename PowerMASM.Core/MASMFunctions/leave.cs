using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class leave : ICallable {
	public string Name => "LEAVE";

	public int ParameterCount => 1;

	[MetaLamaExtentions.IDebuggable]
    public void Call(MicroAsmVmState state, params object[] parameters)
    {
        state.CallStack.PopLabelFrame();
        long rbp = state.GetIntRegister("RBP");
        if (rbp < 0 || rbp + 8 > state.Memory.Length)
        {
            throw new InvalidOperationException("Stack underflow when performing LEAVE");
        }
        // Restore old RBP
        long oldRbp = BitConverter.ToInt64(state.Memory.Span.Slice((int)rbp, 8));
        state.SetIntRegister("RSP", rbp);
        state.SetIntRegister("RBP", oldRbp);
    }
}
