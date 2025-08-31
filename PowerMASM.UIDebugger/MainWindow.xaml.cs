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
using PowerMASM.MASM;
namespace PowerMASM.UIDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var Context = this.DataContext;

        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("RunButton_Click");
        }
    }
}