using Mega_Subtitles_Reborn.Utilitis.FileReader;
using System.Diagnostics;
using System.IO;
using System.Net;
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
            Loaded += UpdateDialogue_Loaded;
            

        }
        private void UpdateDialogue_Loaded(object sender, EventArgs e)
        {
            int id = 0;
            if (GeneralSettings.Default.Language == "Русский")
            {
                id = 1;
                SkipTB.FontSize = 12;
                UpdateTB.FontSize = 12;
            }

            var lang = JsonReader.ReadLanguageJson(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LanguagesFile.json"));

            UpdateDialogue1.Title = lang["UpdateDialogue1"][id];
            NewVersionText.Text = lang["NewVersionText"][id].Replace("XYZ", InternetVersionOfApplication);
            OldVersionText.Text = lang["OldVersionText"][id].Replace("XYZ", LocalVersionOfApplication);
            DownloadText.Text = lang["DownloadText"][id];

            SkipTB.Text = lang["SkipTB"][id];
            UpdateTB.Text = lang["UpdateTB"][id];

        }

        private void SkipBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            DownloadProgressBar.Visibility = Visibility.Visible;
            DownloadProgressBar.Value = 0;

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            using WebClient client = new();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
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


                if (GeneralSettings.Default.Language == "Русский")                
                    MessageBox.Show("Приложение будет закрыто для обновления", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                
                MessageBox.Show("Application will be closed for update", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                Process.Start("cmd", "/k UpdateProgram.bat");
                Application.Current.Shutdown();
            }

            //tar -xf Application.zip -C ApplicationPath to unzip specifig folder

        }

    }
}
