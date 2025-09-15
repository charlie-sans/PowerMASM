using System;
using Xunit;
using PowerMASM.Core.MASMBase;

namespace PowerMASM.Test;

public class MASMCoreLabelParsingTests
{
    [Fact]
    public void PreProcess_ParsesDataLabel_DB()
    {
        string code = "hello: DB \"hello, world!\", 0\nmain: mov rax hello";
        var core = MASMCore.PreProcess(code);
        Assert.NotNull(core.Labels);
        var label = Assert.Single(core.Labels, l => l.Name == "hello");
        Assert.Equal("DB", label.DataDirective);
        Assert.Equal("\"hello, world!\", 0", label.DataValues);
    }

    [Fact]
    public void PreProcess_ParsesMultipleDataLabels()
    {
        string code = "msg1: DB \"abc\", 0\nmsg2: DW 1234\nLBL main\nmov rax msg1\nmov rbx msg2";
        var core = MASMCore.PreProcess(code);
        Assert.NotNull(core.Labels);
        Assert.Contains(core.Labels, l => l.Name == "msg1" && l.DataDirective == "DB");
        Assert.Contains(core.Labels, l => l.Name == "msg2" && l.DataDirective == "DW");
    }

    [Fact]
    public void PreProcess_ParsesLabelWithInstructions()
    {
        string code = "start:\nmov rax 1\nmov rbx 2";
        var core = MASMCore.PreProcess(code);
        var label = Assert.Single(core.Labels, l => l.Name == "start");
        Assert.NotNull(label.Instructions);
        Assert.Contains("mov rax 1", label.Instructions);
        Assert.Contains("mov rbx 2", label.Instructions);
    }

    [Fact]
    public void PreProcess_ParsesDataLabelWithWhitespace()
    {
        string code = "data1:   DB   1, 2, 3\n";
        var core = MASMCore.PreProcess(code);
        var label = Assert.Single(core.Labels, l => l.Name == "data1");
        Assert.Equal("DB", label.DataDirective);
        Assert.Equal("1, 2, 3", label.DataValues);
    }

    [Fact]
    public void PreProcess_ParsesRESBDataLabel()
    {
        string code = "buffer: RESB 256";
        var core = MASMCore.PreProcess(code);
        var label = Assert.Single(core.Labels, l => l.Name == "buffer");
        Assert.Equal("RESB", label.DataDirective);
        Assert.Equal("256", label.DataValues);
    }
}
