using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;

namespace PowerMASM.Core.MASMBase;
public class MASMCore {
	public List<string> Instructions { get; set; } = null;
	public MASMLabel[] Labels { get; set; } = null;
	private MicroAsmVmState _state { get; set; } = null;
	public MASMCore(bool? create = true) {
		if (create == true) {
			_state = new MicroAsmVmState();
		
		}
	}

	public Dictionary<MASMAcessorModifiers.AccessorModifier, List<MASMLabel>> Label_Modifiers { get; set; } = new();

	public void Run()
	{
		Console.WriteLine("meow");
	}

	public MASMCore PreProcess(string code) {
		// Split code into lines, trim whitespace, and remove comments
		var lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(line => line.Trim())
						.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith(";")) // Remove comments and empty lines
						.ToArray();

		var labels = new List<MASMLabel>();
		var instructions = new List<string>();
		
		/*
		 * #include "somefile.mas" ; local
		 * #include <somefile.mas> ; global from include paths
		 *  lbl main
		 *		mov RAX 0 ; stdout
		 *		mov RBX 100 ; value
		 *		call #printf ; defined in include file
		 *		; optional ret here
		 *		
		 */

		return null;
	}
}
