using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.Core.Masibase
{
    /// Masi files have a specific structure:
    /// 
    /// Header (16 bytes): encodes metadata about the MASI file, such as version, entry point, etc.
    /// Import Table: lists external dependencies and their addresses within the MASI file.
    /// Local Variable Table: lists local variables used in the program.
    /// Label Table: lists labels used for jumps and branches in the code and their address within the file.
    /// Constant Table: lists constants used in the program.
    /// Code Section: contains the actual bytecode instructions to be executed.
    /// Data Section: contains static data used by the program.
    /// Export Table: lists functions and variables that can be accessed from outside the MASI file.
    /// Note: The exact structure and size of each section may vary based on the MASI file version and the specific program.
    /// though, this is subject to change as the MASI format evolves.
    /// 
    /// another thing to note is that instructions have to also handle # and $ for label addresing and accessing memory.
    /// 
    /// each instruction is one byte, followed by a byte for if it's a memory access or immediate value, followed by the value itself
    /// either as a 4 byte integer, reference to a label, register, etc.
    /// 
    /// 
    ///
    /// 

    /// <summary>
    /// Provides definitions and mappings for MASI virtual machine instructions, including their names and corresponding
    /// opcode values.Masi files have a specific structure:
    /// </summary>
    /// <remarks>This class serves as a central reference for MASI instruction opcodes and their string
    /// representations. It is intended for use when encoding, decoding, or interpreting MASI bytecode. The instruction
    /// set may evolve as the MASI format changes; ensure compatibility with the MASI file version in use.</remarks>
    public class MasiInstructions
    {

        public static Dictionary<string, byte> InstructionSet { get; set; } = new Dictionary<string, byte>
        {
            // Data Movement
            { "MOV", 0x01 },
            { "PUSH", 0x02 },
            { "POP", 0x03 },
            //{ "LOAD", 0x04 },
            //{ "STORE", 0x05 },
            // Arithmetic Operations
            { "ADD", 0x10 },
            { "SUB", 0x11 },
            { "MUL", 0x12 },
            { "DIV", 0x13 },
            { "INC", 0x14 },
            { "DEC", 0x15 },
            // Logical Operations
            { "AND", 0x20 },
            { "OR", 0x21 },
            { "XOR", 0x22 },
            { "NOT", 0x23 },
            // Control Flow
            { "JMP", 0x30 },
            { "JE", 0x31 },
            { "JNE", 0x32 },
            { "JG", 0x33 },
            { "JL", 0x34 },
            { "CALL", 0x35 },
            { "RET", 0x36 },
            // Comparison
            { "CMP", 0x40 },
            // System Operations
            { "NOP", 0x50 },
            { "HLT", 0x51 },
            { "LBL", 0x52 }
        };
        public MasiInstructions()
        {

        }
        public static byte GetOpcode(string instruction)
        {
            if (InstructionSet.ContainsKey(instruction.ToUpper()))
                return InstructionSet[instruction.ToUpper()];
            throw new ArgumentException($"Invalid instruction: {instruction}");
        }
        public static string GetInstruction(byte opcode)
        {
            var instruction = InstructionSet.FirstOrDefault(x => x.Value == opcode);
            if (!string.IsNullOrEmpty(instruction.Key))
                return instruction.Key;
            throw new ArgumentException($"Invalid opcode: {opcode}");
        }
        public static void PrintInstructionSet()
        {
            Console.WriteLine("MASI Instruction Set:");
            foreach (var instr in InstructionSet)
            {
                Console.WriteLine($"{instr.Key}: 0x{instr.Value:X2}");
            }
        }

        public static bool IsValidInstruction(string instruction)
        {
            return InstructionSet.ContainsKey(instruction.ToUpper());
        }

        public static bool IsValidOpcode(byte opcode)
        {
            return InstructionSet.ContainsValue(opcode);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("MASI Instruction Set:");
            foreach (var instr in InstructionSet)
            {
                sb.AppendLine($"{instr.Key}: 0x{instr.Value:X2}");
            }
            return sb.ToString();
        }

        public static int InstructionCount()
        {
            return InstructionSet.Count;
        }

        public static List<string> GetAllInstructions()
        {
            return InstructionSet.Keys.ToList();
        }

        public static List<byte> GetAllOpcodes()
        {
            return InstructionSet.Values.ToList();
        }

        public static void AddInstruction(string instruction, byte opcode)
        {
            if (IsValidInstruction(instruction))
                throw new ArgumentException($"Instruction already exists: {instruction}");
            if (IsValidOpcode(opcode))
                throw new ArgumentException($"Opcode already exists: 0x{opcode:X2}");
            InstructionSet.Add(instruction.ToUpper(), opcode);
        }
        public static void RemoveInstruction(string instruction)
        {
            if (!IsValidInstruction(instruction))
                throw new ArgumentException($"Instruction does not exist: {instruction}");
            InstructionSet.Remove(instruction.ToUpper());


        }
        public static void RemoveInstruction(byte opcode)
        {
            var instruction = InstructionSet.FirstOrDefault(x => x.Value == opcode);
            if (string.IsNullOrEmpty(instruction.Key))
                throw new ArgumentException($"Opcode does not exist: 0x{opcode:X2}");
            InstructionSet.Remove(instruction.Key);
        }
   
    }
}
