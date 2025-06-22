using Mega_Subtitles_Reborn.Utilitis.FromReaper;
using System.IO;

namespace Mega_Subtitles_Reborn.Utilitis.PreRun
{
    public class PreRunCheckAndRealize
    {
        public static void CheckAndRealize()
        {
            AssignVaraiblesToSettings();            
        }

        private static void AssignVaraiblesToSettings()
        {
            GeneralSettings.Default.ApplicationPath = AppDomain.CurrentDomain.BaseDirectory; // Get the base directory of the application
            GeneralSettings.Default.GeneralCacheFolderPath = Path.Combine(GeneralSettings.Default.ApplicationPath, "Cache"); // Set the path for the general cache folder
            GeneralSettings.Default.CurrentProjectNameFilePath = Path.Combine(GeneralSettings.Default.GeneralCacheFolderPath, "CurrentProjectName.txt"); // Set the path for the current project name file
            GeneralSettings.Default.CurrentProjectName = ReadFromReaper.ReadProjectName(GeneralSettings.Default.CurrentProjectNameFilePath); // Read the current project name from the file
            GeneralSettings.Default.ReaperSyncFilePath = Path.Combine(GeneralSettings.Default.GeneralCacheFolderPath, "SyncForCSharp.txt"); // Set the path for the Reaper sync file
            GeneralSettings.Default.ReaperCommandsFilePath = Path.Combine(GeneralSettings.Default.GeneralCacheFolderPath, "ReaperCommands.json"); // Set the path for the Reaper commands file
            GeneralSettings.Default.ProjectCacheFolderPath = Path.Combine(GeneralSettings.Default.GeneralCacheFolderPath, GeneralSettings.Default.CurrentProjectName); // Set the path for the project cache folder
            GeneralSettings.Default.ProjectCahceJsonPath = Path.Combine(GeneralSettings.Default.ProjectCacheFolderPath, GeneralSettings.Default.CurrentProjectName) + ".json"; // Set the path for the project cache JSON file
            GeneralSettings.Default.DublicateCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mega Subtitles Reborn"); // Set the path for the duplicate cache folder
            GeneralSettings.Default.DublicatedProjectCahceJsonPath = Path.Combine(GeneralSettings.Default.DublicateCachePath, GeneralSettings.Default.CurrentProjectName) + ".json"; // Set the path for the duplicated project cache JSON file
            GeneralSettings.Default.DeletedLinesJsonPath = Path.Combine(GeneralSettings.Default.ProjectCacheFolderPath, "DeletedLines.json"); // Set the path for the deleted lines JSON file
            GeneralSettings.Default.DemoPhrasesPath = Path.Combine(GeneralSettings.Default.ProjectCacheFolderPath, "DemoPhrases.json"); // Set the path for the demo phrases JSON file
            GeneralSettings.Default.ButtonsStatePath = Path.Combine(GeneralSettings.Default.ProjectCacheFolderPath, "ButtonsState.json"); // Set the path for the buttons state JSON file

            GeneralSettings.Default.Save();

            DirectoryOrFileCheck.DirectoryCheck(GeneralSettings.Default.ProjectCacheFolderPath); // Ensure the project cache folder exists
            DirectoryOrFileCheck.DirectoryCheck(GeneralSettings.Default.DublicateCachePath); // Ensure the duplicate cache folder exists
        }

    }
}
