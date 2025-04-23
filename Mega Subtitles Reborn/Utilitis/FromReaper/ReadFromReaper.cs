using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilitis.FromReaper
{
   public class ReadFromReaper
    {
        private static readonly string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string ReaperSourcePath = Path.Combine(ApplicationPath, "Cache");
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        private static readonly string CurrentProjectName = Path.Combine(ReaperSourcePath, "CurrentProjectName.txt");

        public static void ReadProjectName()
        {
            string ProjectName = ReadReaperProjectName();
            
            if (ProjectName != string.Empty)
                mainWindow.CurrentProjectName = ProjectName;
            /*else
                throw new Exception("Project Name does not exists\nPlease one more time open Reaper Project and run script.");
      */
        }

        public static string ReadReaperProjectName()
        {
            try
            {
                if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(CurrentProjectName))
                    return File.ReadAllText(CurrentProjectName, Encoding.UTF8);
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.ExceptionLogger.LogException(ex, "FileReader", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return string.Empty;
            }
        }

        
    }
}
