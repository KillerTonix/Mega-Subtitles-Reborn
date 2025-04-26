using Mega_Subtitles_Reborn.Utilitis.Subtitles.AssProcessing;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;


namespace Mega_Subtitles_Reborn.Utilitis.FileReader
{
    public class JsonReader
    {
        public static SubtitlesData ReadAssSubtitlesDataJson(string inputPath)
        {
            DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(inputPath);

            var jsonBytes = File.ReadAllBytes(inputPath);

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true // so it ignores casing issues
            };

            var subtitlesData = JsonSerializer.Deserialize<SubtitlesData>(jsonBytes, options);

            if (subtitlesData == null)
                throw new Exception("Failed to deserialize subtitles JSON.");

            return subtitlesData;
        }
    }

}
