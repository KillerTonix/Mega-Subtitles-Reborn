using Mega_Subtitles_Reborn.Utilitis.FromReaper;
using System.IO;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilitis.PreRun
{
    public class PreRunCheckAndRealize
    {
        private static readonly string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string ReaperSourcePath = Path.Combine(ApplicationPath, "Cache");
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        private static readonly string CurrentProjectName = mainWindow.CurrentProjectName;
        private static readonly string CacheFolderPath = Path.Combine(ReaperSourcePath, CurrentProjectName);


        public static void CheckAndRealize()
        {
            DirectoryOrFileCheck.DirectoryCheck(CacheFolderPath);
            ReadFromReaper.ReadProjectName();
        }
    }
}
