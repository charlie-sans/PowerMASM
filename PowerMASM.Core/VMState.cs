using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core.MASMBase;
using PowerMASM.Core.Reflection;
using PowerMASM.Core.Runtime;


namespace PowerMASM.Core;
public class ProcessorFlags {
	public bool Zero { get; set; } = false;      // Zero Flag
	public bool Sign { get; set; } = false;      // Sign Flag
	public bool Overflow { get; set; } = false;  // Overflow Flag
	public bool Carry { get; set; } = false;     // Carry Flag
	
	public void UpdateFlags(long result) {
		Zero = result == 0;
		Sign = result < 0;
		if (result > long.MaxValue || result < long.MinValue) {
			Overflow = true;
			Carry = true;
		} else {
			Overflow = false;
			Carry = false;
		}
	}
}

public class MicroAsmVmState {
	// Integer registers (64-bit)
	public long[] _intRegisters = new long[32];
	// Floating-point registers (64-bit double)
	public double[] _floatRegisters = new double[16];

    // Console input
	public ConsoleInWrapper ConsoleIn { get; set; } = new ConsoleInWrapper();

	// Console Output
	public ConsoleOutWrapper ConsoleOut { get; set; } = new ConsoleOutWrapper();

    // Flags register 
    public ProcessorFlags Flags { get; set; } = new ProcessorFlags();
	// Memory (simulated as byte array or Memory<byte>)
	public Memory<byte> Memory { get; set; }
	// Stack pointer (RSP) and base pointer (RBP) are just indices into _intRegisters

	public MASMCallStack CallStack = new();
	public List<MASMException.MASMException> Exceptions = new();
	public List<MASMLabel> Labels = new();
	public Dictionary<string, int> LabelIndices { get; } = new(StringComparer.OrdinalIgnoreCase);

	// Register name to index mappings
	public Dictionary<string, int> _intRegisterMap = null!;
	public Dictionary<string, int> _floatRegisterMap = null!;
	

    public MicroAsmVmState(int memorySize = 65536) {
		Memory = new byte[memorySize].AsMemory();
		InitializeRegisterMaps();
		// Initialize RSP to end of memory (stack grows downward)
		SetIntRegister("RSP", memorySize);
	}
    [MetaLamaExtentions.IDebuggable]
    public void InitializeRegisterMaps() {
		_intRegisterMap = new(StringComparer.OrdinalIgnoreCase) {
			["RAX"] = 0,
			["RBX"] = 1,
			["RCX"] = 2,
			["RDX"] = 3,
			["RSI"] = 4,
			["RDI"] = 5,
			["RBP"] = 6,
			["RSP"] = 7,
			["RIP"] = 8,
			["R0"] = 9,
			["R1"] = 10,
			["R2"] = 11,
			["R3"] = 12,
			["R4"] = 13,
			["R5"] = 14,
			["R6"] = 15,
			["R7"] = 16,
			["R8"] = 17,
			["R9"] = 18,
			["R10"] = 19,
			["R11"] = 20,
			["R12"] = 21,
			["R13"] = 22,
			["R14"] = 23,
			["R15"] = 24,
		};
		_floatRegisterMap = new(StringComparer.OrdinalIgnoreCase) {
			["FP0"] = 0,
			["FP1"] = 1,
			["FP2"] = 2,
			["FP3"] = 3,
			["FP4"] = 4,
			["FP5"] = 5,
			["FP6"] = 6,
			["FP7"] = 7,
			["FP8"] = 8,
			["FP9"] = 9,
			["FP10"] = 10,
			["FP11"] = 11,
			["FP12"] = 12,
			["FP13"] = 13,
			["FP14"] = 14,
			["FP15"] = 15,
		};
	}
	[MetaLamaExtentions.IDebuggable]
    public void Reset() {
		Array.Clear(_intRegisters, 0, _intRegisters.Length);
		Array.Clear(_floatRegisters, 0, _floatRegisters.Length);
		Flags = new ProcessorFlags();
		Memory.Span.Clear();
		SetIntRegister("RSP", Memory.Length);
		
		Exceptions.Clear();
		Labels.Clear();
		LabelIndices.Clear();
    }
	[MetaLamaExtentions.IDebuggable]
    public long GetIntRegister(string name) => _intRegisters[_intRegisterMap[name]];
	[MetaLamaExtentions.IDebuggable]
    public void SetIntRegister(string name, long value) => _intRegisters[_intRegisterMap[name]] = value;
	[MetaLamaExtentions.IDebuggable]
    public double GetFloatRegister(string name) => _floatRegisters[_floatRegisterMap[name]];
	[MetaLamaExtentions.IDebuggable]
    public void SetFloatRegister(string name, double value) => _floatRegisters[_floatRegisterMap[name]] = value;
	[MetaLamaExtentions.IDebuggable]
    public override string ToString() {
		var sb = new StringBuilder();
		sb.AppendLine("Integer Registers:");
		foreach (var reg in _intRegisterMap) {
			sb.Append($"{reg.Key}: {GetIntRegister(reg.Key)},");
		}
		sb.AppendLine("Floating-Point Registers:");
		foreach (var reg in _floatRegisterMap) {
			sb.Append($"{reg.Key}: {GetFloatRegister(reg.Key)},");
		}
		sb.AppendLine($"Flags: Z={Flags.Zero} S={Flags.Sign} O={Flags.Overflow} C={Flags.Carry}");
		return sb.ToString();
	}


}
