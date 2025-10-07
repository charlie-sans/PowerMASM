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
public class add : ICallable {
	public string Name => "add";

	public int ParameterCount => 2;

	[MetaLamaExtentions.IDebuggable] public void Call(MicroAsmVmState state, params object[] parameters) {
	var destName = parameters[0] as string;
	var destVal = VM.ResolveOperandValue(state, parameters[0].ToString());
	var srcVal = VM.ResolveOperandValue(state, parameters[1].ToString());
    if (destName.StartsWith("$")) {
		long addr = VM.ResolveOperandValue(state, destName);
		long result = destVal + srcVal;
		BitConverter.GetBytes(result).CopyTo(state.Memory.Span.Slice((int)addr, 8));
	} else {
		state.SetIntRegister(destName, destVal + srcVal);
        }
    }
}
