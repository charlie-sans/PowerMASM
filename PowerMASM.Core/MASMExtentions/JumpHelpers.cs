using System;
using PowerMASM.Core;

namespace PowerMASM.Core.MASMExtentions;

internal static class JumpHelpers
{
    private static string NormalizeLabel(string rawLabel)
    {
        var label = rawLabel.Trim();
        if (label.StartsWith("#", StringComparison.Ordinal))
        {
            label = label[1..];
        }
        return label;
    }

    private static int ResolveLabelIndex(MicroAsmVmState state, object[] parameters)
    {
        if (parameters.Length != 1)
        {
            throw new ArgumentException("Jump instructions require exactly one parameter");
        }

        if (parameters[0] is not string rawLabel)
        {
            throw new ArgumentException("Jump target must be a label reference");
        }

        var labelName = NormalizeLabel(rawLabel);
      
        
        if (!state.LabelIndices.TryGetValue(labelName, out var targetIndex))
        {
            throw new Exception($"Label '{labelName}' not found");
        }

        return targetIndex;
    }

    public static void JumpUnconditional(MicroAsmVmState state, object[] parameters)
    {
        var targetIndex = ResolveLabelIndex(state, parameters);
        state.SetIntRegister("RIP", targetIndex);
    }

    public static void JumpIf(MicroAsmVmState state, object[] parameters, Func<ProcessorFlags, bool> condition)
    {
        var targetIndex = ResolveLabelIndex(state, parameters);
        if (condition(state.Flags))
        {
            state.SetIntRegister("RIP", targetIndex);
        }
    }
}
