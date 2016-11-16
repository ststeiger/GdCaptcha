
namespace GdTest
{


    class FileHelper
    {


        public static string MapProjectPath(string projectPath)
        {
            string dir = System.IO.Path.GetDirectoryName(typeof(FileHelper).Assembly.Location);
            dir = System.IO.Path.Combine(dir, "../..");
            dir = System.IO.Path.GetFullPath(dir);
            dir = System.IO.Path.Combine(dir, projectPath);
            dir = System.IO.Path.GetFullPath(dir);
            return dir;
        }


    }


}
