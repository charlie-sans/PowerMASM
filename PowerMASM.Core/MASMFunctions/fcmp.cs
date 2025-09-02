using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class fcmp : ICallable {
	public string Name => "fcmp";

	public int ParameterCount => throw new NotImplementedException();

	public static void Call(MicroAsmVmState state, params object[] parameters) {
		throw new NotImplementedException();
	}
}
