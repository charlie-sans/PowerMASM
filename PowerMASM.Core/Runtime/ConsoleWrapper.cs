using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerMASM.Core.Runtime
{
    public class ConsoleInWrapper: TextReader
    {
        private readonly Queue<string> _inputBuffer = new();
        public ConsoleInWrapper() { }
        public void PushInput(string value)
        {
            _inputBuffer.Enqueue(value);
        }
        
        public string ReadLine()
        {
            return _inputBuffer.Count > 0 ? _inputBuffer.Dequeue() : string.Empty;
        }

        public async Task<string> ReadLineAsync()
        {
            // For future async input scenarios
            return await Task.FromResult(ReadLine());
        }
    }

    public class ConsoleOutWrapper: TextWriter
    {
        private readonly List<string> _outputBuffer = new();


        public IReadOnlyList<string> Output => _outputBuffer;

        public override Encoding Encoding => Encoding.UTF8;

        public void WriteLine(string value)
        {
            _outputBuffer.Add(value);
        }

        public void Write(string value)
        {
            if (_outputBuffer.Count == 0)
                _outputBuffer.Add("");
            _outputBuffer[_outputBuffer.Count - 1] += value;
        }

        public void Clear()
        {
            _outputBuffer.Clear();
        }

   
    }
}
