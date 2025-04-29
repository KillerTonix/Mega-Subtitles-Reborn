using Mega_Subtitles_Reborn.Utilitis.Subtitles.AssProcessing;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.FileWriter
{
    class CommentsWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void WriteFullComments()
        {
            string OutputPath = Path.Combine(SelectOutputFolder(GetProjectName(), isFullExport: true), $"[{GetProjectName()}] (Full_Comments).txt");

            using StreamWriter writer = new(OutputPath, false, Encoding.UTF8);
            foreach (var item in mainWindow.SubtitleEntries.OrderBy(e => e.Start))
            {
                if (!string.IsNullOrWhiteSpace(item.Comment))
                {                 
                    writer.WriteLine($"{item.Start}\t{item.End}\t{item.Actor}\t{item.Comment}");
                    writer.WriteLine();
                }
            }
            Succses(OutputPath, isFullExport: true);

        }

        public static void WriteSeparatedComments()
        {
            string OutputPath = SelectOutputFolder(GetProjectName(), isFullExport: false);

            var groupedByActor = mainWindow.subtitleViewSource.View.OfType<AssSubtitlesEnteries>().GroupBy(entry => entry.Actor);

            foreach (var actorGroup in groupedByActor)
            {
                // Only create a file if there's at least one comment for the actor
                if (actorGroup.Any(entry => !string.IsNullOrWhiteSpace(entry.Comment)))
                {
                    string actor = actorGroup.Key ?? "_Unknown_Actor_";
                    string filePath = Path.Combine(OutputPath, $"[{actor}] ({GetProjectName()}).txt");

                    using StreamWriter writer = new(filePath, false, Encoding.UTF8);

                    foreach (var item in actorGroup)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Comment))
                        {
                            writer.WriteLine($"{item.Start}\t{item.End}\t{item.Actor}\t{item.Comment}");
                            writer.WriteLine();
                        }
                    }
                }
            }
            Succses(OutputPath, isFullExport: false);
        }

        private static string GetProjectName()
        {
            string CurrentProjectName = GeneralSettings.Default.CurrentProjectName;
            int lastSpaceIndex = CurrentProjectName.LastIndexOf('_');
            string ProjectName = (lastSpaceIndex != -1) ? CurrentProjectName[(lastSpaceIndex + 1)..] : CurrentProjectName;
            return ProjectName;
        }

        private static string SelectOutputFolder(string ProjectName, bool isFullExport)
        {
            var folderDialog = new OpenFolderDialog { };
            if (folderDialog.ShowDialog() == true)
            {
                if (isFullExport)
                    return folderDialog.FolderName;

                string directoryPath = Path.Combine(folderDialog.FolderName, ProjectName);
                Directory.CreateDirectory(directoryPath);
                return directoryPath;
            }
            return string.Empty;
        }


        private static void Succses(string OutputPath, bool isFullExport)
        {

            string text = "Файлы комментариев были успешно записаны.\nОткрыть выходную папку?";
            if (GeneralSettings.Default.Language == "en")
                text = "The comments files were successfully writed.\nOpen the output folder?";

            var openFolder = MessageBox.Show(text, "Information", MessageBoxButton.YesNo, MessageBoxImage.None);
            if (openFolder == MessageBoxResult.Yes)
            {
                if (isFullExport)
                    Process.Start("explorer.exe", "/select, \"" + OutputPath + "\"");
                else
                    Process.Start("explorer.exe", "\"" + OutputPath + "\"");
            }
        }
    }
}
