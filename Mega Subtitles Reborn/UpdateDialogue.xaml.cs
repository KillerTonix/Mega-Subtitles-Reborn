using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows;

namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for UpdateDialogue.xaml
    /// </summary>
    public partial class UpdateDialogue : Window
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public string? LocalVersionOfApplication = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        public string InternetVersionOfApplication = mainWindow.InternetVersionOfApplication.ToString();
        public string DownloadedContentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp\\Mega Subtitles Reborn\\Application.zip");
        private static readonly string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;


        public UpdateDialogue()
        {
            InitializeComponent();

            NewVersionText.Text = $"Mega Subtitles Reborn version {InternetVersionOfApplication} has been released!";
            OldVersionText.Text = $"You have versio {LocalVersionOfApplication} installed";

        }


        private void SkipBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            DownloadProgressBar.Visibility = Visibility.Visible;
            DownloadProgressBar.Value = 0;

            using WebClient client = new();
            Uri uri = new(mainWindow.DownloadURL ?? "");
            client.DownloadFileAsync(uri, DownloadedContentPath);
            client.DownloadProgressChanged += Downloadprogress;
        }

        private void Downloadprogress(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressBar.Value = e.ProgressPercentage;

            if (e.ProgressPercentage == 100)
            {

                File.WriteAllText("UpdateProgram.bat", $""""
                    tar -xf "{DownloadedContentPath}" -C "{ApplicationPath[..^1]}"
                    timeout 1 >nul
                    start "" "{ApplicationPath}\\Mega Subtitles Reborn.exe"
                    timeout 0 >nul
                    exit 
                    """");
                MessageBox.Show("Application will be closed for update", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                Process.Start("cmd", "/k UpdateProgram.bat");
                Application.Current.Shutdown();
            }

            //tar -xf Application.zip -C ApplicationPath to unzip specifig folder

        }

    }
}
