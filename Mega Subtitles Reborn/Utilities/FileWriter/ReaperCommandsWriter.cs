using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Globalization;
using System.Reflection;
using System.Windows;


namespace Mega_Subtitles_Reborn.Utilities.FileWriter
{
    public class ReaperCommandsWriter
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        public static void Write(string command)
        {
            try
            {
                var selectedItems = mainWindow.RegionManagerListView.SelectedItems.Cast<SubtitlesEnteries>().ToList();
                string startPos = string.Empty;

                foreach (var item in selectedItems)
                {
                    startPos = item.Start;
                }

                var data = new CommandsData
                {
                    Command = command,
                    CachePath = GeneralSettings.Default.ProjectCahceJsonPath,
                    CurrentPosition = StringToSeccondsString(startPos),
                    DateAndTime = DateTime.Now,
                };

                JsonWriter.WriteCommandsDataJson(data, GeneralSettings.Default.ReaperCommandsFilePath);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ReaperCommandsWriter", MethodBase.GetCurrentMethod()?.Name);
            }
        }
        private static string StringToSeccondsString(string seconds)
        {
            if (string.IsNullOrWhiteSpace(seconds))
            {
                return "";
            }
            TimeSpan time = TimeSpan.Parse(seconds, CultureInfo.InvariantCulture);

            return time.TotalSeconds.ToString();
        }
    }
}
