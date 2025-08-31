using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.MASM.Base;
public class MASMAcessorModifiers {
	public enum AccessorModifier {
		Public,
		Private,
		Protected,
		Internal,
		Static,
		ReadOnly,
		WriteOnly,
		Const
	}
	public struct ObjectiveModifiers {
		public bool IsPointer { get; set; }
		public bool IsReference { get; set; }
		public bool IsArray { get; set; }
		public int ArraySize { get; set; }
		public bool IsFunction { get; set; }
		public string ReturnType { get; set; }
		public List<string> Parameters { get; set; }
		public List<AccessorModifier> Modifiers { get; set; }
	}
}
