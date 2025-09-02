using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;

namespace PowerMASM.Core.MASMFunctions;
public class Out : ICallable {
	public string Name => throw new NotImplementedException();

	public int ParameterCount => throw new NotImplementedException();

	public static void Call(MicroAsmVmState state, params object[] parameters) {
		var IOOutput = int.Parse((string)parameters[0]); // should be int or string
														 // user can output to 0 = stdout, 1 = stderr
		if (IOOutput == 0) {
			var content = parameters[1] switch {
				int i => i.ToString(),
				string s => s,
				_ => throw new ArgumentException("Invalid parameter type for out")
			};
			Console.Write(content);
		} else if (IOOutput == 1) {
			var content = parameters[1] switch {
				int i => i.ToString(),
				string s => s,
				_ => throw new ArgumentException("Invalid parameter type for out")
			};
			Console.Error.Write(content);
		} else {
			throw new ArgumentException("Invalid IOOutput parameter for out");
		}
	}
}
