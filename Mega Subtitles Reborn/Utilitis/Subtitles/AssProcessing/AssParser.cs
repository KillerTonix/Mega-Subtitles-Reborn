using Mega_Subtitles_Reborn.Helper.Subtitles.ASS;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Mega_Subtitles_Reborn.Utilitis.Subtitles.AssProcessing
{
    public class AssParser
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        private static readonly string CurrentProjectName = mainWindow.CurrentProjectName;
        private static readonly string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;

        private static readonly string GlobalCacheFolderPath = Path.Combine(ApplicationPath, "Cache");

        private static readonly string ProjectCacheFolderPath = Path.Combine(GlobalCacheFolderPath, CurrentProjectName);
        private static readonly string ProjectCacheCacheFilePath = Path.Combine(ProjectCacheFolderPath, CurrentProjectName) + ".json";
    
        public static void ParseAssFile(string filePath)
        {
            DirectoryOrFileCheck.DirectoryCheck(ProjectCacheFolderPath);
            List<SolidColorBrush> colorPalette = ListSolidColor.SolidColors();
            HashSet<string> uniqueActors = [];

            var lines = File.ReadAllLines(filePath);
            var entries = new List<AssSubtitlesEnteries>();
            var actorColors = new Dictionary<string, SolidColorBrush>();
            var fieldMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int number = 1;
            int colorIndex = 0;
            bool inEvents = false;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();

                if (line.StartsWith("[Events]", StringComparison.OrdinalIgnoreCase))
                {
                    inEvents = true;
                    continue;
                }

                if (!inEvents)
                    continue;

                if (line.StartsWith("Format:", StringComparison.OrdinalIgnoreCase))
                {
                    // Parse Format: line
                    fieldMap.Clear();
                    var formatFields = line[7..].Split(',');
                    for (int i = 0; i < formatFields.Length; i++)
                    {
                        fieldMap[formatFields[i].Trim()] = i;
                    }
                    continue;
                }

                if (line.StartsWith("Dialogue:", StringComparison.OrdinalIgnoreCase))
                {
                    var content = line[9..].Trim();
                    var split = SplitCsvPreserveCommas(content);

                    string GetField(string fieldName)
                    {
                        return fieldMap.TryGetValue(fieldName, out int index) && index < split.Count
                            ? split[index].Trim()
                            : "";
                    }

                    var start = GetField("Start");
                    var end = GetField("End");
                    var style = GetField("Style");
                    var actor = GetField("Name");
                    var text = GetField("Text");

                    if (!actorColors.TryGetValue(actor, out SolidColorBrush? brush))
                    {
                        brush = colorPalette[colorIndex % colorPalette.Count];
                        actorColors[actor] = brush;
                        colorIndex++;
                    }

                    // Convert color to hex
                    string hexColor = ColorToHex(brush.Color);

                    entries.Add(new AssSubtitlesEnteries
                    {
                        Number = number++,
                        Color = hexColor,
                        Start = TimeSpan.Parse(start, CultureInfo.InvariantCulture),
                        End = TimeSpan.Parse(end, CultureInfo.InvariantCulture),
                        Actor = actor,
                        Text = text,
                        Comments = ""
                    });
                }
            }


            JsonWriter.WriteAssSubtitlesEnteriesJson(entries, ProjectCacheCacheFilePath);


            var ReadedJson = JsonReader.ReadAssSubtitlesEnteriesJson(ProjectCacheCacheFilePath);
            foreach (var entry in ReadedJson)
            {
                if (entry.Actor != null)                
                    uniqueActors.Add(entry.Actor);                
            }

            List<string> UniqueActorsList = [.. uniqueActors.OrderBy(actor => actor)];
            mainWindow.ActorsList.Items.Clear();
            foreach (var actor in UniqueActorsList)
                mainWindow.ActorsList.Items.Add(actor);

        }

        private static string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
              


        // Handles commas inside text (e.g., lines that have commas in the subtitle text)
        private static List<string> SplitCsvPreserveCommas(string line)
        {
            var result = new List<string>();
            var current = "";
            bool inQuotes = false;

            foreach (var c in line)
            {
                if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                    if (c == '"')
                        inQuotes = !inQuotes;
                }
            }

            result.Add(current);
            return result;
        }
    }
}
