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
using PowerMASM;

namespace PowerMASM.UIDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

    private MASMCore _core;
    private MicroAsmVmState _state;
    public class RegisterViewModel { public string? Name { get; set; } public string? Value { get; set; } }
    public class StackViewModel { public string? Address { get; set; } public string? Value { get; set; } }

        public MainWindow()
        {
            InitializeComponent();
            var Context = this.DataContext;

        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            // Get code from MasmEditor (assume it has a Text property or similar)
            string code = string.Empty;
            if (this.FindName("Editor") is FrameworkElement editor)
            {
                var textProp = editor.GetType().GetProperty("Text");
                if (textProp != null)
                {
                    code = textProp.GetValue(editor) as string ?? string.Empty;
                }
                else
                {
                    // fallback: try Document property (for RichTextBox-like controls)
                    var docProp = editor.GetType().GetProperty("Document");
                    if (docProp != null)
                    {
                        var doc = docProp.GetValue(editor);
                        if (doc != null)
                        {
                            var contentStart = doc.GetType().GetProperty("ContentStart")?.GetValue(doc);
                            var contentEnd = doc.GetType().GetProperty("ContentEnd")?.GetValue(doc);
                            if (contentStart != null && contentEnd != null)
                            {
                                code = new TextRange((TextPointer)contentStart, (TextPointer)contentEnd).Text;
                            }
                        }
                    }
                }
            }

            // Preprocess and run
            _core = MASMCore.PreProcess(code);
            _core.Run();
            _state = _core.GetType().GetProperty("_state", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_core) as MicroAsmVmState;
            UpdateRegisterView();
            UpdateStackView();
            if (this.FindName("Output") is TextBlock outputBlock)
            {
                if (_state != null && _state.Exceptions != null && _state.Exceptions.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Program executed with errors:");
                    foreach (var ex in _state.Exceptions)
                    {
                        sb.AppendLine(ex.ToString());
                    }
                    outputBlock.Text = sb.ToString();
                }
                else
                {
                    outputBlock.Text = "Program executed successfully.";
                }
            }
        }

        private void UpdateRegisterView()
        {
            if (_state == null) return;
            var regs = new[] { "RAX", "RBX", "RCX", "RDX", "RSI", "RDI", "RSP", "RBP", "RIP" };
            var regList = new System.Collections.Generic.List<RegisterViewModel>();
            foreach (var reg in regs)
            {
                regList.Add(new RegisterViewModel { Name = reg, Value = _state.GetIntRegister(reg).ToString() });
            }
            if (this.FindName("RegisterGrid") is DataGrid regGrid)
                regGrid.ItemsSource = regList;
        }

        private void UpdateStackView()
        {
            if (_state == null) return;
            var stackList = new System.Collections.Generic.List<StackViewModel>();
            long rsp = _state.GetIntRegister("RSP");
            for (int i = 0; i < 16; i++)
            {
                long addr = rsp + i * 8;
                if (addr < _state.Memory.Length)
                {
                    var bytes = _state.Memory.Slice((int)addr, 8).ToArray();
                    long val = BitConverter.ToInt64(bytes, 0);
                    stackList.Add(new StackViewModel { Address = $"0x{addr:X4}", Value = val.ToString() });
                }
            }
            if (this.FindName("StackGrid") is DataGrid stackGrid)
                stackGrid.ItemsSource = stackList;
        }
    }
}