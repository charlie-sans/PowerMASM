using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.MASM.MASMProgram;
namespace PowerMASM.MASM.Runtime;
public class VM {
	public MicroAsmVmState State { get; private set; }
	public MASMCore Program { get; set; }
	public VM(int memorySize = 65536) {
		State = new MicroAsmVmState(memorySize);
	}
	public override string ToString() {
		return State.ToString();
	}
	public void Reset() {
		State = new MicroAsmVmState(State.Memory.Length);
	}
	public void LoadProgram(string[] program) {
		// Load program into memory starting at address 0
		var bytes = Encoding.UTF8.GetBytes(string.Join("\n", program));
		bytes.CopyTo(State.Memory);
		// Set instruction pointer (RIP) to start of program
		State.SetIntRegister("RIP", 0);
	}
}
