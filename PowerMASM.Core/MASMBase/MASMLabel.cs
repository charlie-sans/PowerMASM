using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.Core.MASMBase;

public class MASMLabel {
	public string Name { get; set; } = null;
	public string[] Instructions { get; set; } = null;
	public MASMAcessorModifiers.ObjectiveModifiers? modifiers { get; set; } = null;

	// --- FDEF/Function support ---
	public bool IsFunction { get; set; } = false; // True if this label is a function (fdef)
	public string ReturnType { get; set; } = null; // e.g. "int", "double"
	public List<(string Type, string Name)> Parameters { get; set; } = null; // List of (type, name)
	public bool Exported { get; set; } = false; // True if function is exported (fdef)

	// --- Data label support ---
	public string DataDirective { get; set; } = null; // e.g., "DB", "DW"
	public string DataValues { get; set; } = null;    // e.g., '"hello, world", 0'
}
