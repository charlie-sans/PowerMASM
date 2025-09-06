using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
using PowerMASM.Core.MASMExtentions;

namespace PowerMASM.Core.MASMFunctions;
public class add : ICallable {
	public string Name => "add";

	public int ParameterCount => 2;

	public void Call(MicroAsmVmState state, params object[] parameters) {
	var destName = parameters[0] as string;
	var destVal = parameters[0].AsRegister(state);
	var srcVal = parameters[1].AsRegister(state);
	state.SetIntRegister(destName, destVal + srcVal);
	}
}
