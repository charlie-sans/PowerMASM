using System;
using Xunit;
using PowerMASM.Core.MASMBase;

namespace PowerMASM.Test;

public class MASMLabelTests
{
    [Fact]
    public void MASMLabel_DefaultsToNullProperties()
    {
        var label = new MASMLabel();
        Assert.Null(label.Name);
        Assert.Null(label.Instructions);
        Assert.Null(label.modifiers);
    }

    [Fact]
    public void MASMLabel_CanSetProperties()
    {
        var label = new MASMLabel
        {
            Name = "start",
            Instructions = new[] { "mov rax 1" },
        };
        Assert.Equal("start", label.Name);
        Assert.Single(label.Instructions);
        Assert.Equal("mov rax 1", label.Instructions[0]);
    }
}
