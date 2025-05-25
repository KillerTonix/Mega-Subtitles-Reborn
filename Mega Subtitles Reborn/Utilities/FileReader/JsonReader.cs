using Mega_Subtitles_Reborn.Utilities.Subtitles;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;


namespace Mega_Subtitles_Reborn.Utilitis.FileReader
{
    public static class JsonReader
    {
        public static SubtitlesData ReadAssSubtitlesDataJson(string inputPath)
        {
            DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(inputPath);

            var jsonBytes = File.ReadAllBytes(inputPath);

            JsonSerializerOptions jsonSerializerOptions = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true // so it ignores casing issues
            };
            JsonSerializerOptions options = jsonSerializerOptions;

            var subtitlesData = JsonSerializer.Deserialize<SubtitlesData>(jsonBytes, options);

            return subtitlesData ?? throw new Exception("Failed to deserialize subtitles JSON.");
        }

        public static Dictionary<string, string> ReadVersionJson(string inputString)
        {
            var jsonBytes = Encoding.UTF8.GetBytes(inputString);

            JsonSerializerOptions jsonSerializerOptions = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;

            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonBytes, options);

            return data ?? [];
        }

        public static Dictionary<string, List<string>> ReadLanguageJson(string filePath)
        {
            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true
            };

            string json = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json, options);
        }




    }

}
