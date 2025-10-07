using System;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class CallInstruction : ICallable {
    public string Name => "CALL";

    public int ParameterCount => 1;
    string ICallable.ToString() => Name;

    [MetaLamaExtentions.IDebuggable] public void Call(MicroAsmVmState state, params object[] parameters) {
        if (parameters.Length != 1) {
            throw new ArgumentException("CALL requires exactly one parameter");
        }

        if (parameters[0] is not string rawTarget) {
            throw new ArgumentException("CALL target must be a label name");
        }

        var labelName = rawTarget.Trim();
        if (labelName.StartsWith("#", StringComparison.Ordinal)) {
            labelName = labelName[1..];
        }

        if (!state.LabelIndices.TryGetValue(labelName, out var targetIndex)) {
            throw new Exception($"Label '{labelName}' not found for CALL");
        }

        long returnAddress = state.GetIntRegister("RIP") + 1;
        long rsp = state.GetIntRegister("RSP") - 8;
        if (rsp < 0 || rsp + 8 > state.Memory.Length) {
            throw new InvalidOperationException("Stack overflow when performing CALL");
        }
        BitConverter.GetBytes(returnAddress).CopyTo(state.Memory.Span.Slice((int)rsp, 8));
        state.SetIntRegister("RSP", rsp);

        state.CallStack.PushLabelFrame(labelName, returnAddress);
        state.SetIntRegister("RIP", targetIndex);
    }
}
