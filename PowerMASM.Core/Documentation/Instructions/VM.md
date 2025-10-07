# VM Instruction Execution

This document explains how instructions are run in the PowerMASM Virtual Machine (`VM`).

## Overview

The `VM` class is responsible for loading programs, managing state, and executing instructions. It uses a `MicroAsmVmState` to track memory, registers, and labels, and an `ICallableCollector` to map opcodes to callable instruction implementations.

## Loading a Program

- The VM loads a program via `LoadProgram`, which sets up memory, labels, and data sections.
- Data labels are parsed and written to memory according to their directive (`DB`, `DW`, `DD`, `DQ`, `RESB`).
- Code labels are stored for later reference.
- The instruction pointer (`RIP`) is set to the start of the program.
- The VM looks for a `_start` or `main` label to begin execution, or runs a provided instruction array.

## Executing Instructions

- The `ExecuteInstructions` method runs instructions line by line.
- Each line is parsed into an opcode and arguments.
- The VM uses `ICallableCollector.GetCallableByName` to find the implementation for the opcode.
- The instruction is executed by calling `callable.Call(State, args)`.
- Errors are caught and logged in the VM state.

### Example Flow

1. Load program and initialize state.
2. Set up data and code labels.
3. Set instruction pointer to entry point.
4. For each instruction:
    - Parse opcode and arguments.
    - Find corresponding callable.
    - Execute instruction, updating VM state.
    - Handle errors gracefully.

## Related Files
- `Runtime/VM.cs`: Main VM logic.
- `MASMFunctions/`: Individual instruction implementations.
- `Interfaces/ICallable.cs`: Interface for instruction callables.

## See Also
- [ICallable.md](ICallable.md) for details on instruction callables.
- [MicroV2.md](../MicroV2.md) for VM architecture overview.
