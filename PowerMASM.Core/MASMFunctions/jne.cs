using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
using PowerMASM.Core.MASMExtentions;

namespace PowerMASM.Core.MASMFunctions;
public class jne: ICallable {
	public string Name => "JNE";

	public int ParameterCount => 1;

	[MetaLamaExtentions.IDebuggable] public void Call(MicroAsmVmState state, params object[] parameters) {
		JumpHelpers.JumpIf(state, parameters, flags => !flags.Zero);
    }
}
