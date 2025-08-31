using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.MASM.Base;

namespace PowerMASM.MASM.MASMProgram;
public class MASMLabel {
	public string Name { get; set; } = null;
	public string[] Instructions { get; set; } = null;
	public MASMAcessorModifiers[] modifiers { get; set; } = null;
}
