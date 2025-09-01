using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class fmov: ICallable {
	public string Name => "fmov";

	public int ParameterCount => 2;

	public void Call(MicroAsmVmState state, params object[] parameters) {
		throw new NotImplementedException();
	}
}
