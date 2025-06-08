using Mega_Subtitles_Reborn.Utilities;
using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;


namespace Mega_Subtitles_Reborn.Utilitis.FileReader
{
    public static class JsonReader
    {
        public static SubtitlesData ReadAssSubtitlesDataJson(string filePath)
        {
            try
            {
                DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(filePath);

                string json = File.ReadAllText(filePath, Encoding.UTF8);

                JsonSerializerOptions jsonSerializerOptions = new()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNameCaseInsensitive = true // so it ignores casing issues
                };
                JsonSerializerOptions options = jsonSerializerOptions;

                var subtitlesData = JsonSerializer.Deserialize<SubtitlesData>(json, options);

                return subtitlesData ?? throw new Exception("Failed to deserialize subtitles JSON.");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "JsonReader", MethodBase.GetCurrentMethod()?.Name);                
                return new SubtitlesData();
            }
        }

        public static Dictionary<string, string> ReadVersionJson(string filePath)
        {
            try
            {
                var jsonByts = Encoding.UTF8.GetBytes(filePath);

                JsonSerializerOptions jsonSerializerOptions = new()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNameCaseInsensitive = true
                };
                JsonSerializerOptions options = jsonSerializerOptions;

                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonByts, options);

                return data ?? [];
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "JsonReader", MethodBase.GetCurrentMethod()?.Name);
                return [];
            }

        }

        public static Dictionary<string, List<string>> ReadLanguageJson(string filePath)
        {
            try
            {
                JsonSerializerOptions options = new()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNameCaseInsensitive = true
                };

                string json = File.ReadAllText(filePath, Encoding.UTF8);
                var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json, options);
                return data ?? [];
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "JsonReader", MethodBase.GetCurrentMethod()?.Name);
                return [];
            }
        }



        public static List<ReaperData> ReadReaperDataListJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);

                JsonSerializerOptions options = new()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNameCaseInsensitive = true
                };

                var dataList = JsonSerializer.Deserialize<List<ReaperData>>(json, options);

                return dataList ?? throw new Exception("Failed to deserialize list of ReaperData.");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "JsonReader", MethodBase.GetCurrentMethod()?.Name);
                return [];
            }
        }


    }

}
