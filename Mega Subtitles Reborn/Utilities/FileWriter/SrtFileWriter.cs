using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis;
using System.IO;
using System.Text;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.FileWriter
{
    internal class SrtFileWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void WriteSrtFile(List<string?> selectedActors, string srtFilePath, bool isSingleFile = false, bool zeroLine = false, bool addTenSec = false)
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
                    var firstStart = TimeSpan.Parse(entries[0].Start);

                    if (firstStart > TimeSpan.FromSeconds(10))
                    {
                        // Add 20 seconds of noise from start
                        writer.WriteLine(counter++);
                        writer.WriteLine("00:00:00,000 --> 00:00:10,000");
                        writer.WriteLine("10 seconds for recording noise");
                        writer.WriteLine();
                    }
                    else
                    {
                        foreach (var item in entries)
                        {
                            var duration = TimeSpan.Parse(item.End) - TimeSpan.Parse(item.Start);
                            if (duration > TimeSpan.FromSeconds(11))
                            {
                                writer.WriteLine(counter++);
                                writer.WriteLine($"{item.Start.Replace('.', ',')} --> {item.End.Replace('.', ',')}");
                                writer.WriteLine("10 seconds for recording noise");
                                writer.WriteLine();
                                break;
                            }
                        }
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
                    .OrderBy(e => TimeSpan.Parse(e.Start))
                    .ToList();

                WriteToFile(srtFilePath, entries);
            }
            else
            {
                foreach (var actor in selectedActors)
                {
                    if (actor == null) continue;

                    var entries = allEntries
                        .Where(e => e.Actor == actor)
                        .OrderBy(e => TimeSpan.Parse(e.Start))
                        .ToList();

                    var outputPath = Path.Combine(srtFilePath, actor + ".srt");

                    if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(outputPath))
                    {
                        var result = MessageBox.Show($"{outputPath} already exists.\nDo you want to replace it?\nIf press No then '_(1)' will be added.", "Save the subtitles file", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (result == MessageBoxResult.Yes)
                        {
                            File.Delete(outputPath);
                        }
                        else
                        {
                            outputPath = Path.Combine(srtFilePath, actor + "_(1).srt");
                        }
                    }

                    WriteToFile(outputPath, entries);
                }
            }

            MessageBox.Show("Subtitles saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
