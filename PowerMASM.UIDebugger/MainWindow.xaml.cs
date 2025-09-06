using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PowerMASM.Core.MASMBase;
using PowerMASM.Core;

namespace PowerMASM.UIDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MASMCore _core;
        private MicroAsmVmState _state;

        public MainWindow()
        {
            InitializeComponent();
            _core = new MASMCore();
            _state = _core.GetType().GetProperty("_state", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_core) as MicroAsmVmState;
            if (_state == null)
            {
                _state = new MicroAsmVmState();
            }
            UpdateRegisterView();
            UpdateStackView();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            // Get code from RichTextBox
            string code = "";
            if (this.FindName("Editor") is System.Windows.Controls.RichTextBox rtb)
            {
                code = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
            }
            else
            {
                // fallback: try to find the first RichTextBox
                foreach (var child in LogicalTreeHelper.GetChildren(this))
                {
                    if (child is System.Windows.Controls.RichTextBox box)
                    {
                        code = new TextRange(box.Document.ContentStart, box.Document.ContentEnd).Text;
                        break;
                    }
                }
            }

            // Preprocess and run
            _core = MASMCore.PreProcess(code);
            _core.Run();
            _state = _core.GetType().GetProperty("_state", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_core) as MicroAsmVmState;
            UpdateRegisterView();
            UpdateStackView();
            if (Output != null)
            {
                if (_state != null && _state.Exceptions != null && _state.Exceptions.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Program executed with errors:");
                    foreach (var ex in _state.Exceptions)
                    {
                        sb.AppendLine(ex.ToString());
                    }
                    Output.Text = sb.ToString();
                }
                else
                {
                    Output.Text = "Program executed successfully.";
                }
            }
        }

        private void UpdateRegisterView()
        {
            if (_state == null) return;
            void SetReg(string name, long value)
            {
                if (this.FindName(name) is TextBlock tb)
                    tb.Text = $"{name}: {value}";
            }
            SetReg("RAX", _state.GetIntRegister("RAX"));
            SetReg("RBX", _state.GetIntRegister("RBX"));
            SetReg("RCX", _state.GetIntRegister("RCX"));
            SetReg("RDX", _state.GetIntRegister("RDX"));
            SetReg("RSI", _state.GetIntRegister("RSI"));
            SetReg("RDI", _state.GetIntRegister("RDI"));
            SetReg("RSP", _state.GetIntRegister("RSP"));
            SetReg("RBP", _state.GetIntRegister("RBP"));
            SetReg("RIP", _state.GetIntRegister("RIP"));
        }

        private void UpdateStackView()
        {
            if (_state == null || Stack == null) return;
            var sb = new StringBuilder();
            // Show top 16 stack values
            long rsp = _state.GetIntRegister("RSP");
            for (int i = 0; i < 16; i++)
            {
                long addr = rsp + i * 8;
                if (addr < _state.Memory.Length)
                {
                    var bytes = _state.Memory.Slice((int)addr, 8).ToArray();
                    long val = BitConverter.ToInt64(bytes, 0);
                    sb.AppendLine($"[{addr:X4}]: {val}");
                }
            }
            Stack.Text = sb.ToString();
        }
    }
}