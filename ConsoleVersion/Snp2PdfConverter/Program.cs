/* 
 * 
 * Author: Miguel Vrolijk 
 * Date..: 2013-05-19
 * Time..: 10:00
 * 
 * Based on code by: 
 * 
 * Jervis Muindi 
 * Graphical SNP to PDF Converter
 * Columbia College Information Technology
 * Copyright © CCIT 2011
 * 
 */
using System;
using System.IO;
using System.Linq;

namespace Snp2PdfConverter {
    class Program {
        static int Main(string[] args) {
            // Make sure that the required DLL files exist before proceeding further.
            // If DLL, not found - show user error msg box and quit

            if (!DllExist()) {
                string msg = 
                    "A required dll was not found. Please make sure that both \n" +
                    "StrStorage.dll and dynapdf.dll are in the same folder as this program\n" +
                    "and try again.\n";

                Console.WriteLine(msg);

                return 1;
            }

            // Check command line arguments.
            if (args.Length == 0) {
                Console.WriteLine("Directory missing\n\n" +
                    "Usage: Snp2PdfConverter directory (e.g. C:\\Temp)");
                return 1;
            }

            return ConvertPath(args[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static int ConvertPath(string path) {
            if (!Directory.Exists(path)) {
                Console.WriteLine("Directory doesn't exist!");
                return 1;
            }

            string[] files = Directory.GetFiles(path, "*.snp");
            if (files.Length == 0) {
                Console.WriteLine("No snapshots found to convert.");
                return 1;
            }

            return files.Count(file => !ConvertSnpFile(file));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static bool ConvertSnpFile(string file) {
            string name = Path.GetFileName(file);
            string dir = Path.GetDirectoryName(file);

            bool result = Snapshot.ConvertToPdf(dir, name);
            return result;
        }

        /// <summary>
        /// Determines if one of the required DLLs is not in the current directory.
        /// The dlls in quesiton are StrStorage.dll and dynapdf.dll
        /// </summary>
        /// <returns></returns>
        static bool DllExist() {

            var strstorDll = 
                new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), 
                    "StrStorage.dll"));
            var dynapdfDll = 
                new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), 
                    "dynapdf.dll"));

            // if either required DLL is missing.
            if (!strstorDll.Exists || !dynapdfDll.Exists) { 
                return false;
            }
            return true;
        }
    }
}
