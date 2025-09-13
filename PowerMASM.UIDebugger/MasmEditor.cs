// Add this file to your solution and reference it in your MainWindow.xaml
// This is a basic AvalonEdit-based MASM editor with syntax highlighting for MicroASM

using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.IO;
using System.Reflection;
using System.Xml;

namespace PowerMASM.UIDebugger
{
    public class MasmEditor : System.Windows.Controls.UserControl
    {
        public TextEditor Editor { get; private set; }
        public MasmEditor()
        {
            Editor = new TextEditor
            {
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 16,
                ShowLineNumbers = true,
                Background = System.Windows.Media.Brushes.Black,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(8),
                SyntaxHighlighting = LoadHighlighting()
            };
            Content = Editor;
        }

        private IHighlightingDefinition LoadHighlighting()
        {
            // Embedded resource: MasmHighlighting.xshd
            // var assembly = Assembly.GetExecutingAssembly();
            // using (Stream s = assembly.GetManifestResourceStream("PowerMASM.UIDebugger.MasmHighlighting.xshd"))
            // using (XmlReader reader = XmlReader.Create(s))
            // {
            //     return HighlightingLoader.Load(reader, HighlightingManager.Instance);
            // }
            return null; // Placeholder if the resource is not available
        }
    }
}
