using System;
using Xunit;
using PowerMASM.Core.MASMBase;

namespace PowerMASM.Test;

public class MASMCoreTests
{
    [Fact]
    public void PreProcess_SplitsCodeAndRemovesComments()
    {
    string code = "mov rax 1\n; this is a comment\nadd rax 2\nlabel1:\nsub rax 1";
    var core = MASMCore.PreProcess(code);
    Assert.NotNull(core);
    Assert.Contains("mov rax 1", core.Instructions);
    Assert.Contains("add rax 2", core.Instructions);
    Assert.DoesNotContain("; this is a comment", core.Instructions);
    Assert.NotNull(core.Labels);
    Assert.Contains(core.Labels, l => l.Name == "label1");
    }

    [Fact]
    public void Run_PrintsMeow()
    {
        var core = new MASMCore();
        // This just checks that Run() does not throw
        var ex = Record.Exception(() => core.Run());
        Assert.Null(ex);
    }
}
