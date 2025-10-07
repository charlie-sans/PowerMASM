using System;
using PowerMASM.Core;
using PowerMASM.Core.Interfaces;
using PowerMASM.Core.MASMExtentions;

namespace PowerMASM.Core.MASMFunctions;
public class DbInstruction : ICallable {
	public string Name => "DB";

	public int ParameterCount => 1;
	string ICallable.ToString() => Name;

	[MetaLamaExtentions.IDebuggable] public void Call(MicroAsmVmState state, params object[] parameters)
	{
		// Console.WriteLine("DB instruction called");
		// Console.WriteLine($"Parameters: {string.Join(", ", parameters)} with length {parameters.Length}");
		if (parameters.Length != 2)
		{
			throw new ArgumentException("DB requires exactly two operands");
		}
		// db $<address> "<value>"
		if (parameters[0] is not string addressParam || !addressParam.StartsWith('$'))
		{
			throw new ArgumentException("First operand of DB must be an address in the format $<address>");
		}

		if (parameters[1] is not string valueParam)
		{
			throw new ArgumentException("Second operand of DB must be a string");
		}
		if (!int.TryParse(addressParam[1..], out int address))
		{
			throw new ArgumentException("Invalid address format in DB instruction");
		}
		if (address < 0 || address >= state.Memory.Length)
		{
			throw new ArgumentOutOfRangeException("Address is out of memory bounds");
		}
		var trimmedValue = valueParam.Trim().Replace("\"", "").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\r", "\r");
		byte[] bytes = System.Text.Encoding.ASCII.GetBytes(trimmedValue);
		if (address + bytes.Length > state.Memory.Length)
		{
			throw new ArgumentOutOfRangeException("String exceeds memory bounds");
		}
		try
		{

			bytes.CopyTo(state.Memory.Span.Slice(address, bytes.Length));
		}
		catch (Exception ex)
		{
			throw new Exception("Error writing to memory in DB instruction: " + ex.Message);
		}
	}
}
