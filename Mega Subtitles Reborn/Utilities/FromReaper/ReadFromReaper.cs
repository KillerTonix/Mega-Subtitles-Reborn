using System.IO;
using System.Reflection;
using System.Text;

namespace Mega_Subtitles_Reborn.Utilitis.FromReaper
{
   public class ReadFromReaper
    {
        public static void ReadProjectName()
        {
            string ProjectName = ReadReaperProjectName();

            if (ProjectName != string.Empty)
            {
                GeneralSettings.Default.CurrentProjectName = ProjectName;
                GeneralSettings.Default.Save();
            }
            /*else
                throw new Exception("Project Name does not exists\nPlease one more time open Reaper Project and run script.");
            */
        }

        public static string ReadReaperProjectName()
        {           
            try
            {
                if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(GeneralSettings.Default.CurrentProjectNameFilePath))
                    return File.ReadAllText(GeneralSettings.Default.CurrentProjectNameFilePath, Encoding.UTF8);
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.ExceptionLogger.LogException(ex, "FileReader", MethodBase.GetCurrentMethod()?.Name);
                return string.Empty;
            }
        }

        
    }
}
