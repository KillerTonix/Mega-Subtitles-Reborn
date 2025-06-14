using Mega_Subtitles_Reborn.Utilities.Subtitles;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Mega_Subtitles_Reborn.Utilitis.FromReaper
{
    public class ReadFromReaper
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static string ReadProjectName(string filePath)
        {
            try
            {
                if (DirectoryOrFileCheck.CheckFileExistingAndNotEmpty(filePath)) // Check if the project name file exists and is not empty
                    return File.ReadAllText(GeneralSettings.Default.CurrentProjectNameFilePath, Encoding.UTF8); // Read the project name from the file and return it
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
                double syncValue = double.Parse(File.ReadAllText(GeneralSettings.Default.ReaperSyncFilePath, Encoding.UTF8)); // Read the sync value from the file

                if (mainWindow.subtitleViewSource.View is CollectionView view) // Check if the View is a CollectionView
                {
                    var entries = view.Cast<SubtitlesEnteries>(); // Cast the View to a collection of SubtitlesEnteries
                    var match = entries.FirstOrDefault(e => FromSecondToMicroseconds(e.Start) == syncValue); // Find the first entry where the Start time matches the sync value

                    if (match != null) 
                    {
                        mainWindow.RegionManagerListView.SelectedItem = match; // Select the matching entry in the ListView
                        mainWindow.RegionManagerListView.ScrollIntoView(match); // Scroll the ListView to bring the selected item into view
                        mainWindow.RegionManagerListView.Focus(); // Set focus to the ListView
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
            TimeSpan time = TimeSpan.Parse(seconds, CultureInfo.InvariantCulture); // Parse the string to a TimeSpan object using invariant culture to ensure correct parsing
            return time.TotalSeconds; // Return the total seconds as a double
        }
    }
}
