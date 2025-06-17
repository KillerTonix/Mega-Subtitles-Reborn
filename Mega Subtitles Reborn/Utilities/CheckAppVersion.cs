using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Net.Http;
using System.Reflection;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities
{
    internal class CheckAppVersion
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        static readonly HttpClient client = new();

        public async static void CheckVersion()
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync(@"https://raw.githubusercontent.com/KillerTonix/Mega-Subtitles-Reborn/refs/heads/master/Mega%20Subtitles%20Reborn/version.txt");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var LocalVersionOfApplication = Assembly.GetExecutingAssembly().GetName().Version;
                mainWindow.InternetVersionOfApplication = Version.Parse(responseBody);

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
