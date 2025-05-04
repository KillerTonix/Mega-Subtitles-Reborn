using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using System.IO;
using System.Text;

namespace Mega_Subtitles_Reborn.Utilities.FileWriter
{
    internal class SrtFileWriter
    {
        public static void WriteSrtFile(string srtFilePath)
        {
            var subtitlesData = JsonReader.ReadAssSubtitlesDataJson(GeneralSettings.Default.ProjectCahceJsonPath);

            using var writer = new StreamWriter(srtFilePath, true, Encoding.UTF8);
            List<SubtitlesEnteries> entries = subtitlesData.Entries;
            int counter = 1;

            foreach (var item in entries)
            {
                writer.WriteLine(counter++);
                writer.WriteLine($"{item.Start} --> {item.End}");
                writer.WriteLine($"{item.Text}");
                writer.WriteLine("");
            }

        }
    }
}
