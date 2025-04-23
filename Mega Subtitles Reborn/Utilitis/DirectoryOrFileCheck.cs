using System.IO;

namespace Mega_Subtitles_Reborn.Utilitis
{
   public class DirectoryOrFileCheck
    {
        public static void DirectoryCheck(string DirectoryPath)
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
        }

        public static bool CheckFileExistingAndNotEmpty(string FilePath)
        {
            if (File.Exists(FilePath) && (new FileInfo(FilePath).Length > 0))
                return true;
            else
                return false;
        }
    }
}
