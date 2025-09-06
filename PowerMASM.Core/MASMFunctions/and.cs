using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class and : ICallable {
	public string Name => "and";
	public int ParameterCount => 2;
	public void Call(MicroAsmVmState state, params object[] parameters) {
		var destName = parameters[0] as string;
		var destVal = parameters[0].AsRegister(state);
		var srcVal = parameters[1].AsRegister(state);
		state.SetIntRegister(destName, destVal & srcVal);
	}
}
