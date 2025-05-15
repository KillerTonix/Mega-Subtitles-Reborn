using Mega_Subtitles_Reborn.Utilitis.FileReader;
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

            using HttpResponseMessage response = await client.GetAsync(@"https://raw.githubusercontent.com/KillerTonix/Mega-Subtitles-Reborn/refs/heads/master/Mega%20Subtitles%20Reborn/version.json");
            response.EnsureSuccessStatusCode();
            
            string responseBody = await response.Content.ReadAsStringAsync();
           
            Dictionary<string, string> result = JsonReader.ReadVersionJson(responseBody);

            var LocalVersionOfApplication = Assembly.GetExecutingAssembly().GetName().Version;
            mainWindow.InternetVersionOfApplication = Version.Parse(result["version"]);            

            if (mainWindow.InternetVersionOfApplication > LocalVersionOfApplication)
            {
                mainWindow.DownloadURL = result["url"];
                var updateWindow = new UpdateDialogue { Owner = mainWindow };
                updateWindow.ShowDialog();
            }
        }
    }
}
