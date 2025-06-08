using Mega_Subtitles.Helper.Subtitles;
using Mega_Subtitles_Reborn.Utilities;
using Mega_Subtitles_Reborn.Utilities.FileReader;
using Mega_Subtitles_Reborn.Utilities.FileWriter;
using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilities.Subtitles.AssProcessing;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.FromReaper;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using Mega_Subtitles_Reborn.Utilitis.PreRun;
using Mega_Subtitles_Reborn.Utilitis.Subtitles;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RoutedCommand CtrlS { get; set; } = new();
        public RoutedCommand CtrlI { get; set; } = new();
        public RoutedCommand CtrlE { get; set; } = new();
        public RoutedCommand ShiftE { get; set; } = new();
        public RoutedCommand CtrlO { get; set; } = new();
        public RoutedCommand CtrlD { get; set; } = new();
        public RoutedCommand CtrlM { get; set; } = new();
        public RoutedCommand CtrlR { get; set; } = new();
        public RoutedCommand CtrlA { get; set; } = new();
        public RoutedCommand CtrlP { get; set; } = new();
        public RoutedCommand CtrlF { get; set; } = new();

        public ObservableCollection<SubtitlesEnteries> SubtitleEntries { get; set; } = [];
        public ObservableCollection<ActorsEnteries> ActorEnteries { get; set; } = [];
        public CollectionViewSource subtitleViewSource = new();

        public ObservableCollection<string> AvailableActors { get; set; } = [];
        public Dictionary<string, SolidColorBrush> ActorsAndColorsDict = [];

        public Version InternetVersionOfApplication = new();
        public string? DownloadURL = "NAN";

        public List<int> _matchedIndices = [];
        public int _currentMatchIndex = -1;
        public string _lastKeyword = "";


        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            SubtitleEntries = [];
            ActorEnteries = [];
            AvailableActors = [""];

            subtitleViewSource = new CollectionViewSource
            {
                Source = SubtitleEntries
            };
            RegionManagerListView.ItemsSource = subtitleViewSource.View;
            ActorsListView.ItemsSource = ActorEnteries;

            this.DataContext = this;
            ActorsListView.DataContext = this;

            SetupHotKeys();

            ColorPickerCombobox.ItemsSource = ListSolidColor.SolidColors();

            LanguageChanger.UpdateLanguage();
            this.Title = "Mega Subtitles Reborn v" + Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        }


        private void MainWindow_Loaded(object sender, EventArgs e)
        {

            if (GeneralSettings.Default.SaveWindowSize)
            {
                this.Width = GeneralSettings.Default.MainWindowWidth;
                this.Height = GeneralSettings.Default.MainWindowHeight;
            }
            else
            {
                this.Width = 1366;
                this.Height = 820;
            }



            CheckAppVersion.CheckVersion();


            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timer.Start();
            timer.Tick += (sender, args) =>
            {
                timer.Stop();

                if (DownloadURL != null && DownloadURL == "NAN")
                {
                    PreRunCheckAndRealize.CheckAndRealize();
                    SelectSubtitlesBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            };
        }


        private void SelectSubtitlesBtn_Click(object sender, RoutedEventArgs e)
        {
            (string? InputFilePath, string? InputFileType) = SelectSubtitlesFile.ChooseFile();
            if (InputFilePath != null && InputFileType != null)
            {
                FileTypeSplitter.TypeSplitter(InputFilePath, InputFileType);
            }
        }

        private void ParseSubtitlesBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                subtitleViewSource.Filter -= Filters.FilterWithComment;
                subtitleViewSource.Filter -= Filters.ActorsFilter;

                if (ActorsListView.SelectedItems.Count > 0)
                {
                    var subtitlesData = JsonReader.ReadAssSubtitlesDataJson(GeneralSettings.Default.ProjectCahceJsonPath);
                    List<SubtitlesEnteries> entries = subtitlesData.Entries;

                    var selectedActors = ActorsListView.SelectedItems
                        .Cast<ActorsEnteries>()
                        .Select(a => a.Actors)
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var filteredEntries = entries
                        .Where(entry => selectedActors.Contains(entry.Actor))
                        .ToList();

                    SubtitleEntries.Clear();
                    foreach (var entry in filteredEntries)
                    {
                        SubtitleEntries.Add(entry);
                    }

                    RegionManagerListView.Items.Refresh(); // Optional                                             
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите актёров для парсинга.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }


                ReaperCommandsWriter.Write("CreateRegionsWithOutColor"); // Create regions without color command


            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name);
            }
        }

        private void SelectAllActorsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ActorsListView.SelectedItems.Count == ActorsListView.Items.Count)
                ActorsListView.UnselectAll();
            else
                ActorsListView.SelectAll();
        }

        private void OpenCacheFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "\"" + GeneralSettings.Default.ProjectCacheFolderPath + "\"");
        }

        private void ClearProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            string messageText = "Вы точно хотите очистить кэш проекта?\nЭто приведёт к удалению всех ранее добавленных комментариев и актёров.";
            string messageHeader = "Оповещение";

            if (GeneralSettings.Default.Language == "English")
            {
                messageText = "Are you sure you want to clear the project cache?\nThis will delete all previously added comments and actors.";
                messageHeader = "Notification";
            }

            var result = MessageBox.Show(messageText, messageHeader, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                Directory.Delete(GeneralSettings.Default.ProjectCacheFolderPath, true);

        }

        private void SaveSubtitlesBtn_Click(object sender, RoutedEventArgs e)
        {
            var saveSubtitlesWindow = new SaveSubtitlesWindow { Owner = this };
            saveSubtitlesWindow.ShowDialog();
        }

        private void ActorComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            JsonWriter.WriteCacheJson();
        }

        private void CommentsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            JsonWriter.WriteCacheJson();
        }


        private void SetActorColorContext_Click(object sender, RoutedEventArgs e)
        {
            var selectedActors = ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList();
            ChangeColorActorLabel.Content = selectedActors[0];

            ColorPickerGrid.Visibility = Visibility.Visible;
            ActorReanameGrid.Visibility = Visibility.Hidden;
        }

        private void RenameActorContext_Click(object sender, RoutedEventArgs e)
        {
            var selectedActors = ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList();
            ActorLabel.Content = selectedActors[0];
            ActorTextBox.Text = selectedActors[0];

            ActorReanameGrid.Visibility = Visibility.Visible;
            ColorPickerGrid.Visibility = Visibility.Hidden;
        }

        private void DeleteActorContext_Click(object sender, RoutedEventArgs e)
        {
            ActorsProcessing.DeleteActor();
        }


        private void SetColorBtn_Click(object sender, RoutedEventArgs e)
        {
            ActorsProcessing.SetActorColor();
            ColorPickerGrid.Visibility = Visibility.Hidden;
        }

        private void ColorPickerCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerGrid.Visibility = Visibility.Hidden;
        }

        private void ActorReanameCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            ActorReanameGrid.Visibility = Visibility.Hidden;
        }

        private void RenameBtn_Click(object sender, RoutedEventArgs e)
        {
            ActorsProcessing.RenameActor();
            ActorReanameGrid.Visibility = Visibility.Hidden;
        }

        private void SeparateExportCommentsBtn_Click(object sender, RoutedEventArgs e)
        {
            CommentsWriter.WriteSeparatedComments();
        }

        private void FullExportCommentsBtn_Click(object sender, RoutedEventArgs e)
        {
            CommentsWriter.WriteFullComments();
        }

        private void ImportCommentsBtn_Click(object sender, RoutedEventArgs e)
        {
            CommentsReader.ReadComments();
        }

        private void FilterActorsBtn_Click(object sender, RoutedEventArgs e)
        {
            subtitleViewSource.Filter -= Filters.FilterWithComment;
            subtitleViewSource.Filter -= CheckForMissing.FilterForMissing;
            subtitleViewSource.Filter += Filters.ActorsFilter;

            subtitleViewSource.View.Refresh();
        }

        private void DeleteLineListViewContext_Click(object sender, RoutedEventArgs e)
        {
            RegionManagerLineUtil.DublicateOrDeleteLine("Delete"); // Delete the selected line in the region manager list view
        }

        private void DublicateLineListViewContext_Click(object sender, RoutedEventArgs e)
        {
            RegionManagerLineUtil.DublicateOrDeleteLine("Dublicate"); // Dublicate the selected line in the region manager list view
        }

        private void CopyTimingsListViewContext_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard.CopyText("timings"); // Copy timings from the selected entries in the region manager list view
        }

        private void CopyTextListViewContext_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard.CopyText("text"); // Copy text from the selected entries in the region manager list view
        }

        private void CopyCommentsListViewContext_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard.CopyText("comments"); // Copy comments from the selected entries in the region manager list view
        }

        private void ClearCommentsListViewContext_Click(object sender, RoutedEventArgs e)
        {
            RegionManagerLineUtil.ClearComments(); // Clear comments for selected entries in the region manager list view
        }

        private void RegionManagerListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RegionManagerLineUtil.ParseHotKeys(e); // Handle key presses for region manager list view
        }




        private void SetupHotKeys()
        {
            try
            {
                // Save subtitles hotkey (CTRL + S)
                this.CommandBindings.Add(new CommandBinding(CtrlS, SaveSubtitlesBtn_Click));

                // Import Comments hotkey (CTRL + I)
                this.CommandBindings.Add(new CommandBinding(CtrlI, ImportCommentsBtn_Click));

                // Export Full Comments hotkey (CTRL + E)
                this.CommandBindings.Add(new CommandBinding(CtrlE, FullExportCommentsBtn_Click));

                // Export Separated Comments hotkey (SHIFT + E)                
                this.CommandBindings.Add(new CommandBinding(ShiftE, SeparateExportCommentsBtn_Click));

                // Open cache folder hotkey (CTRL + O)
                this.CommandBindings.Add(new CommandBinding(CtrlO, OpenCacheFolderBtn_Click));

                // Clear the project hotkey (CTRL + D)
                this.CommandBindings.Add(new CommandBinding(CtrlD, ClearProjectBtn_Click));

                // Check for missing hotkey (CTRL + M)
                this.CommandBindings.Add(new CommandBinding(CtrlM, CheckForMissingBtn_Checked));

                // Check for repeats hotkey (CTRL + R)
                this.CommandBindings.Add(new CommandBinding(CtrlR, CheckForRepeatsBtn_Checked));

                // Select All hotkey (CTRL + A)
                this.CommandBindings.Add(new CommandBinding(CtrlA, SelectAllActorsBtn_Click));

                // Parse hotkey (CTRL + P)
                this.CommandBindings.Add(new CommandBinding(CtrlP, ParseSubtitlesBtn_Click));

                // Find hotkey (CTRL + F)
                this.CommandBindings.Add(new CommandBinding(CtrlF, OpenFindWindow));
            }
            catch (Exception ex)
            {
                // Handle exceptions
                MessageBox.Show($"Error setting up hotkeys: {ex.Message}"); // Show an error message if hotkey setup fails
            }
        }


        private void OpenFindWindow(object sender, RoutedEventArgs e)
        {
            var findWindow = new FindWindow { Owner = this }; // Create a new instance of FindWindow and set the owner to the current window
            findWindow.ShowDialog(); // Show the find window as a dialog
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow { Owner = this }; // Create a new instance of SettingsWindow and set the owner to the current window
            settingsWindow.ShowDialog(); // Show the settings window as a dialog
        }

        private void ViewOnlyWithCommentsBtn_Checked(object sender, RoutedEventArgs e)
        {
            subtitleViewSource.Filter += Filters.FilterWithComment; // Add the filter for comments
            subtitleViewSource.Filter -= Filters.ActorsFilter; // Remove the filter for actors
            subtitleViewSource.Filter -= CheckForMissing.FilterForMissing; // Remove the filter for missing entries
            subtitleViewSource.View.Refresh(); // Refresh the view to apply changes

        }

        private void ViewOnlyWithCommentsBtn_Unchecked(object sender, RoutedEventArgs e)
        {

            subtitleViewSource.Filter -= Filters.FilterWithComment; // Remove the filter for comments
            subtitleViewSource.Filter -= Filters.ActorsFilter; // Remove the filter for actors
            subtitleViewSource.Filter -= CheckForMissing.FilterForMissing; // Remove the filter for missing entries
            subtitleViewSource.View.Refresh(); // Refresh the view to apply changes
        }

        private void RegionsOnlyWithCommentsBtn_Checked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("RegionsOnlyWithComments"); // Create regions without color and only with comments command
        }

        private void RegionsOnlyWithCommentsBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("CreateRegionsWithOutColor"); // Create regions without color command
        }

        private void DeleteCommentsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RegionsOnlyWithCommentsBtn.IsChecked == true) // Unsubscribe from the Checked event to avoid recursion
            {
                RegionsOnlyWithCommentsBtn.Checked -= RegionsOnlyWithCommentsBtn_Checked; // Unsubscribe from the Checked event to avoid recursion
            }

            var selectedActors = ActorsListView.SelectedItems // Get the selected actors from the ActorsListView
                      .Cast<ActorsEnteries>()
                      .Select(a => a.Actors)
                      .Where(name => !string.IsNullOrWhiteSpace(name))
                      .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in SubtitleEntries) // Iterate through each subtitle entry in the SubtitleEntries collection
            {
                if (entry.Actor != null && selectedActors.Contains(entry.Actor)) // Check if the entry's actor is in the selected actors
                {
                    entry.Comment = string.Empty; // Clear the comment
                }
                else if (entry.Actor == null) // If the actor is null, clear the comment as well
                {
                    entry.Comment = string.Empty; // Clear the comment
                }
            }
            subtitleViewSource.View.Refresh();
            JsonWriter.WriteCacheJson();
        }

        private void ColorizeSelectedActorsBtn_Checked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("ColorizeSelectedActors"); // Create regions with color command
        }

        private void ColorizeSelectedActorsBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("CreateRegionsWithOutColor"); // Create regions without color command

        }

        private void ColorizeSelectedTracksBtn_Checked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("ColorizeSelectedTracks"); // Colorize selected tracks command
        }

        private void ColorizeSelectedTracksBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("CreateRegionsWithOutColor"); // Create regions without color command
        }

        private void ColorizeSelectedActorsCommentsBtn_Checked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("ColorizeSelectedActorsComments"); // Create regions without color command
        }

        private void ColorizeSelectedActorsCommentsBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("CreateRegionsWithOutColor"); // Create regions without color command
        }

        private void CheckForMissingBtn_Checked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("CheckForMissing"); // Create only Missing regions command

            CheckForMissing.DelayedMissingsCheck(400); // Delay to allow Reaper to process the command
            subtitleViewSource.View.Refresh(); // Refresh the view to apply changes
        }

        private void CheckForMissingBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("CreateRegionsWithOutColor"); // Create regions without color command

            subtitleViewSource.Filter -= Filters.FilterWithComment; // Remove the filter for comments
            subtitleViewSource.Filter -= Filters.ActorsFilter; // Remove the filter for actors
            subtitleViewSource.Filter -= CheckForMissing.FilterForMissing; // Remove the filter for missing entries
            subtitleViewSource.View.Refresh(); // Refresh the view to apply changes
        }

        private void CheckForRepeatsBtn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckForRepeatsBtn_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void FindDemoPhrasesBtn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void FindDemoPhrasesBtn_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void RegionManagerListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource; // Get the original source of the mouse event

            while (obj != null && obj != RegionManagerListView) // Traverse up the visual tree until we reach the ListView or the root
            {
                if (obj.GetType() == typeof(ListViewItem)) // Check if the original source is a ListViewItem
                { 
                    ReaperCommandsWriter.Write("SyncPositon"); // Create regions without color command
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj); // Move to the parent in the visual tree
            }
        }

        private void GeneralWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GeneralSettings.Default.MainWindowHeight = GeneralWindow.ActualHeight; // Save the height of the main window
            GeneralSettings.Default.MainWindowWidth = GeneralWindow.ActualWidth; // Save the width of the main window
            GeneralSettings.Default.Save(); // Save the settings

            ReaperCommandsWriter.Write("ApplicationCloseEvent"); // Create regions without color command

        }

        private void SyncBtn_Click(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("SyncCsToReaper"); // Create regions without color command
            ReadFromReaper.DelayedReadReaperSyncFile(400); // Delay to allow Reaper to process the command
        }

        private void ActorsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ActorsListView.SelectedItems.Count > 0) // Check if any actor is selected
                ParseSubtitlesBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); // Trigger the ParseSubtitlesBtn_Click event

        }
    }
}