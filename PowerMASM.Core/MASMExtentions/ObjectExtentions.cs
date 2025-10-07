using System;
using System.Collections.Generic;
using System.Globalization;
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
			}
			if (state._floatRegisterMap.TryGetValue(str, out int fIndex)) {
				return BitConverter.DoubleToInt64Bits(state._floatRegisters[fIndex]);
			}
			if (str.StartsWith("$", StringComparison.Ordinal)) {
				var addressToken = str.Substring(1);
				long address = ParseNumericOrRegister(addressToken, state);
				ValidateMemoryRead(address, state, 8);
				return BitConverter.ToInt64(state.Memory.Span.Slice((int)address, 8));
			}
			if (long.TryParse(str, out var parsedLong)) {
				return parsedLong;
			}
			if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && long.TryParse(str[2..], NumberStyles.HexNumber, null, out var hexValue)) {
				return hexValue;
			}
			throw new Exception($"Unknown operand: {str}");
		}
		else if (obj is long l) {
			return l;
		}
		else if (obj is int i) {
			return i;
		}
		else if (obj is short s) {
			return s;
		}
		else if (obj is byte b) {
			return b;
		}
		else if (obj is double d) {
			return BitConverter.DoubleToInt64Bits(d);
		}
		else if (obj is float f) {
			return BitConverter.DoubleToInt64Bits(f);
		}
		else {
			throw new Exception($"Cannot convert type {GetTypeName(obj)} to register");
		}

	}

	private static long ParseNumericOrRegister(string token, MicroAsmVmState state) {
		if (state._intRegisterMap.TryGetValue(token, out var regIndex)) {
			return state._intRegisters[regIndex];
		}
		if (token.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && long.TryParse(token[2..], NumberStyles.HexNumber, null, out var hexValue)) {
			return hexValue;
		}
		if (long.TryParse(token, out var value)) {
			return value;
		}
		throw new Exception($"Unable to resolve operand '{token}' to a numeric value");
	}

	private static void ValidateMemoryRead(long address, MicroAsmVmState state, int size) {
		if (address < 0 || address + size > state.Memory.Length) {
			throw new ArgumentOutOfRangeException(nameof(address), $"Memory address out of range: {address}");
		}
	}
}
