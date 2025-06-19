using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.FileWriter
{

    internal class TxtFileWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void WriteSrtFile(List<string?> selectedActors, string srtFilePath, bool isSingleFile = false, bool zeroLine = false, bool addTenSec = false)
        {
            var allEntries = mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>().ToList();

            void WriteToFile(string path, List<SubtitlesEnteries> entries)
            {
                using var writer = new StreamWriter(path, true, Encoding.UTF8);

                if (zeroLine)                
                    writer.WriteLine("00:00:00,000 > 00:00:00,000 > Empty Line");                

                if (addTenSec && entries.Count > 0)
                {
                    var firstStart = TimeSpan.Parse(entries[0].Start, CultureInfo.InvariantCulture);
                    if (firstStart > TimeSpan.FromSeconds(10))
                        writer.WriteLine("00:00:00,000 > 00:00:10,000 > 10 seconds for recording noise");
                    else
                    {
                        foreach (var item in entries)
                        {
                            var duration = TimeSpan.Parse(item.End, CultureInfo.InvariantCulture) - TimeSpan.Parse(item.Start, CultureInfo.InvariantCulture);
                            if (duration > TimeSpan.FromSeconds(11))
                            {
                                writer.WriteLine($"{item.Start.Replace('.', ',')} > {item.End.Replace('.', ',')} > 10 seconds for recording noise");
                                break;
                            }
                        }
                    }
                }
                foreach (var item in entries)
                    writer.WriteLine($"{item.Start.Replace('.', ',')} > {item.End.Replace('.', ',')} > {item.Text}");
            }

            if (isSingleFile)
            {
                var entries = allEntries
                    .Where(e => selectedActors.Contains(e.Actor))
                    .OrderBy(e => TimeSpan.Parse(e.Start, CultureInfo.InvariantCulture))
                    .ToList();
                WriteToFile(srtFilePath, entries);
                Succses(srtFilePath, true);
            }
            else
            {
                foreach (var actor in selectedActors)
                {
                    if (actor == null) continue;

                    var entries = allEntries
                        .Where(e => e.Actor == actor)
                        .OrderBy(e => TimeSpan.Parse(e.Start, CultureInfo.InvariantCulture))
                        .ToList();

                    var outputPath = Path.Combine(srtFilePath, actor + ".txt");

                    if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(outputPath))
                    {
                        string message = $"Файл {outputPath} уже существует.\nВы хотите заменить его?\nЕсли нажать Нет, то будет добавлено '_(1)'.";
                        if (GeneralSettings.Default.Language == "English")
                            message = $"{outputPath} already exists.\nDo you want to replace it?\nIf press No then '_(1)' will be added.";
                        var result = MessageBox.Show($"{outputPath} already exists.\nDo you want to replace it?\nIf press No then '_(1)' will be added.", "Save the subtitles file", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (result == MessageBoxResult.Yes)
                            File.Delete(outputPath);
                        else
                            outputPath = Path.Combine(srtFilePath, actor + "_(1).txt");
                    }

                    WriteToFile(outputPath, entries);
                }
                Succses(srtFilePath, false);
            }
        }

        private static void Succses(string OutputPath, bool isSingleFile)
        {
            string text = "Файлы были успешно записаны.\nОткрыть выходную папку?";
            if (GeneralSettings.Default.Language == "English")
                text = "The files were successfully writed.\nOpen the output folder?";

            var openFolder = MessageBox.Show(text, "Information", MessageBoxButton.YesNo, MessageBoxImage.None);
            if (openFolder == MessageBoxResult.Yes)
            {
                if (isSingleFile)
                    Process.Start("explorer.exe", "/select, \"" + OutputPath + "\"");
                else
                    Process.Start("explorer.exe", "\"" + OutputPath + "\"");
            }
        }
    }
}
