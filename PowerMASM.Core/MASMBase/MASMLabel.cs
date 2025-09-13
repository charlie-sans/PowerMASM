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
}
