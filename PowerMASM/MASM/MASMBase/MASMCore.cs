using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.MASM.Base;
namespace PowerMASM.MASM.MASMProgram;
public class MASMCore {
	public string[] Instructions { get; set; } = null;
	public MASMLabel[] Labels { get; set; } = null;
	private MicroAsmVmState _state { get; set; } = null;
	public MASMCore(bool? create = true) {
		if (create == true) {
			_state = new MicroAsmVmState();
		
		}
	}

	public Dictionary<MASMAcessorModifiers.AccessorModifier, List<MASMLabel>> Label_Modifiers { get; set; } = new();

	public MASMCore PreProcess(string code) {
		// Split code into lines, trim whitespace, and remove comments
		var lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(line => line.Trim())
						.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith(";")) // Remove comments and empty lines
						.ToArray();




		return null;
	}
}
