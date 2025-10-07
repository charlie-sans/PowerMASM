using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using PowerMASM.Core;
using System.Text.RegularExpressions;

namespace PowerMASM.Core.MASMBase;
public class MASMCore
{
    public List<string> Instructions { get; set; } = null;
    public string[] AllInstructions { get; set; } = null; // <-- Added property
    public Config Config { get; set; } = new Config();
    public MASMLabel[] Labels { get; set; } = null;
    public List<MASMLabel> Functions { get; set; } = null; // Store parsed fdef functions
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
        Console.WriteLine("meow");
    }
    public MASMLabel? GetFunction(string name)
    {
        if (Functions != null)
            return Functions.FirstOrDefault(f => f.Name == name);
        return null;
    }

    public static MASMCore PreProcess(string code)
    {
        // Split code into lines, trim whitespace, and remove comments
        var lines = code
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(';')[0].Trim()) // Remove comments
            .Where(line => !string.IsNullOrWhiteSpace(line)) // Remove empty lines
            .ToArray();

        var labels = new List<MASMLabel>();
        var globalInstructions = new List<string>();
        var functions = new List<MASMLabel>();
        MASMLabel currentLabel = null;
        List<string> currentInstructions = null;

        var dataLabelRegex = new Regex(@"^(\w+):\s*(DB|DW|DD|DQ|DF|DDbl|RESB|RESW|RESD|RESQ|RESF|RESDbl)\s+(.+)$", RegexOptions.IgnoreCase);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (dataLabelRegex.IsMatch(line))
            {
                var match = dataLabelRegex.Match(line);
                var labelName = match.Groups[1].Value;
                var directive = match.Groups[2].Value.ToUpper();
                var data = match.Groups[3].Value;
                var dataLabel = new MASMLabel
                {
                    Name = labelName,
                    DataDirective = directive,
                    DataValues = data,
                    Instructions = null,
                    modifiers = null
                };
                labels.Add(dataLabel);
                continue; // Skip to next line
            }
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
                        if (includedCore.Functions != null)
                            functions.AddRange(includedCore.Functions);
                    }
                }
            }
            else if (line.StartsWith("fdef ", StringComparison.OrdinalIgnoreCase))
            {
                // Parse fdef signature: fdef <type> <name>(<args>) -> <return type>
                // Example: fdef int Add(int a, int b) -> int
                var signature = line.Substring(5).Trim();
                var arrowIdx = signature.IndexOf("->");
                string returnType = null;
                if (arrowIdx != -1)
                {
                    returnType = signature.Substring(arrowIdx + 2).Trim();
                    signature = signature.Substring(0, arrowIdx).Trim();
                }
                // Now signature: <type> <name>(<args>)
                var parenStart = signature.IndexOf('(');
                var parenEnd = signature.IndexOf(')');
                if (parenStart == -1 || parenEnd == -1 || parenEnd < parenStart)
                    continue; // Invalid signature, skip
                var funcNameType = signature.Substring(0, parenStart).Trim();
                var argsStr = signature.Substring(parenStart + 1, parenEnd - parenStart - 1).Trim();
                // funcNameType: <type> <name>
                var funcParts = funcNameType.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (funcParts.Length < 2)
                    continue; // Invalid, skip
                var funcType = funcParts[0];
                var funcName = funcParts[1];
                // Parse arguments
                var parameters = new List<(string Type, string Name)>();
                if (!string.IsNullOrWhiteSpace(argsStr))
                {
                    var argList = argsStr.Split(',');
                    foreach (var arg in argList)
                    {
                        var argParts = arg.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (argParts.Length == 2)
                            parameters.Add((argParts[0], argParts[1]));
                    }
                }
                // Parse function body (lines until next fdef/label/EOF)
                var funcInstructions = new List<string>();
                i++;
                while (i < lines.Length)
                {
                    var bodyLine = lines[i];
                    if (bodyLine.StartsWith("fdef ", StringComparison.OrdinalIgnoreCase) ||
                        bodyLine.EndsWith(":") ||
                        bodyLine.StartsWith("lbl ", StringComparison.OrdinalIgnoreCase))
                    {
                        i--; // step back so outer loop can process this line
                        break;
                    }
                    funcInstructions.Add(bodyLine);
                    i++;
                }
                var funcLabel = new MASMLabel
                {
                    Name = funcName,
                    Instructions = funcInstructions.ToArray(),
                    IsFunction = true,
                    ReturnType = returnType ?? funcType,
                    Parameters = parameters,
                    Exported = true
                };
                functions.Add(funcLabel);
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
        var core = new MASMCore(false)
        {
            Labels = labels.Count > 0 ? labels.ToArray() : null,
            Instructions = globalInstructions.Count > 0 ? globalInstructions : null,
            Functions = functions.Count > 0 ? functions : null
        };
        core.AllInstructions = lines; // <-- Assign all lines
        return core;
    }
}
