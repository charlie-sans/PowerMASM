using System;
using Xunit;
using PowerMASM.Core.MASMException;

namespace PowerMASM.Test;

public class MASMExceptionTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var ex = new MASMException("Error!", 42, "mov rax 1", new InvalidOperationException("inner"));
        Assert.Equal("Error!", ex.Message);
        Assert.Equal(42, ex.LineNumber);
        Assert.Equal("mov rax 1", ex.Instruction);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Fact]
    public void ToString_ContainsAllInfo()
    {
        var ex = new MASMException("Oops", 7, "add rbx 2", null);
        var str = ex.ToString();
        Assert.Contains("MASM Exception: Oops", str);
        Assert.Contains("Line Number: 7", str);
        Assert.Contains("Instruction: add rbx 2", str);
    }

    [Fact]
    public void ToString_HandlesNulls()
    {
        var ex = new MASMException("Only message");
        var str = ex.ToString();
        Assert.Contains("MASM Exception: Only message", str);
        Assert.DoesNotContain("Line Number", str);
        Assert.DoesNotContain("Instruction", str);
        Assert.DoesNotContain("Inner Exception", str);
    }
}
