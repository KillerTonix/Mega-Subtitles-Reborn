using Mega_Subtitles_Reborn.Utilities.Subtitles;
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

            return subtitlesData ?? throw new Exception("Failed to deserialize subtitles JSON.");
        }

        public static List<string> ReadVersionJson(string inputPath)
        {

            DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(inputPath);

            var jsonBytes = File.ReadAllBytes(inputPath);

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true // so it ignores casing issues
            };

            var Data = JsonSerializer.Deserialize<List<string>>(jsonBytes, options);

            if (Data == null)
                return [];

            return Data;
        }

    }

}
