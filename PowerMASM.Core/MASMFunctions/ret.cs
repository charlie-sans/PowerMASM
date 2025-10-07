using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class ret: ICallable {
	public string Name => "RET";

	public int ParameterCount => 0;

	[MetaLamaExtentions.IDebuggable]
	public void Call(MicroAsmVmState state, params object[] parameters)
	{
		// Pop the return address from the stack
		long RSP = state.GetIntRegister("RSP");
		if (RSP < 0 || RSP + 8 > state.Memory.Length)
		{
			throw new InvalidOperationException("Stack underflow when performing RET");
		}
		long returnAddress = BitConverter.ToInt64(state.Memory.Span.Slice((int)RSP, 8));
		state.SetIntRegister("RSP", RSP + 8);
		// Set RIP to the return address
		state.SetIntRegister("RIP", returnAddress);
		if (state.CallStack.Count == 0)
		{
			//throw new InvalidOperationException("Call stack underflow when performing RET");
			return;
        }
        state.CallStack.PopLabelFrame();
    }
}
