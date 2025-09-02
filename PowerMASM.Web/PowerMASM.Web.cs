using System.Runtime.InteropServices.JavaScript;
namespace PowerMASM.Web
{
    public partial class Program
    {
        public static string Version => "0.9.0";

        [JSExport]
        public static string GetVersion()
        {
            return Version;
        }

        public static void Main(string[] args)
        {

        }
    }
}
