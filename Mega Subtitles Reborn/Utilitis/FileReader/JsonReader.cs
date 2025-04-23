using Mega_Subtitles_Reborn.Utilitis.Subtitles.AssProcessing;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;


namespace Mega_Subtitles_Reborn.Utilitis.FileReader
{
    public class JsonReader
    {
        public static List<AssSubtitlesEnteries> ReadAssSubtitlesEnteriesJson(string inputPath)
        {
            DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(inputPath);

            var jsonBytes = File.ReadAllBytes(inputPath);

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var entries = JsonSerializer.Deserialize<List<AssSubtitlesEnteries>>(jsonBytes, options) ?? []; // return empty list if deserialization fails

            return entries;
        }

    }
}
