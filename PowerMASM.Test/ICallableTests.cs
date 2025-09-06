using System;
using Xunit;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Test;

public class ICallableTests
{
    private class DummyCallable : ICallable
    {
    public string Name => "dummy";
    public int ParameterCount => 1;
    public override string ToString() => Name;
    }

    [Fact]
    public void Name_ReturnsExpected()
    {
        var c = new DummyCallable();
        Assert.Equal("dummy", c.Name);
    }

    [Fact]
    public void ParameterCount_ReturnsExpected()
    {
        var c = new DummyCallable();
        Assert.Equal(1, c.ParameterCount);
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        var c = new DummyCallable();
        Assert.Equal("dummy", c.ToString());
    }
}
