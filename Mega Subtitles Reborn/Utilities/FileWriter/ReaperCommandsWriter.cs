﻿using Mega_Subtitles_Reborn.Utilities.Subtitles;
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
        public static void Write(string command, bool isDemoPhrases = false)
        {
            try
            {
                var selectedItems = mainWindow.RegionManagerListView.SelectedItems.Cast<SubtitlesEnteries>().ToList();
                string startPos = string.Empty;
                List<string?> selectedActors = [.. mainWindow.ActorsListView.SelectedItems // Get the selected actors from the ActorsListView
                          .Cast<ActorsEnteries>()
                          .Select(a => a.Actors)
                          .Where(name => !string.IsNullOrWhiteSpace(name))];

                string cachePath = isDemoPhrases ? GeneralSettings.Default.DemoPhrasesPath : GeneralSettings.Default.ProjectCahceJsonPath;
                foreach (var item in selectedItems)
                {
                    startPos = item.Start;
                }

                var data = new CommandsData
                {
                    Command = command,
                    CachePath = cachePath,
                    CurrentPosition = StringToSeccondsString(startPos).Replace(",","."),
                    Actors = selectedActors,
                    DateAndTime = DateTime.Now,
                };

                JsonWriter.WriteCommandsDataJson(data, GeneralSettings.Default.ReaperCommandsFilePath);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ReaperCommandsWriter", MethodBase.GetCurrentMethod()?.Name);
            }
        }
        private static string StringToSeccondsString(string time)
        {
            if (string.IsNullOrWhiteSpace(time) || time.Length < 1)
            {
                return "";
            }
            string v = string.Empty;
            int dotIndex = time.LastIndexOf('.');
            if (time.Length - dotIndex == 3)
                time = time[..^1];

            TimeSpan timeTimespan = TimeSpan.Parse(time, CultureInfo.InvariantCulture);

            return timeTimespan.TotalSeconds.ToString();
        }
    }
}
