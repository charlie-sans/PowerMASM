using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.Core.MASMException;
public class MASMException {
	public string Message { get; set; } = null;
	public int LineNumber { get; set; } = -1;
	public string Instruction { get; set; } = null;
	public Exception InnerException { get; set; } = null;
	public MASMException(string message, int lineNumber = -1, string instruction = null, Exception innerException = null) {
		Message = message;
		LineNumber = lineNumber;
		Instruction = instruction;
		InnerException = innerException;
	}
	public override string ToString() {
		var sb = new StringBuilder();
		sb.AppendLine($"MASM Exception: {Message}");
		if (LineNumber >= 0) {
			sb.AppendLine($"  Line Number: {LineNumber}");
		}
		if (!string.IsNullOrEmpty(Instruction)) {
			sb.AppendLine($"  Instruction: {Instruction}");
		}
		if (InnerException != null) {
			sb.AppendLine($"  Inner Exception: {InnerException}");
		}
		return sb.ToString();
	}
}
