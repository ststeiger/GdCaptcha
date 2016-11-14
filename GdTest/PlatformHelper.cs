
namespace GdTest
{


    class PlatformHelper
    {



        [System.Runtime.InteropServices.DllImport("libc")]
        private static extern int uname(System.IntPtr buf);

        // https://github.com/jpobst/Pinta/blob/master/Pinta.Core/Managers/SystemManager.cs
        private static bool IsRunningOnMac()
        {
            System.IntPtr buf = System.IntPtr.Zero;
            try
            {
                buf = System.Runtime.InteropServices.Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname ()
                if (uname(buf) == 0)
                {
                    string os = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin")
                        return true;
                }
            }
            catch
            {
            }
            finally
            {
                if (buf != System.IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(buf);
            }
            return false;
        } // End Function IsRunningOnMac


        public static void GetDll()
        {
            bool isWindows = false;
            bool isLinux = false;
            bool isMac = false;

            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                if (IsRunningOnMac())
                    isMac = true;
                else
                    isLinux = true;
            }
            else
                isWindows = true;

            if (isLinux)
            {
                if (PlatformLoader.CanLoadSharedObject("libgd.so"))
                    return;
            }

            if (isWindows)
            {
                if (PlatformLoader.CanLoadSharedObject("libgd.dll"))
                    return;
            }

            if (isMac)
            {
                if (PlatformLoader.CanLoadSharedObject("libgd.dylib"))
                    return;
            }

            string platform = "windows";
            string extension = "dll";
            string architecture = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

            if (System.StringComparer.OrdinalIgnoreCase.Equals(architecture, "amd64"))
                architecture = "x86";


            if (isLinux)
            {
                platform = "linux";
                extension = "so";
            }

            if (isMac)
            {
                platform = "mac";
                extension = "dylib";
            }



            int bitNess = System.IntPtr.Size * 8;
            System.Reflection.Assembly ass = typeof(MainClass).Assembly;
            string resToCopy = null;

            string searchedResource = "runtimes." + platform + "_" + architecture + "_" + bitNess.ToString() + ".libgd." + extension;

            foreach (string resName in ass.GetManifestResourceNames())
            {
                // GdTest.runtimes.linux_x86_64.libgd.so
                // GdTest.runtimes.windows_x86_64.libgd.dll
                // GdTest.runtimes.windows_x86_32.libgd.dll
                if (resName.EndsWith(searchedResource))
                {
                    resToCopy = resName;
                    break;
                }
            }

            if (resToCopy == null)
                return;


            string libGD = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ass.Location), "libgd." + extension);

            using (System.IO.Stream inputStream = ass.GetManifestResourceStream(resToCopy))
            {
                using (System.IO.FileStream outputStream = System.IO.File.OpenWrite(libGD))
                {
                    byte[] buffer = new byte[8192];

                    int bytesRead;
                    while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                    } // Whend 

                    outputStream.Flush();
                } // End Using outputStream 

            } // End using inputStream 

        } // End Sub GetDll 


    } // End Class 


} // End Namespace 
