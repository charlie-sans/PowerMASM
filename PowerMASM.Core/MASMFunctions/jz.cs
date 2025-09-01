using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class jz: ICallable {
	public string Name => throw new NotImplementedException();

	public int ParameterCount => throw new NotImplementedException();

	public void Call(MicroAsmVmState state, params object[] parameters) {
		throw new NotImplementedException();
	}
}
