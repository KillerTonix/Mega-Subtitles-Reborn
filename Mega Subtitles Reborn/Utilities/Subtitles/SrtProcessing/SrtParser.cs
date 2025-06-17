using Mega_Subtitles_Reborn.Utilitis;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace Mega_Subtitles_Reborn.Utilities.Subtitles.SrtProcessing
{
    internal class SrtParser
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void ParseSrtFile(string filePath)
        {
            try
            {
                DirectoryOrFileCheck.DirectoryCheck(GeneralSettings.Default.ProjectCacheFolderPath);
                List<SolidColorBrush> colorPalette = ListSolidColor.SolidColors();

                var lines = File.ReadAllLines(filePath);
                var entries = new List<SubtitlesEnteries>();
                var actorsEntries = new List<ActorsEnteries>();

                int i = 0;

                while (i < lines.Length)
                {
                    // Skip empty lines
                    if (string.IsNullOrWhiteSpace(lines[i]))
                    {
                        i++;
                        continue;
                    }

                    // Parse number
                    if (!int.TryParse(lines[i++], out int number))
                        continue;

                    // Parse timecode
                    if (i >= lines.Length || !Regex.IsMatch(lines[i], @"\d{2}:\d{2}:\d{2},\d{3} --> \d{2}:\d{2}:\d{2},\d{3}"))
                        continue;

                    string[] times = lines[i++].Split(" --> ");
                    string start = times[0].Trim();
                    string end = times[1].Trim();

                    // Parse text (possibly multi-line)
                    var textLines = new List<string>();
                    while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
                    {
                        if (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i + 1]))
                            textLines.Add(RemoveTextTags(lines[i++].Trim()) + Environment.NewLine);
                        else
                            textLines.Add(RemoveTextTags(lines[i++]));
                    }

                    entries.Add(new SubtitlesEnteries
                    {
                        Number = number,
                        Color = colorPalette[0].ToString(),
                        Start = start.Replace(",", "."),
                        End = end.Replace(",", "."),
                        Actor = "𓈖Unknown_Actor𓈖",
                        Text = string.Join(" ", textLines)
                    });
                }

                var data = new SubtitlesData
                {
                    SubtitlesPath = GeneralSettings.Default.SubtitlesPath,
                    Entries = entries
                };

                JsonWriter.WriteAssSubtitlesDataJson(data, GeneralSettings.Default.ProjectCahceJsonPath);

                mainWindow.AvailableActors.Clear();
                mainWindow.AvailableActors.Add("𓈖Unknown_Actor𓈖");
                actorsEntries.Add(new ActorsEnteries
                {
                    ActorsNumber = 1,
                    Actors = "𓈖Unknown_Actor𓈖",
                    ActorsLineCount = i
                });
                foreach (var entry in actorsEntries)
                    mainWindow.ActorEnteries.Add(entry);

                mainWindow.AvailableActorsColors.Clear();
                var usedColors = mainWindow.ActorsAndColorsDict.Values.Select(b => b.Color).ToHashSet();
                foreach (var brush in colorPalette.Where(brush => !usedColors.Contains(brush.Color)))
                    mainWindow.AvailableActorsColors.Add(brush);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "SrtParser", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }


        public static string RemoveTextTags(string text)
        {
            try
            {
                string result = Regex.Replace(text, @"\{.*?\}", string.Empty);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "SrtParser", MethodBase.GetCurrentMethod()?.Name);
                return text;
            }
        }
    }
}

