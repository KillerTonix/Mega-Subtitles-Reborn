using Mega_Subtitles_Reborn.Utilities;
using Mega_Subtitles_Reborn.Utilities.Subtitles;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilitis.FileWriter
{
    class JsonWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void WriteAssSubtitlesDataJson(SubtitlesData data, string outputPath)
        {
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            JsonSerializerOptions options = jsonSerializerOptions;

            byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(data, options);
            File.WriteAllBytes(outputPath, jsonBytes);


            if (GeneralSettings.Default.SaveDublicateCache)
                File.WriteAllBytes(GeneralSettings.Default.DublicatedProjectCahceJsonPath, jsonBytes);

        }

        public static void WriteDeletedLinesJson(List<SubtitlesEnteries> entries, string outputPath)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(entries, options);
            File.WriteAllBytes(outputPath, jsonBytes);
        }


        public static void WriteCacheJson(bool isDemoPhrases = false)
        {
            var data = new SubtitlesData
            {
                SubtitlesPath = GeneralSettings.Default.SubtitlesPath,
                Entries = [.. mainWindow.SubtitleEntries]
            };
            if (isDemoPhrases)
                WriteAssSubtitlesDataJson(data, GeneralSettings.Default.DemoPhrasesPath);
            else
                WriteAssSubtitlesDataJson(data, GeneralSettings.Default.ProjectCahceJsonPath);
        }


        public static void WriteCommandsDataJson(CommandsData data, string outputPath)
        {
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            JsonSerializerOptions options = jsonSerializerOptions;

            byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(data, options);
            File.WriteAllBytes(outputPath, jsonBytes);
        }
    }

}
