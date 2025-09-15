using System;
using Xunit;
using PowerMASM.Core;
using PowerMASM.Core.MASMFunctions;

namespace PowerMASM.Test;

public class MASMArithmeticTests
{
    [Fact]
    public void Sub_SubtractsRegisterValues()
    {
        var state = new MicroAsmVmState();
        new mov().Call(state, "RAX", 10);
        new mov().Call(state, "RBX", 3);
        var subFunc = new sub();
        subFunc.Call(state, "RAX", "RBX");
        Assert.Equal(7, state.GetIntRegister("RAX"));
    }

    [Fact]
    public void Mul_MultipliesRegisterValues()
    {
        var state = new MicroAsmVmState();
        new mov().Call(state, "RAX", 4);
        new mov().Call(state, "RBX", 5);
        var mulFunc = new mul();
        mulFunc.Call(state, "RAX", "RBX");
        Assert.Equal(20, state.GetIntRegister("RAX"));
    }

    [Fact]
    public void Div_DividesRegisterValues()
    {
        var state = new MicroAsmVmState();
        new mov().Call(state, "RAX", 20);
        new mov().Call(state, "RBX", 4);
        var divFunc = new div();
        divFunc.Call(state, "RAX", "RBX");
        Assert.Equal(5, state.GetIntRegister("RAX"));
    }

    [Fact]
    public void Div_ThrowsOnDivideByZero()
    {
        var state = new MicroAsmVmState();
        new mov().Call(state, "RAX", 20);
        new mov().Call(state, "RBX", 0);
        var divFunc = new div();
        Assert.Throws<DivideByZeroException>(() => divFunc.Call(state, "RAX", "RBX"));
    }

    [Fact]
    public void And_PerformsBitwiseAnd()
    {
        var state = new MicroAsmVmState();
        new mov().Call(state, "RAX", 0b1101);
        new mov().Call(state, "RBX", 0b1011);
        var andFunc = new and();
        andFunc.Call(state, "RAX", "RBX");
        Assert.Equal(0b1001, state.GetIntRegister("RAX"));
    }
}
