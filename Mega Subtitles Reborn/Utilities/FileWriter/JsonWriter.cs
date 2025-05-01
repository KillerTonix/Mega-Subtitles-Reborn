using Mega_Subtitles_Reborn.Utilities.Subtitles;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Mega_Subtitles_Reborn.Utilitis.FileWriter
{   
    class JsonWriter
    {
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
        }
    }

}
