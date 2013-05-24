using System;
using System.Runtime.InteropServices;
using System.IO;

namespace Snp2PdfConverter
{

    /// <summary>
    /// Author: Jervis Muindi
    /// Class that deals with conversion of snapshot files to PDFs. 
    /// </summary>    
    public static class Snapshot
    {
        /// <summary>
        ///  Converts the SNP file in the given directory.
        /// </summary>
        /// <param name="directory">The given directory. It should have a "\" a 
        /// the end or otherwise directory won't be found</param>
        /// <param name="compressedSnapshotName"></param>
        /// <returns></returns>
        public static bool ConvertToPdf(string directory, 
            string compressedSnapshotName)
        {
            //Check that directory exists
            if (!Directory.Exists(directory)) {
                Console.WriteLine("Directory does not exist - " + directory);
                return false;
            }
            Directory.SetCurrentDirectory(directory);

            // Ensure that the file exists
            if (!File.Exists(Path.Combine(directory, compressedSnapshotName)))
            {

                Console.WriteLine("The following file does not exist : " + 
                    Path.Combine(directory, compressedSnapshotName));
                return false;
            }

            string compressedFilepath = 
                Path.Combine(directory, compressedSnapshotName);
            // get the full path of the uncompressed snapshot file.
            string uncompressedSnapshot = 
                GetUncompressedSnapshot(compressedFilepath);  
            bool result = ConvertToPdf(uncompressedSnapshot);

            // remove previous temp file if it exists already. 
            if (File.Exists(uncompressedSnapshot))
            {
                FileInfo fi = new FileInfo(uncompressedSnapshot);
                fi.Delete();
                // Console.WriteLine("Temp File was deleted successfully");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compressedSnapshot"></param>
        /// <returns></returns>
        public static string GetUncompressedSnapshot(string compressedSnapshot) {
            if (!File.Exists(compressedSnapshot)) {
                return null;
            }

            string dir = Path.GetDirectoryName(compressedSnapshot);
            if (dir == null) { return null; }

            string tempName = 
                Path.GetFileNameWithoutExtension(compressedSnapshot) + ".tmp";
            string tempPath = Path.Combine(dir, tempName);

            SetupDecompressOrCopyFile(compressedSnapshot, tempPath, 0);

            //Console.WriteLine("temp path is " + tempPath);

            return tempPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uncompressedSnapshotName"></param>
        /// <returns></returns>
        public static bool ConvertToPdf(string uncompressedSnapshotName) {
            string outputPdf = 
                Path.GetFileNameWithoutExtension(uncompressedSnapshotName) + ".pdf";
            bool r = false;

            try {
                r = ConvertUncompressedSnapshot(uncompressedSnapshotName, 
                    outputPdf, 0, "", "", 0, 0, 0);
            }
            catch (DllNotFoundException) {
                Console.WriteLine(
                    "\nA required DLL was not found. Please make sure that the" +
                    "dynapdf.dll and StrStorage.dll both exist and are in the " +
                    "same folder as this executable and try again\n");
                Environment.Exit(-1);
            }
            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourFileName"></param>
        /// <param name="targetFileName"></param>
        /// <param name="compressionType"></param>
        /// <returns></returns>
        [DllImport("setupapi.dll")]
        public static extern int SetupDecompressOrCopyFile(string sourFileName, 
            string targetFileName, uint compressionType);


        /*
         * In .Net 4.0 Calling the external library causes a stack imbalance error
         * even though the specified parameters are correct for the unmanaged code 
         * function that we're calling. Use .Net 3.5 to get around this issue since
         * in that runtime version, the framework will auto-correct and resolve 
         * this imbalance error. 
         * 
         * See: http://codenition.blogspot.com/2010/05/pinvokestackimbalance-in-net-40i-beg.html
         * 
         */
        // [DllImport("StrStorage.dll")] .Net 3.5

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uncompressedSnapshotName"></param>
        /// <param name="outputPdf"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="passwordOpen"></param>
        /// <param name="passwordOwner"></param>
        /// <param name="passwordRestrictions"></param>
        /// <param name="pdfNoFontEmbedding"></param>
        /// <param name="pdfUnicodeFlags"></param>
        /// <returns></returns>
        // [DllImport("StrStorage.dll", CallingConvention = CallingConvention.Cdecl)]
        [DllImport("StrStorage.dll")]
        public static extern bool 
            ConvertUncompressedSnapshot(String uncompressedSnapshotName,
                                        String outputPdf,
                                        uint compressionLevel,
                                        String passwordOpen,
                                        String passwordOwner,
                                        uint passwordRestrictions,
                                        uint pdfNoFontEmbedding,
                                        uint pdfUnicodeFlags);
    }
}
