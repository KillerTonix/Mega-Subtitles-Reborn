using Mega_Subtitles_Reborn.Utilities.Subtitles;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Mega_Subtitles_Reborn.Utilitis.FromReaper
{
    public class ReadFromReaper
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void ReadProjectName()
        {
            string ProjectName = ReadReaperProjectName();

            if (ProjectName != string.Empty)
            {
                GeneralSettings.Default.CurrentProjectName = ProjectName;
                GeneralSettings.Default.Save();
            }

        }

        public static string ReadReaperProjectName()
        {
            try
            {
                if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(GeneralSettings.Default.CurrentProjectNameFilePath))
                    return File.ReadAllText(GeneralSettings.Default.CurrentProjectNameFilePath, Encoding.UTF8);
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.ExceptionLogger.LogException(ex, "FileReader", MethodBase.GetCurrentMethod()?.Name);
                return string.Empty;
            }
        }

        public static async void DelayedReadReaperSyncFile(int milliseconds)
        {
            await Task.Delay(milliseconds); // Wait asynchronously for the specified delay
            ReadReaperSyncFile(); // Call the method after the delay
        }

        private static void ReadReaperSyncFile()
        {
            try
            {
                double syncValue = double.Parse(File.ReadAllText(GeneralSettings.Default.ReaperSyncFilePath, Encoding.UTF8));

                if (mainWindow.subtitleViewSource.View is CollectionView view)
                {
                    var entries = view.Cast<SubtitlesEnteries>();
                    var match = entries.FirstOrDefault(e => FromSecondToMicroseconds(e.Start) == syncValue);

                    if (match != null)
                    {
                        mainWindow.RegionManagerListView.SelectedItem = match;
                        mainWindow.RegionManagerListView.ScrollIntoView(match);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ExceptionLogger.LogException(ex, "FileReader", MethodBase.GetCurrentMethod()?.Name);
            }
        }

        private static double FromSecondToMicroseconds(string seconds)
        {
            TimeSpan time = TimeSpan.Parse(seconds, CultureInfo.InvariantCulture);
            return time.TotalSeconds;
        }
    }
}
