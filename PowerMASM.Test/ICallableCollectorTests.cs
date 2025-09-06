using System;
using Xunit;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Test;

public class ICallableCollectorTests
{
    [Fact]
    public void GetCallableNames_ReturnsNames()
    {
        var collector = new ICallableCollector();
        var fake = new FakeCallable();
        collector.Callables.Add(fake);
        var names = collector.GetCallableNames();
        Assert.Contains("fake", names);
    }

    [Fact]
    public void GetCallableByName_FindsCallable()
    {
        var collector = new ICallableCollector();
        var fake = new FakeCallable();
        collector.Callables.Add(fake);
        var found = collector.GetCallableByName("fake");
        Assert.Equal(fake, found);
    }

    [Fact]
    public void GetCallablesByParameterCount_FiltersCorrectly()
    {
        var collector = new ICallableCollector();
        collector.Callables.Add(new FakeCallable());
        Assert.Single(collector.GetCallablesByParameterCount(2));
        Assert.Empty(collector.GetCallablesByParameterCount(1));
    }

    private class FakeCallable : ICallable
    {
        public string Name => "fake";
        public int ParameterCount => 2;
    }
}
