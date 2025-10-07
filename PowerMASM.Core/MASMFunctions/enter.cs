using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class enter : ICallable {
	public string Name => "ENTER";

	public int ParameterCount => 1;

	[MetaLamaExtentions.IDebuggable]
    public void Call(MicroAsmVmState state, params object[] parameters)
    {
        long rbp = state.GetIntRegister("RBP");
        if (rbp < 0 || rbp + 8 > state.Memory.Length)
        {
            throw new InvalidOperationException("Stack underflow when performing LEAVE");
        }
        // Restore old RBP
        long oldRbp = BitConverter.ToInt64(state.Memory.Span.Slice((int)rbp, 8));
        state.SetIntRegister("RSP", rbp);
        state.SetIntRegister("RBP", oldRbp);
        if (state.CallStack.Count == 0)
        {
            //throw new InvalidOperationException("Call stack underflow when performing ENTER");
            return;
        }
        state.CallStack.PopLabelFrame();
    }
}
