using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
using PowerMASM.Core.Runtime;

namespace PowerMASM.Core.MASMFunctions;
public class shr : ICallable {
	public string Name => "SHR";

	public int ParameterCount => 2;

	[MetaLamaExtentions.IDebuggable] public void Call(MicroAsmVmState state, params object[] parameters) {
		var dest = parameters[0].ToString();
		var destVal = VM.ResolveOperandValue(state, parameters[0].ToString());
		var srcVal = VM.ResolveOperandValue(state, parameters[1].ToString());
		state.SetIntRegister(dest, (long)((ulong)destVal >> (int)srcVal));
    }
}
