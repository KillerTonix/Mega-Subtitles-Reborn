using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Mega_Subtitles_Reborn.Utilities
{
    internal class CheckForMissing
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        private static List<ReaperData> reaperDataList = [];

        public static async void DelayedMissingsCheck(int milliseconds)
        {
            await Task.Delay(milliseconds); // Wait asynchronously for the specified delay
            MissingsCheck(); // Call the method after the delay
        }

        private static void MissingsCheck()
        {
            try
            {
                reaperDataList = JsonReader.ReadReaperDataListJson(GeneralSettings.Default.ProjectCahceJsonPath.Replace(".json", "_CacheFromReaper.json"));


                mainWindow.subtitleViewSource.Filter -= Filters.FilterWithComment;
                mainWindow.subtitleViewSource.Filter -= Filters.ActorsFilter;
                mainWindow.subtitleViewSource.Filter += FilterForMissing;

                // Refresh the view to apply changes
                mainWindow.subtitleViewSource.View.Refresh();

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "CheckForMissing", MethodBase.GetCurrentMethod()?.Name);
            }
        }

        public static void FilterForMissing(object sender, FilterEventArgs e)
        {
            try
            {
                if (e.Item is SubtitlesEnteries entry)
                {
                    var match = reaperDataList.FirstOrDefault(reaper =>
                        reaper.Start == FromSecondToMicroseconds(entry.Start) ||
                        reaper.End == FromSecondToMicroseconds(entry.End));

                    e.Accepted = match != null;
                }
                else
                {
                    e.Accepted = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "CheckForMissing", MethodBase.GetCurrentMethod()?.Name);
            }
        }

        private static double FromSecondToMicroseconds(string seconds)
        {
            TimeSpan time = TimeSpan.Parse(seconds, CultureInfo.InvariantCulture);

            return time.TotalSeconds;
        }
    }
}
