using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace GraphicalVersion
{
    /// <summary>
    /// Author: Jervis Muindi    
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Make sure that the required DLL files exist before proceeding further.
            // If DLL, not found - show user error msg box and quit

            if (!DLLExist())
            {
                string  msg = "A required dll was not found. Please make sure that both \n";
                        msg += "StrStorage.dll and dynapdf.dll are in the same folder as this program\n";
                        msg += "and try again.\n";

                MessageBox.Show(msg, "A Required DLL Was Not Found");

                return; // Don't Launch the application
            }



            Application.Run(new Form1());
        }


        /// <summary>
        /// Determines if one of the required DLLs is not in the current directory.
        /// The dlls in quesiton are StrStorage.dll and dynapdf.dll
        /// </summary>
        /// <returns></returns>
        static bool DLLExist(){

            FileInfo strstor_dll = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "StrStorage.dll"));
            FileInfo dynapdf_dll = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "dynapdf.dll"));

            if(!strstor_dll.Exists || !dynapdf_dll.Exists){ // if either required DLL is missing
                return false;
            }
            else {
                return true;
            }
        }
    }
}
