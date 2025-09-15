using System;
using Xunit;
using PowerMASM.Core;
using PowerMASM.Core.MASMFunctions;

namespace PowerMASM.Test;

public class MASMFunctionsTests
{
    [Fact]
    public void Mov_SetsRegisterValue()
    {
        var state = new MicroAsmVmState();
        new mov().Call(state, "RAX", 42);
        Assert.Equal(42, state.GetIntRegister("RAX"));
    }

    [Fact]
    public void Add_AddsRegisterValues()
    {
        var state = new MicroAsmVmState();
        new mov().Call(state, "RAX", 10);
        new mov().Call(state, "RBX", 5);
        var addFunc = new add();
        addFunc.Call(state, "RAX", "RBX");
        Assert.Equal(15, state.GetIntRegister("RAX"));
    }
}
