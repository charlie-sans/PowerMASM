using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class div : ICallable {
	public string Name => "div";
	public int ParameterCount => 2;
	public void Call(MicroAsmVmState state, params object[] parameters) {
		var destName = parameters[0] as string;
		var destVal = parameters[0].AsRegister(state);
		var srcVal = parameters[1].AsRegister(state);
		if (srcVal == 0) throw new DivideByZeroException("Division by zero in div instruction");
		state.SetIntRegister(destName, destVal / srcVal);
	}
}
