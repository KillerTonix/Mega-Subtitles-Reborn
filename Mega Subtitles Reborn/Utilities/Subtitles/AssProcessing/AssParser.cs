using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace Mega_Subtitles_Reborn.Utilitis.Subtitles.AssProcessing
{
    public static class AssParser
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void ParseAssFile(string filePath)
        {
            DirectoryOrFileCheck.DirectoryCheck(GeneralSettings.Default.ProjectCacheFolderPath);
            List<SolidColorBrush> colorPalette = ListSolidColor.SolidColors();
            HashSet<string> uniqueActors = [];

            var lines = File.ReadAllLines(filePath);
            var entries = new List<SubtitlesEnteries>();
            var actorsEntries = new List<ActorsEnteries>();

            int number = 1;
            int colorIndex = 0;

            string DialoguePattern = @"Dialogue:\s+(\d+),([0-9:.,]+),([0-9:.,]+),\s*(.*?),\s*(.*?),\s*(\d+),\s*(\d+),\s*(\d+),\s*(.*?)(?:,{(.*)})?$";
            string BracerPattern = @"\{.*?\}";


            foreach (var rawLine in lines)
            {
                if (rawLine.StartsWith("Dialogue:"))
                {
                    var match = Regex.Match(rawLine, DialoguePattern);

                    if (match.Success)
                    {
                        string actor = string.Empty;
                        string checkingText = match.Groups[9].Value.Trim().TrimStart(',');
                        if (match.Groups[5].Value.Trim().Length > 0)
                            actor = match.Groups[5].Value.Trim();
                        else
                        {
                            var BracerMatch = Regex.Match(checkingText, BracerPattern);

                            actor = "_Unknown_Actor_";
                            if (BracerMatch.Success)
                                continue;
                        }

                        if (!mainWindow.ActorsAndColorsDict.TryGetValue(actor, out SolidColorBrush? value))
                        {
                            value = colorPalette[colorIndex % colorPalette.Count];
                            mainWindow.ActorsAndColorsDict[actor] = value;
                            colorIndex++;
                        }
                                               

                        entries.Add(new SubtitlesEnteries
                        {
                            Number = number++,
                            Color = value.ToString(),
                            Layer = match.Groups[1].Value.Trim(),
                            Start = NormalizeTimeFormat(match.Groups[2].Value.Trim().Replace(",",".")),
                            End = NormalizeTimeFormat(match.Groups[3].Value.Trim().Replace(",", ".")),
                            Style = match.Groups[4].Value.Trim(),
                            Actor = actor,
                            Effect = match.Groups[8].Value.Trim(),
                            Text = RemoveTextTags(match.Groups[9].Value.Trim().TrimStart(',')),
                            Comment = ""
                        });                      
                    }
                }               
            }


            var data = new SubtitlesData
            {
                SubtitlesPath = GeneralSettings.Default.SubtitlesPath,
                Entries = entries
            };

            JsonWriter.WriteAssSubtitlesDataJson(data, GeneralSettings.Default.ProjectCahceJsonPath);
            
            var subtitlesData = JsonReader.ReadAssSubtitlesDataJson(GeneralSettings.Default.ProjectCahceJsonPath);
            List<SubtitlesEnteries> subtitleseEntries = subtitlesData.Entries;
            foreach (var entry in subtitleseEntries)
            {
                if (entry.Actor != null)
                    uniqueActors.Add(entry.Actor);
            }

            List<string> UniqueActorsList = [.. uniqueActors.OrderBy(actor => actor)];
            mainWindow.AvailableActors.Clear();

            int actorIndex = 1;

            foreach (var actor in UniqueActorsList)
            {
                int lineCount = subtitleseEntries.Count(e => e.Actor == actor);
                mainWindow.AvailableActors.Add(actor);
                actorsEntries.Add(new ActorsEnteries
                {
                    ActorsNumber = actorIndex++,
                    Actors = actor,
                    ActorsLineCount = lineCount
                    // ActorsColor is auto-set by UpdateColor() in the setter
                });
            }

            foreach (var entry in actorsEntries)
                mainWindow.ActorEnteries.Add(entry);
        }

        public static string RemoveTextTags(string text)
        {
            try
            {
                string result = Regex.Replace(text, @"\{.*?\}", string.Empty);
                result = Regex.Replace(result, @"\<.*?\>", string.Empty);
                return result.Replace(@" \N", Environment.NewLine).Replace(@"\N ", Environment.NewLine).Replace(@"\N", Environment.NewLine);
            }
            catch (Exception ex)
            {
                Logger.ExceptionLogger.LogException(ex, "AssParser", MethodBase.GetCurrentMethod()?.Name);
                return text;
            }
        }

        public static string NormalizeTimeFormat(string time)
        {
            int dotIndex = time.LastIndexOf('.');
            if (dotIndex != -1 && dotIndex < time.Length - 1)
            {
                string milliseconds = time[(dotIndex + 1)..];

                return milliseconds.Length switch
                {
                    1 => time + "00", // add 00
                    2 => time + "0", // add 0
                    3 => time,       // do nothing
                    _ => time        // leave as-is or handle differently if needed
                };
            }

            return time; // no dot found — return unchanged
        }


    }
}
