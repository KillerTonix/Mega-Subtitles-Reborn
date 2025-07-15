using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.FileWriter
{
    internal class SrtFileWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void WriteSrtFile(List<string?> selectedActors, string srtFilePath, bool isSingleFile = false, bool zeroLine = false, bool addTenSec = false, int template = 1)
        {
            var allEntries = mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>().ToList();

            void WriteToFile(string path, List<SubtitlesEnteries> entries)
            {
                using var writer = new StreamWriter(path, true, Encoding.UTF8);
                int counter = 1;

                if (zeroLine)
                {
                    writer.WriteLine(counter++);
                    writer.WriteLine("00:00:00,000 --> 00:00:00,000");
                    writer.WriteLine();
                }

                if (addTenSec && entries.Count > 0)
                {
                    // Sort by start time to ensure correct order
                    entries = entries.OrderBy(e => TimeSpan.Parse(e.Start, CultureInfo.InvariantCulture)).ToList();

                    var inserted = false;
                    var previousEnd = TimeSpan.Zero;

                    foreach (var item in entries)
                    {
                        var start = TimeSpan.Parse(item.Start, CultureInfo.InvariantCulture);

                        // Check gap between previousEnd and this start
                        if ((start - previousEnd) >= TimeSpan.FromSeconds(10))
                        {
                            var noiseStart = previousEnd;
                            var noiseEnd = previousEnd + TimeSpan.FromSeconds(10);

                            writer.WriteLine(counter++);
                            writer.WriteLine($"{noiseStart:hh\\:mm\\:ss\\,ff} --> {noiseEnd:hh\\:mm\\:ss\\,ff}");
                            writer.WriteLine("10 seconds for recording noise");
                            writer.WriteLine();
                            inserted = true;
                            break;
                        }

                        previousEnd = TimeSpan.Parse(item.End, CultureInfo.InvariantCulture);
                    }

                    if (!inserted)
                    {
                        // Add noise entry after the last subtitle
                        var lastEnd = TimeSpan.Parse(entries.Last().End, CultureInfo.InvariantCulture);
                        var noiseStart = lastEnd;
                        var noiseEnd = lastEnd + TimeSpan.FromSeconds(10);

                        writer.WriteLine(counter++);
                        writer.WriteLine($"{noiseStart:hh\\:mm\\:ss\\,ff} --> {noiseEnd:hh\\:mm\\:ss\\,ff}");
                        writer.WriteLine("10 seconds for recording noise");
                        writer.WriteLine();
                    }
                }


                foreach (var item in entries)
                {
                    writer.WriteLine(counter++);
                    writer.WriteLine($"{item.Start.Replace('.', ',')} --> {item.End.Replace('.', ',')}");
                    writer.WriteLine(item.Text);
                    writer.WriteLine();
                }
            }

            if (isSingleFile)
            {
                var entries = allEntries
                    .Where(e => selectedActors.Contains(e.Actor))
                    .OrderBy(e => TimeSpan.Parse(e.Start, CultureInfo.InvariantCulture))
                    .ToList();

                WriteToFile(srtFilePath, entries);
                Succses(srtFilePath, true, template);

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

                    var outputPath = Path.Combine(srtFilePath, actor + ".srt");

                    if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(outputPath))
                    {
                        string message = $"Файл {outputPath} уже существует.\nВы хотите заменить его?\nЕсли нажать Нет, то будет добавлено '_(1)'.";
                        if (GeneralSettings.Default.Language == "English")
                            message = $"{outputPath} already exists.\nDo you want to replace it?\nIf press No then '_(1)' will be added.";
                        var result = MessageBox.Show($"{outputPath} already exists.\nDo you want to replace it?\nIf press No then '_(1)' will be added.", "Save the subtitles file", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (result == MessageBoxResult.Yes)                        
                            File.Delete(outputPath);                        
                        else                        
                            outputPath = Path.Combine(srtFilePath, actor + "_(1).srt");                        
                    }

                    WriteToFile(outputPath, entries);    
                }
                Succses(srtFilePath, false, template);
            }
        }

        private static void Succses(string OutputPath, bool isSingleFile, int template)
        {
            if (template != 1)
                return;

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
