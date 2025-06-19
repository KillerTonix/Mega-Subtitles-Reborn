using Mega_Subtitles_Reborn.Utilitis.Logger;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.FileReader
{
    internal class CommentsReader
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void ReadComments()
        {
            try
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Comments File (.txt)|*.txt",
                    Title = "Select the comments file",
                    Multiselect = true,
                    CheckFileExists = true,
                    CheckPathExists = true
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var file in openFileDialog.FileNames)
                    {
                        string[] lines = File.ReadAllLines(file, Encoding.UTF8);
                        for (int i = 0; i < lines.Length - 1; i += 2) // Skip blank line after each comment
                        {
                            string[] parts = lines[i].Split('\t');
                            if (parts.Length < 3) continue;

                            string start = parts[0];
                            string end = parts[1];
                            string comment = parts[2];

                            // Try to find matching entry by Start, End and Actor
                            var match = mainWindow.SubtitleEntries.FirstOrDefault(entry =>
                                entry.Start == start &&
                                entry.End == end);

                            if (match != null)
                            {
                                match.Comment = comment;
                            }
                        }
                    }
                    mainWindow.RegionManagerListView.Items.Refresh();
                    if (GeneralSettings.Default.Language == "English")
                        MessageBox.Show("Comments imported successfully.", "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Комментарии успешно импортированы.", "Импорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "CommentsReader", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }

        }

    }
}
