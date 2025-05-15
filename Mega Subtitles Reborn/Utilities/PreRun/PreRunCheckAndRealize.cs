using Mega_Subtitles_Reborn.Utilitis.FromReaper;
using System.IO;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilitis.PreRun
{
    public class PreRunCheckAndRealize
    {

        public static void CheckAndRealize()
        {
            AssignVaraiblesToSettings();
            ReadFromReaper.ReadProjectName();
            DirectoryOrFileCheck.DirectoryCheck(GeneralSettings.Default.ProjectCacheFolderPath);
        }

        private static void AssignVaraiblesToSettings()
        {
            GeneralSettings.Default.ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
            GeneralSettings.Default.GeneralCacheFolderPath = Path.Combine(GeneralSettings.Default.ApplicationPath, "Cache");
            GeneralSettings.Default.CurrentProjectNameFilePath = Path.Combine(GeneralSettings.Default.GeneralCacheFolderPath, "CurrentProjectName.txt");
            GeneralSettings.Default.ProjectCacheFolderPath = Path.Combine(GeneralSettings.Default.GeneralCacheFolderPath, GeneralSettings.Default.CurrentProjectName);
            GeneralSettings.Default.ProjectCahceJsonPath = Path.Combine(GeneralSettings.Default.ProjectCacheFolderPath, GeneralSettings.Default.CurrentProjectName) + ".json";
      
           

            GeneralSettings.Default.Save();
        }

    }
}
