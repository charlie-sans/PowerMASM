using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
using PowerMASM.Core.MASMExtentions;
using PowerMASM.Core.Runtime;
using System;

namespace PowerMASM.Core.MASMFunctions;
public class CmpInstruction : ICallable {
	public string Name => "CMP";

	public int ParameterCount => 2;
	string ICallable.ToString() => Name;

	[MetaLamaExtentions.IDebuggable] public void Call(MicroAsmVmState state, params object[] parameters) {
		if (parameters.Length != 2) {
			throw new ArgumentException("CMP requires exactly two operands");
		}

        var destValue = VM.ResolveOperandValue(state, parameters[0].ToString());
        var srcValue = VM.ResolveOperandValue(state, parameters[1].ToString());
        long result = destValue - srcValue;

		state.Flags.Zero = result == 0;
		state.Flags.Sign = result < 0;
		state.Flags.Carry = (ulong)destValue < (ulong)srcValue;

		bool destNegative = destValue < 0;
		bool srcNegative = srcValue < 0;
		bool resultNegative = result < 0;
		state.Flags.Overflow = destNegative != srcNegative && destNegative != resultNegative;
	}
}
