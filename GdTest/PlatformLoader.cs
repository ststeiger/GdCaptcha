
using System.Collections.Generic;
using System.Text;

namespace GdTest
{


    // Hinweis: Anweisungen zum Aktivieren des klassischen Modus von IIS6 oder IIS7 
    // finden Sie unter "http://go.microsoft.com/?LinkId=9394801".
    public class PlatformLoader 
    {

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        static extern System.IntPtr LoadLibrary(string lpFileName);

        [System.Runtime.InteropServices.DllImport("kernel32", CharSet = System.Runtime.InteropServices.CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern System.UIntPtr GetProcAddress(System.IntPtr hModule, string procName);

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool FreeLibrary(System.IntPtr hModule);


        // See http://mpi4py.googlecode.com/svn/trunk/src/dynload.h
        const int RTLD_LAZY = 1; // for dlopen's flags
        const int RTLD_NOW = 2; // for dlopen's flags

        [System.Runtime.InteropServices.DllImport("libdl")]
        static extern System.IntPtr dlopen(string filename, int flags);

        [System.Runtime.InteropServices.DllImport("libdl")]
        static extern System.IntPtr dlsym(System.IntPtr handle, string symbol);

        [System.Runtime.InteropServices.DllImport("libdl")]
        static extern int dlclose(System.IntPtr handle);

        [System.Runtime.InteropServices.DllImport("libdl")]
        static extern string dlerror();




        public static bool CanLoadSharedObject(string strFileName)
        {
            System.IntPtr hSO = System.IntPtr.Zero;

            try
            {

                if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                {
                    hSO = dlopen(strFileName, RTLD_NOW);
                }
                else
                {
                    hSO = LoadLibrary(strFileName);

                } // End if (Environment.OSVersion.Platform == PlatformID.Unix)

            } // End Try
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            } // End Catch

            if (hSO == System.IntPtr.Zero)
                return false;

            return true;
        } // End Sub LoadSharedObject


        public static void LoadSharedObject(string strFileName)
        {
            System.IntPtr hSO = System.IntPtr.Zero;

            try
            {

                if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                {
                    hSO = dlopen(strFileName, RTLD_NOW);
                }
                else
                {
                    hSO = LoadLibrary(strFileName);

                } // End if (Environment.OSVersion.Platform == PlatformID.Unix)

            } // End Try
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            } // End Catch

            if (hSO == System.IntPtr.Zero)
            {
                throw new System.ApplicationException("Cannot open " + strFileName);
            } // End if (hExe == IntPtr.Zero)

        } // End Sub LoadSharedObject


        // http://stackoverflow.com/questions/281145/asp-net-hostingenvironment-shadowcopybinassemblies
        public static void EnsureOracleDllsLoaded()
        {
            int iBitNess = System.IntPtr.Size * 8;

            string strTargetDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
            strTargetDirectory = System.IO.Path.GetDirectoryName(strTargetDirectory);

            string strSourcePath = "~/bin/dependencies/InstantClient/";

            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                strSourcePath += "Linux" + iBitNess.ToString();
            }
            else
            {
                strSourcePath += "Win" + iBitNess.ToString();
            }

            strSourcePath = ""; // Server.MapPath(strSourcePath);

            string[] astrAllFiles = System.IO.Directory.GetFiles(strSourcePath, "*.dll");


            foreach (string strSourceFile in astrAllFiles)
            {
                string strTargetFile = System.IO.Path.GetFileName(strSourceFile);
                strTargetFile = System.IO.Path.Combine(strTargetDirectory, strTargetFile);
                System.IO.File.Copy(strSourceFile, strTargetFile, true);

                
                string RegexDirSeparator = System.Text.RegularExpressions.Regex.Escape(System.IO.Path.DirectorySeparatorChar.ToString());
                //if(strTargetFile.EndsWith("orannzsbb11.dll", StringComparison.OrdinalIgnoreCase))
                if (System.Text.RegularExpressions.Regex.IsMatch(strTargetFile, @"^(.*" + RegexDirSeparator + @")?orannzsbb11\.(dll|so|dylib)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    continue; // Unneeded exception thrower

                try
                {
                    LoadSharedObject(strTargetFile);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }

            } // Next strSourceFile

        } // End Sub EnsureOracleDllsLoaded


    } // End Class MvcApplication : System.Web.HttpApplication


} // End Namespace 
