using System;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
using PowerMASM.Core.MASMExtentions;

namespace PowerMASM.Core.MASMFunctions;
public class JnzInstruction : ICallable {
	public string Name => "JNZ";

	public int ParameterCount => 1;
	string ICallable.ToString() => Name;

	[MetaLamaExtentions.IDebuggable] public void Call(MicroAsmVmState state, params object[] parameters) {
		JumpHelpers.JumpIf(state, parameters, flags => !flags.Zero);
	}
}
