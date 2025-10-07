using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class LblInstruction : ICallable {
	public string Name => "LBL";

	public int ParameterCount => 1;
	string ICallable.ToString() => Name;

	[MetaLamaExtentions.IDebuggable] public void Call(MicroAsmVmState state, params object[] parameters) {
		// Labels are processed during pre-execution scanning. No runtime action required.
	}
}
