using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities
{
    internal class CheckAppVersion
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public async static void CheckVersion()
        {
            try
            {
                using HttpClient client = new(); // Create a new HttpClient instance
                string url = "https://raw.githubusercontent.com/KillerTonix/Mega-Subtitles-Reborn/master/Mega%20Subtitles%20Reborn/version.txt";
                using HttpResponseMessage response = await client.GetAsync(url); // Send a GET request to the URL
                response.EnsureSuccessStatusCode(); // Ensure the request was successful
                string responseBody = await response.Content.ReadAsStringAsync(); // Read the response body as a string
                var LocalVersionOfApplication = Assembly.GetExecutingAssembly().GetName().Version; // Get the local version of the application
                mainWindow.InternetVersionOfApplication = Version.Parse(responseBody); // Parse the version from the response body

                if (mainWindow.InternetVersionOfApplication > LocalVersionOfApplication)
                {
                    var updateWindow = new UpdateDialogue { Owner = mainWindow };
                    updateWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "CheckAppVersion", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }
    }
}
