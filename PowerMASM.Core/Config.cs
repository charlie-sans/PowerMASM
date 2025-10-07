using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;
namespace PowerMASM.Core;

[JsonSerializable(typeof(Config))]
public class Config
{
    public int StackSize { get; set; } = 32768; // 32KB
    public int HeapSize { get; set; } = 32768; // 32KB
    public int DataSize { get; set; } = 32768; // 32KB
    public int CodeSize { get; set; } = 32768; // 32KB
    public int memSize { get; set; } = 131072; // 128KB 
    public bool EnableDebugging { get; set; } = false;
    public bool EnableOptimizations { get; set; } = true;
    public bool EnableWarningsAsErrors { get; set; } = false;
    public List<string> IncludePaths { get; set; } = new List<string>();
    public List<string> LibraryPaths { get; set; } = new List<string>();
    public List<string> PredefinedMacros { get; set; } = new List<string>();
    public static Config Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Config file not found: {filePath}");
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Config>(json);
    }
    public void Save(string filePath)
    {
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}
