# Micro-Assembly instructions

with Regular micro-assembly interpreters, the instructions are defined in functions that live within one or more files.
developers will often use if/switch statements with enums or strings to determine what to call and when to call it.

not only is this method inefficent if MNI developers want to extend the base instructions but also for creating new ones that don't use MNI.


With PowerMASM, we took the issues of other runtimes and created a new way to define instructions.

PowerMASM uses what we call the ICallable interface.

developers can:
- inherit from the ICallable interface
- fill in the required fields and functions
- write up their instruction in the `call` method

and boom, your instruction that does stack based wizardry is done!

there's no limit to how many instructions you can define inside a Addon or "Library" for PowerMASM.
