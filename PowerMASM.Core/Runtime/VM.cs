using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.MASMBase;

namespace PowerMASM.Core.Runtime;
public class VM(int memorySize = 65536) {
	public MicroAsmVmState State { get; private set; } = new MicroAsmVmState(memorySize);
	public MASMCore Program { get; set; }

	public override string ToString() {
		return State.ToString();
	}
	public void Reset() {
		State = new MicroAsmVmState(State.Memory.Length);
	}
	public void LoadProgram(string[] program) {
		
		// Set instruction pointer (RIP) to start of program
		State.SetIntRegister("RIP", 0);
	}
}
