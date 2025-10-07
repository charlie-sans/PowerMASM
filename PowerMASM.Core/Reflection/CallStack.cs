using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.Reflection;

public class MASMCallStack
{
    public abstract record CallFrame;
    public record MASMLabelFrame(string LabelName, long ReturnAddress) : CallFrame;
    public record MNIModuleFrame(IModuleFunctionAttribute Function, IModule Module) : CallFrame;

    private Stack<CallFrame> callStack = new();

    // For MASM label/function calls
    public void PushLabelFrame(string labelName, long returnAddress)
        => callStack.Push(new MASMLabelFrame(labelName, returnAddress));

    public MASMLabelFrame PopLabelFrame()
        => callStack.Pop() as MASMLabelFrame ?? throw new InvalidOperationException("Top frame is not a MASM label frame.");

    // For MNI function calls
    public void PushMNIModuleFrame(IModuleFunctionAttribute function, IModule module)
        => callStack.Push(new MNIModuleFrame(function, module));

    public MNIModuleFrame PopMNIModuleFrame()
        => callStack.Pop() as MNIModuleFrame ?? throw new InvalidOperationException("Top frame is not an MNI module frame.");

    public CallFrame? Peek() => callStack.Count > 0 ? callStack.Peek() : null;
    public int Count => callStack.Count;

}
