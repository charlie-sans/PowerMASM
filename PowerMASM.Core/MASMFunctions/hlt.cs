using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class hlt: ICallable {
	public string Name => "hlt";

	public int ParameterCount => 0;

	public void Call(MicroAsmVmState state, params object[] parameters) {
		// do nothing, it's a stub anyways.
	}
}
