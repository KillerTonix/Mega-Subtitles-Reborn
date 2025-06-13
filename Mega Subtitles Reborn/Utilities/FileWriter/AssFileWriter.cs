using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using System.IO;
using System.Text;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilitis.FileWriter
{
    public class AssFileWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void WriteAssFile(List<string?> selectedActors, string assFilePath, bool isSingleFile = false, bool zeroLine = false, bool addTenSec = false)
        {
            var subtitlesData = JsonReader.ReadAssSubtitlesDataJson(GeneralSettings.Default.ProjectCahceJsonPath);
            var allEntries = mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>().ToList();

            void WriteToFile(string path, List<SubtitlesEnteries> entries)
            {
                using var writer = new StreamWriter(path, true, Encoding.UTF8);
                writer.WriteLine(ChangeAssHeader(GetAssHeader(subtitlesData.SubtitlesPath)));

                if (zeroLine)
                    writer.WriteLine("Dialogue: 0,00:00:00.00,00:00:00.00,Default,,0,0,0,,\n");

                if (addTenSec && entries.Count > 0)
                {
                    var firstStart = TimeSpan.Parse(entries[0].Start);

                    if (firstStart > TimeSpan.FromSeconds(10))
                    {
                        // Add 20-second noise at start
                        writer.WriteLine("Dialogue: 0,00:00:00.00,00:00:10.00,Default,,0,0,0,,10 seconds for recording noise");
                    }
                    else
                    {
                        // Add 10-second noise entry after any long subtitle
                        foreach (var item in entries)
                        {
                            var duration = TimeSpan.Parse(item.End) - TimeSpan.Parse(item.Start);
                            var startPlusTenSec = TimeSpan.Parse(item.End) + TimeSpan.FromSeconds(10);
                            if (duration > TimeSpan.FromSeconds(11))
                            {
                                writer.WriteLine($"Dialogue: 0,{item.End},{startPlusTenSec},Default,,0,0,0,,10 seconds for recording noise");
                                break;
                            }
                        }
                    }
                }

                foreach (var item in entries)
                {
                    var text = item.Text.Replace(@"\N", Environment.NewLine);
                    writer.WriteLine($"Dialogue: {item.Layer},{item.Start},{item.End},{item.Style},{item.Actor},0,0,0,{item.Effect},{text}");
                }
            }

            if (isSingleFile)
            {
                var entries = allEntries
                    .Where(e => selectedActors.Contains(e.Actor))
                    .OrderBy(e => e.Start)
                    .ToList();

                WriteToFile(assFilePath, entries);
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

                    var outputPath = Path.Combine(assFilePath, actor + ".ass");

                    if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(outputPath))
                    {
                        var result = MessageBox.Show($"{outputPath} already exists.\nDo you want to replace it?\nIf press No then '_(1)' will be added.", "Save the subtitles file", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (result == MessageBoxResult.Yes)
                        {
                            File.Delete(outputPath);
                        }
                        else
                        {
                            outputPath = Path.Combine(assFilePath, actor + "_(1).ass");
                        }
                    }

                    WriteToFile(outputPath, entries);
                }
            }

            MessageBox.Show("Subtitles file saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }




        private static string[] GetAssHeader(string OldSubtitlesFilePath)
        {
            List<string> header = [];
            foreach (string line in File.ReadAllLines(OldSubtitlesFilePath))
            {
                header.Add(line); // Add the line to the list
                if (line.Contains("Format: Layer, Start, End"))
                    break; // Stop reading lines after finding the target line
            }

            return [.. header]; // Convert the list to an array before returning
        }

        private static string ChangeAssHeader(string[] Header)
        {

            for (int i = 0; i < Header.Length; i++)
            {
                if (Header[i].Contains("; Script generated by Aegisub 3.2.2"))
                {
                    Header[i] = "; File generated by Mega Subtitles";
                }
                else if (Header[i].Contains("; http://www.aegisub.org/"))
                {
                    Header[i] = "; Author - Artur Tonoyan\r\n; VK - https://vk.com/aatonoyan\r\n; Discord - arturtonoyan\r\n; Email - artur.tonoyan2012@yandex.ru\r\n; Telegram - @BioWareZ";
                }
            }

            return string.Join("\n", Header);
        }
    }



}
