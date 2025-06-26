using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilitis.FileWriter
{
    public class AssFileWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void WriteAssFile(List<string?> selectedActors, string assFilePath, bool isSingleFile = false, bool zeroLine = false, bool addTenSec = false)
        {
            try
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
                        // Sort by start time to ensure correct order
                        entries = [.. entries.OrderBy(e => TimeSpan.Parse(e.Start, CultureInfo.InvariantCulture))];

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

                                writer.WriteLine($"Dialogue: 0,{noiseStart:hh\\:mm\\:ss\\.ff},{noiseEnd:hh\\:mm\\:ss\\.ff},Default,,0,0,0,,10 seconds for recording noise");
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

                            writer.WriteLine($"Dialogue: 0,{noiseStart:hh\\:mm\\:ss\\.ff},{noiseEnd:hh\\:mm\\:ss\\.ff},Default,,0,0,0,,10 seconds for recording noise");
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
                    Succses(assFilePath, true);

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

                        var outputPath = Path.Combine(assFilePath, actor + ".ass");

                        if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(outputPath))
                        {
                            var result = MessageBoxResult.No;
                            if (GeneralSettings.Default.Language == "English")
                                result = MessageBox.Show($"{outputPath} already exists.\nDo you want to replace it?\nIf press No then '_(1)' will be added.", "Save the subtitles file", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                            else
                                result = MessageBox.Show($"{outputPath} уже существует.\nВы хотите заменить его?\nЕсли нажмете Нет, то будет добавлено '_(1)'.", "Сохранить файл субтитров", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                            
                            if (result == MessageBoxResult.Yes)                            
                                File.Delete(outputPath);                            
                            else                            
                                outputPath = Path.Combine(assFilePath, actor + "_(1).ass");                           
                        }

                        WriteToFile(outputPath, entries);
                    }
                    Succses(assFilePath, false);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "AssFileWriter", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private static void Succses(string OutputPath, bool isSingleFile)
        {
            try
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
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "AssFileWriter", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
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
