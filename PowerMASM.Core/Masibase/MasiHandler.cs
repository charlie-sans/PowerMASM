using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.Numerics;
namespace PowerMASM.Core.Masibase
{
    /*
    /// the MasiHandler is responsible for handling MASI files.
    /// Masi files are the compiled version of MASM files with a .masi extension.
    /// Masi files contain bytecode that can be executed by the PowerMASM runtime.
    /// 
    /// Masi files have a specific structure:
    /// 
    /// Header (24 bytes): encodes metadata about the MASI file, such as version, entry point, etc.
    /// Import Table: lists external dependencies and their addresses within the MASI file.
    /// Local Variable Table: lists local variables used in the program.
    /// Label Table: lists labels used for jumps and branches in the code and their address within the file.
    /// Constant Table: lists constants used in the program.
    /// Code Section: contains the actual bytecode instructions to be executed.
    /// Data Section: contains static data used by the program.
    /// Export Table: lists functions and variables that can be accessed from outside the MASI file.
    /// Note: The exact structure and size of each section may vary based on the MASI file version and the specific program.
    /// though, this is subject to change as the MASI format evolves.
    */
    public class MasiHandler
    {
        public byte[] MasiData { get; set; }
        public string MasiFilePath { get; set; }
        public string MasiFileName { get; set; }
        public MasiHandler()
        {
        }
        public MasiHandler(string filePath)
        {
            LoadMasiFile(filePath);
        }

        public void LoadMasiFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                throw new System.IO.FileNotFoundException($"Masi file not found: {filePath}");
            MasiData = System.IO.File.ReadAllBytes(filePath);
            MasiFilePath = filePath;
            MasiFileName = System.IO.Path.GetFileName(filePath);
        }
        public void UnloadMasiFile()
        {
            MasiData = null;
            MasiFilePath = null;
            MasiFileName = null;
        }
        public override string ToString()
        {
            return $"MasiHandler: {MasiFileName} ({(MasiData != null ? MasiData.Length : 0)} bytes)";
        }
        public void PrintMasiDataHex()
        {
            if (MasiData == null)
            {
                Console.WriteLine("No MASI data loaded.");
                return;
            }
            Console.WriteLine(BitConverter.ToString(MasiData).Replace("-", " "));
        }


        public MasiHandler Compile(string masmSource)
        {
           
             byte[] header = new byte[24]; // 24-byte header 
            


            return this;
        }

        public string ParseMasiData()
        {
            if (MasiData == null)
                return null;
            // now here's the tricky part, we need to read the byte array, parse the headers, program table ect.

            // presuming the person called the load function first.
            // build the file back into a string.
            StringBuilder sb = new StringBuilder();

            return sb.ToString();


        }
    }

}
