using PowerMASM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.Core.Interfaces;
public interface ICallable
{
    string Name { get; }
    int ParameterCount { get; }
    /// <summary>
    /// Allows the VM to call this function with the given state and parameters.
    /// </summary>
    /// <param name="state">the current state of the MASM Runtime</param>
    /// <param name="parameters">Params that was passed to the ICallable</param>
	 void Call(MicroAsmVmState state, params object[] parameters) { }

    string ToString() => Name;
}
