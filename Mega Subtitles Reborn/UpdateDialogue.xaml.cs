using GithubReleaseDownloader;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
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
        public string? InternetVersionOfApplication = mainWindow.InternetVersionOfApplication.ToString();
        public static string? DownloadedContentPath = string.Empty;
        public static string? DownloadedContentDirectoryPath = string.Empty;
        private static readonly string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string UpdaterAppPath = Path.Combine(ApplicationPath, @"Updater\MegaUpdater.exe");
        private static readonly string UpdaterFilesPath = Path.Combine(ApplicationPath, @"Updater\FilePaths.txt");


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
            try
            {
                OldVersionText.Visibility = Visibility.Hidden;
                DownloadText.Visibility = Visibility.Hidden;
                NewVersionText.Text = GeneralSettings.Default.Language == "Русский" ? "Загрузка обновления..." : "Downloading update...";
                // The owner and repo to download from, and target path
                var owner = "KillerTonix";
                var repo = "Mega-Subtitles-Reborn";
                // Get last release using .GetLatest(), can substitute with other available methods
                var release = ReleaseManager.Instance.GetLatest(owner, repo);
                if (release is null) return;

                var downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp");

                DownloadedContentDirectoryPath = Path.Combine(downloadPath, $"Mega.Subtitles.Reborn.{release.TagName}");
                if (!Directory.Exists(DownloadedContentDirectoryPath))
                    Directory.CreateDirectory(DownloadedContentDirectoryPath);

                DownloadedContentPath = $"{DownloadedContentDirectoryPath}.zip";

                // In this case, we download all assets
                AssetDownloader.Instance.DownloadAllAssets(release, downloadPath);

                Delay(400); // Delay to allow Reaper to process the command and check for missing entries
                static async void Delay(int milliseconds)
                {
                    await Task.Delay(milliseconds); // Wait asynchronously for the specified delay
                    CreateUpdateProgramBatFile();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "UpdateDialogue", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
                this.Close(); // Close the update dialogue if an error occurs
            }
        }

        private static void CreateUpdateProgramBatFile()
        {
            try
            {
                if (GeneralSettings.Default.Language == "Русский")
                    MessageBox.Show("Приложение будет закрыто для обновления", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Application will be closed for update", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                

                string content = $"{DownloadedContentPath}\n" +
                                 $"{ApplicationPath[..^1]}\n" +
                                 $"{ApplicationPath}Mega Subtitles Reborn.exe";
                File.WriteAllText(UpdaterFilesPath, content, Encoding.UTF8);

                Delay(400); // Delay to allow Reaper to process the command and check for missing entries
                static async void Delay(int milliseconds)
                {
                    await Task.Delay(milliseconds); // Wait asynchronously for the specified delay
                                                    // Call the updater with the necessary parameters
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = $"\"{UpdaterAppPath}\"",
                        UseShellExecute = true
                    });

                    Application.Current.Shutdown(); // Close the main app to release all file handles
                }
                

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "UpdateDialogue", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }
    }
}
