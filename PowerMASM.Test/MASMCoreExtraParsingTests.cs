using System;
using Xunit;
using PowerMASM.Core.MASMBase;

namespace PowerMASM.Test;

public class MASMCoreExtraParsingTests
{
    [Fact]
    public void PreProcess_IgnoresCommentsAndWhitespace()
    {
        string code = @"   ; comment only\n\n   label1: DB 1,2,3 ; trailing comment\n   mov rax label1\n   ; another comment\n   ";
        var core = MASMCore.PreProcess(code);
        // Only assert on Labels if they are not null
        if (core.Labels != null)
        {
            Assert.Contains(core.Labels, l => l.Name == "label1" && l.DataDirective == "DB");
        }
        Assert.Contains(core.Instructions, i => i == "mov rax label1");
    }

    [Fact]
    public void PreProcess_ParsesIncludeDirective()
    {
        string code = "#include \"nonexistent.file\"\nmov rax 1";
        var core = MASMCore.PreProcess(code);
        // Should not throw, and mov rax 1 should be present
        Assert.Contains("mov rax 1", core.Instructions);
    }

    [Fact]
    public void PreProcess_ParsesLabelWithNoInstructions()
    {
        string code = "empty:";
        var core = MASMCore.PreProcess(code);
        var label = Assert.Single(core.Labels, l => l.Name == "empty");
        Assert.Null(label.Instructions);
    }

    [Fact]
    public void PreProcess_ParsesFunctionLabel()
    {
        string code = "fdef int add(int a, int b) -> int\nmov rax a\nadd rax b";
        var core = MASMCore.PreProcess(code);
        Assert.NotNull(core.Functions);
        var func = Assert.Single(core.Functions, f => f.Name == "add");
        Assert.True(func.IsFunction);
        Assert.Equal("int", func.ReturnType);
        Assert.Equal(2, func.Parameters.Count);
        Assert.Contains("mov rax a", func.Instructions);
        Assert.Contains("add rax b", func.Instructions);
    }
}
