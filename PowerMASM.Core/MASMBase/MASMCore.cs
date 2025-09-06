using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;

namespace PowerMASM.Core.MASMBase;
public class MASMCore
{
    public List<string> Instructions { get; set; } = null;
    public MASMLabel[] Labels { get; set; } = null;
    private MicroAsmVmState _state { get; set; } = null;
    private static MASMCore _coreSelf { get; set; } = null;
    public MASMCore(bool? create = true)
    {
        if (create == true)
        {
            _state = new MicroAsmVmState();
        }
        _coreSelf = this; // reference to self, hopefully...
    }


    public void Run()
    {
        if (_state == null)
            _state = new MicroAsmVmState();
        _state.Exceptions.Clear();
        var instructions = Instructions ?? new List<string>();
        var collector = new PowerMASM.Core.Interfaces.ICallableCollector();
        collector.Collect();
        for (int i = 0; i < instructions.Count; i++)
        {
            var instr = instructions[i];
            try
            {
                var parts = instr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;
                var name = parts[0];
                var callable = collector.GetCallableByName(name);
                var paramCount = callable.ParameterCount;
                if (parts.Length - 1 < paramCount)
                    throw new Exception($"Instruction '{name}' expects {paramCount} parameters, got {parts.Length - 1}.");
                var parameters = parts.Skip(1).Take(paramCount).ToArray();
                callable.Call(_state, parameters);
            }
            catch (Exception ex)
            {
                _state.Exceptions.Add(new PowerMASM.Core.MASMException.MASMException($"Error at instruction {i}: {instr}", i, instr, ex));
            }
        }
    }

    public static MASMCore PreProcess(string code)
    {
        // Split code into lines, trim whitespace, and remove comments
        var lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Trim())
                        .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith(";")) // Remove comments and empty lines
                        .ToArray();

        var labels = new List<MASMLabel>();
        var globalInstructions = new List<string>();
        MASMLabel currentLabel = null;
        List<string> currentInstructions = null;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.StartsWith("#include", StringComparison.OrdinalIgnoreCase))
            {
                string domainBaseDir = AppDomain.CurrentDomain.BaseDirectory;
                var start = line.IndexOf('"') != -1 ? line.IndexOf('"') : line.IndexOf('<');
                var end = line.IndexOf('"', start + 1) != -1 ? line.IndexOf('"', start + 1) : line.IndexOf('>', start + 1);
                if (start != -1 && end != -1 && end > start)
                {
                    var includePath = line.Substring(start + 1, end - start - 1).Replace('.', System.IO.Path.DirectorySeparatorChar);
                    var fullPath = System.IO.Path.Combine(domainBaseDir, includePath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        var includedCode = System.IO.File.ReadAllText(fullPath);
                        var includedCore = PreProcess(includedCode);
                        if (includedCore.Labels != null)
                            labels.AddRange(includedCore.Labels);
                        if (includedCore.Instructions != null)
                            globalInstructions.AddRange(includedCore.Instructions);
                    }
                }
            }
            else if (line.EndsWith(":") || line.StartsWith("lbl ", StringComparison.OrdinalIgnoreCase))
            {
                // Save previous label if exists
                if (currentLabel != null)
                {
                    currentLabel.Instructions = currentInstructions?.ToArray();
                    labels.Add(currentLabel);
                }
                string labelName;
                if (line.EndsWith(":"))
                    labelName = line.TrimEnd(':').Trim();
                else
                    labelName = line.Substring(4).Trim(); // after "lbl "
                currentLabel = new MASMLabel { Name = labelName, Instructions = null, modifiers = null };
                currentInstructions = new List<string>();
            }
            else // Regular instruction
            {
                if (currentLabel != null)
                    currentInstructions.Add(line);
                else
                    globalInstructions.Add(line);
            }
        }
        // Add last label if exists
        if (currentLabel != null)
        {
            currentLabel.Instructions = currentInstructions?.ToArray();
            labels.Add(currentLabel);
        }
        _coreSelf = new MASMCore(false)
        {
            Labels = labels.Count > 0 ? labels.ToArray() : null,
            Instructions = globalInstructions.Count > 0 ? globalInstructions : null
        };
        return _coreSelf;
    }
}
