using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;

namespace PowerMASM.Core.MASMExtentions;
static class ObjectExtentions {
	public static string GetTypeName(object obj) {
		if (obj == null) return "null";
		return obj.GetType().Name;
	}
	public static long AsRegister(this object obj, MicroAsmVmState state) {

		if (obj == null) throw new Exception("Cannot convert null to register");
		if (obj is string str) {
			if (state._intRegisterMap.TryGetValue(str, out int index)) {
				return state._intRegisters[index];
			} else if (state._floatRegisterMap.TryGetValue(str, out int fIndex)) {
				return BitConverter.DoubleToInt64Bits(state._floatRegisters[fIndex]);
			} else {
				throw new Exception($"Unknown register name: {str}");
			}
		} else if (obj is long l) {
			return l;
		} else if (obj is int i) {
			return i;
		} else if (obj is short s) {
			return s;
		} else if (obj is byte b) {
			return b;
		} else if (obj is double d) {
			return BitConverter.DoubleToInt64Bits(d);
		} else if (obj is float f) {
			return BitConverter.DoubleToInt64Bits(f);
		} else {
			throw new Exception($"Cannot convert type {GetTypeName(obj)} to register");
		}

	}
}
