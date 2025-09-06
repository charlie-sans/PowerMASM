using System;
using Xunit;
using PowerMASM.Core;
using PowerMASM.Core.MASMBase;

namespace PowerMASM.Test;

public class MicroAsmVmStateTests
{
    [Fact]
    public void Constructor_InitializesMemoryAndRegisters()
    {
        var vm = new MicroAsmVmState(1024);
        Assert.Equal(1024, vm.Memory.Length);
        Assert.Equal(1024, vm.GetIntRegister("RSP"));
    }

    [Fact]
    public void SetAndGetIntRegister_WorksCorrectly()
    {
        var vm = new MicroAsmVmState();
        vm.SetIntRegister("RAX", 123);
        Assert.Equal(123, vm.GetIntRegister("RAX"));
    }

    [Fact]
    public void SetAndGetFloatRegister_WorksCorrectly()
    {
        var vm = new MicroAsmVmState();
        vm.SetFloatRegister("FP0", 3.14);
        Assert.Equal(3.14, vm.GetFloatRegister("FP0"), 3);
    }

    [Fact]
    public void Reset_ClearsRegistersAndMemory()
    {
        var vm = new MicroAsmVmState(128);
        vm.SetIntRegister("RAX", 42);
        vm.SetFloatRegister("FP0", 2.71);
        vm.Memory.Span[0] = 0xFF;
        vm.Reset();
        Assert.Equal(0, vm.GetIntRegister("RAX"));
        Assert.Equal(0, vm.GetFloatRegister("FP0"));
        Assert.Equal(0, vm.Memory.Span[0]);
        Assert.Equal(128, vm.GetIntRegister("RSP"));
    }
}
