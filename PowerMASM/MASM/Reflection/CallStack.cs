using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PowerMASM.MASM.Interfaces;

namespace PowerMASM.MASM.Reflection;

//TODO: Implement CallStack class to track function calls and execution flow in MASM programs.
public class MASMCallStack {
	List<ICallable> callStack = new List<ICallable>();
	public MASMCallStack() {

	}

}
