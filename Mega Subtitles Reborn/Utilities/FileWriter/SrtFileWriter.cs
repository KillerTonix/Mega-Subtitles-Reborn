using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis;
using System.IO;
using System.Text;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.FileWriter
{
    internal class SrtFileWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void WriteSrtFile(List<string?> SelectedActors, string SrtFilePath, bool isSingleFile)
        {


            if (isSingleFile)
            {
                var entries = mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>()
                .Where(e => SelectedActors.Contains(e.Actor)).OrderBy(e => e.Start).ToList();

                using var writer = new StreamWriter(SrtFilePath, true, Encoding.UTF8);
                int counter = 1;

                foreach (var item in entries)
                {
                    writer.WriteLine(counter++);
                    writer.WriteLine($"{item.Start.Replace('.', ',')} --> {item.End.Replace('.', ',')}");
                    writer.WriteLine($"{item.Text}");
                    writer.WriteLine("");
                }
            }
            else
            {
                for (int i = 0; i < SelectedActors.Count; i++)
                {
                    string? actor = SelectedActors[i];
                    if (actor == null)
                        return;

                    var entries = mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>().Where(e => e.Actor == actor).ToList();

                    string? OutputPath = Path.Combine(SrtFilePath, actor) + ".srt";

                    if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(OutputPath))
                    {
                        var result = MessageBox.Show($"{OutputPath} already exists.\nDo you want to replace it?\nIf press No then will be add '_(1)' after file name.", "Save the subtitles file", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (result == MessageBoxResult.Yes)
                        {
                            File.Delete(OutputPath);
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            OutputPath = Path.Combine(SrtFilePath, actor) + "_(1).srt";
                        }
                    }


                    using var writer = new StreamWriter(OutputPath, true, Encoding.UTF8);

                    int counter = 1;

                    foreach (var item in entries)
                    {
                        writer.WriteLine(counter++);
                        writer.WriteLine($"{item.Start.Replace('.', ',')} --> {item.End.Replace('.', ',')}");
                        writer.WriteLine($"{item.Text}");
                        writer.WriteLine("");
                    }

                }
            }
        }
    }
}
