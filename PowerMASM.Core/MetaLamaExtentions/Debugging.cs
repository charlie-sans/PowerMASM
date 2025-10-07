using Metalama.Framework.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.Core.MetaLamaExtentions
{
    public class Debugging
    {
        public static bool EnableDebugging { get; set; } = true;
        public static Debugging DebuggerInstance { get; set; }
        public Debugging() {
            EnableDebugging = false;
            DebuggerInstance = this;

        }
        public static Debugging GetDebuggingInstance()
        {
            if (DebuggerInstance == null)
            {
                DebuggerInstance = new Debugging();
            }
            return DebuggerInstance;
        }
        public class DebugInfo
        {
            public string FileName { get; set; }
            public int LineNumber { get; set; }
            public int ColumnNumber { get; set; }
            public string CodeLine { get; set; }
            public DebugInfo(string fileName, int lineNumber, int columnNumber, string codeLine)
            {
                FileName = fileName;
                LineNumber = lineNumber;
                ColumnNumber = columnNumber;
                CodeLine = codeLine;
            }
            public override string ToString()
            {
                return $"{FileName}({LineNumber},{ColumnNumber}): {CodeLine}";
            }
        }
        public class DebugInfoCollection
        {
            private List<DebugInfo> debugInfos;
            public DebugInfoCollection()
            {
                debugInfos = new List<DebugInfo>();
            }
            public void Add(DebugInfo info)
            {
                debugInfos.Add(info);
            }
            public void Clear()
            {
                debugInfos.Clear();
            }
            public IEnumerable<DebugInfo> GetAll()
            {
                return debugInfos;
            }
            public override string ToString()
            {
                var sb = new StringBuilder();
                foreach (var info in debugInfos)
                {
                    sb.AppendLine(info.ToString());
                }
                return sb.ToString();
            }
        }
        public class Debugger
        {
            private DebugInfoCollection debugInfoCollection;
            public Debugger()
            {
                debugInfoCollection = new DebugInfoCollection();
            }
            public void Log(string fileName, int lineNumber, int columnNumber, string codeLine)
            {
                var info = new DebugInfo(fileName, lineNumber, columnNumber, codeLine);
                debugInfoCollection.Add(info);
            }
            public void ClearLogs()
            {
                debugInfoCollection.Clear();
            }
            public IEnumerable<DebugInfo> GetLogs()
            {
                return debugInfoCollection.GetAll();
            }
            public override string ToString()
            {
                return debugInfoCollection.ToString();
            }
        }
        
    }
    public class IDebuggable : OverrideMethodAspect
    {
        public override dynamic? OverrideMethod()
        {
            try
            {
                if (Debugging.EnableDebugging)
                {
                    Console.WriteLine($"Entering method: {meta.Target.Method.Name}");
                    var result = meta.Proceed();
                    Console.WriteLine($"Exiting method: {meta.Target.Method.Name}");
                    return result;
                }
                else
                {
                    return meta.Proceed();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in method: {meta.Target.Method.Name} - {ex.Message}");
                throw;
            }
        }
    }
}