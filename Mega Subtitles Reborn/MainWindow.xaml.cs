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
        public RoutedCommand CtrlZ { get; set; } = new();

        public ObservableCollection<SubtitlesEnteries> SubtitleEntries { get; set; } = [];
        public ObservableCollection<ActorsEnteries> ActorEnteries { get; set; } = [];
        public CollectionViewSource subtitleViewSource = new();
        public ObservableCollection<SolidColorBrush> AvailableActorsColors { get; set; } = [];
        public ObservableCollection<string> AvailableActors { get; set; } = [];
        public Dictionary<string, SolidColorBrush> ActorsAndColorsDict = [];

        public Version InternetVersionOfApplication = new();
        public string? DownloadURL = "NAN";

        public List<int> _matchedIndices = [];
        public int _currentMatchIndex = -1;
        public string _lastKeyword = "";


        public MainWindow()
        {
            InitializeComponent(); // Initialize the components of the MainWindow

            Loaded += MainWindow_Loaded; // Subscribe to the Loaded event of the MainWindow         

            subtitleViewSource = new CollectionViewSource
            {
                Source = SubtitleEntries // Set the source of the CollectionViewSource to the SubtitleEntries collection
            };
            RegionManagerListView.ItemsSource = subtitleViewSource.View; // Set the ItemsSource of the RegionManagerListView to the view of the SubtitleEntries collection
            ActorsListView.ItemsSource = ActorEnteries; // Set the ItemsSource of the ActorsListView to the ActorEnteries collection
            ColorPickerCombobox.ItemsSource = AvailableActorsColors; // Set the items source for the color picker combobox to a list of solid colors

            this.DataContext = this; // Set the DataContext of the MainWindow to the current instance
            ActorsListView.DataContext = this; // Set the DataContext of the ActorsListView to the current instance of MainWindow

            SetupHotKeys(); // Setup the hotkeys for various actions


            LanguageChanger.UpdateLanguage(); // Update the language of the application based on the user's settings
            this.Title = "Mega Subtitles Reborn v" + Assembly.GetExecutingAssembly().GetName().Version?.ToString(); // Set the title of the main window to include the application version
        }


        private void MainWindow_Loaded(object sender, EventArgs e)
        {
            try
            {
                if (GeneralSettings.Default.SaveWindowSize)
                {
                    this.Width = GeneralSettings.Default.MainWindowWidth;
                    this.Height = GeneralSettings.Default.MainWindowHeight;
                }
                else
                {
                    this.Width = 1376;
                    this.Height = 830;
                }

                CheckAppVersion.CheckVersion(); // Check the application version and update if necessary
                PreRunCheckAndRealize.CheckAndRealize(); // Check and realize the pre-run state of the application

                DelayedMissingsCheck(400); // Delay to allow Reaper to process the command and check for missing entries
                static async void DelayedMissingsCheck(int milliseconds)
                {
                    await Task.Delay(milliseconds); // Wait asynchronously for the specified delay
                    PostRunCheckAndRealize.CheckAndRealize(); // Check and realize the post-run state of the application
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }


            /*  var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
              timer.Start();
              timer.Tick += (sender, args) =>
              {
                  timer.Stop();

                  if (DownloadURL != null && DownloadURL == "NAN")
                  {

                  }
              };*/
        }



        private void SelectSubtitlesBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (string? InputFilePath, string? InputFileType) = SelectSubtitlesFile.ChooseFile();
                if (InputFilePath != null && InputFileType != null)
                    FileTypeSplitter.TypeSplitter(InputFilePath, InputFileType);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void ParseSubtitlesBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                subtitleViewSource.Filter -= Filters.FilterWithComment; // Remove the filter for comments
                subtitleViewSource.Filter -= Filters.ActorsFilter; // Remove the filter for actors
                subtitleViewSource.Filter -= CheckForMissing.FilterForMissing; // Remove the filter for missing entries

                if (ActorsListView.SelectedItems.Count > 0) // Check if any actors are selected
                {
                    var subtitlesData = JsonReader.ReadAssSubtitlesDataJson(GeneralSettings.Default.ProjectCahceJsonPath); // Read the subtitles data from the JSON file
                    List<SubtitlesEnteries> entries = subtitlesData.Entries; // Get the list of subtitle entries from the data

                    var selectedActors = ActorsListView.SelectedItems // Get the selected actors from the ActorsListView
                        .Cast<ActorsEnteries>()
                        .Select(a => a.Actors)
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var filteredEntries = entries
                        .Where(entry => selectedActors.Contains(entry.Actor)) // Filter entries based on selected actors
                        .ToList();

                    SubtitleEntries.Clear(); // Clear the existing entries in the SubtitleEntries collection
                    foreach (var entry in filteredEntries)
                    {
                        SubtitleEntries.Add(entry); // Add the filtered entries to the SubtitleEntries collection
                    }

                    // Renumber all entries
                    for (int i = 0; i < SubtitleEntries.Count; i++) // Iterate through all entries in the SubtitleEntries list
                    {
                        SubtitleEntries[i].Number = i + 1; // Set the Number property of each entry to its index + 1
                    }
                    RegionManagerListView.Items.Refresh(); // Refresh the ListView to reflect the changes made to the SubtitleEntries list
                    ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
                    ReaperCommandsWriter.Write("CreateRegionsWithOutColor"); // Create regions without color command
                }
                else
                {
                    if (GeneralSettings.Default.Language == "English")
                        MessageBox.Show("Please select actors for parsing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("Пожалуйста, выберите актёров для парсинга.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name);
            }
        }

        private void SelectAllActorsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ActorsListView.SelectedItems.Count == ActorsListView.Items.Count) // If all actors are selected
                ActorsListView.UnselectAll(); // Unselect all actors
            else
                ActorsListView.SelectAll(); // Select all actors
        }

        private void OpenCacheFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "\"" + GeneralSettings.Default.ProjectCacheFolderPath + "\""); // Open the project cache folder in File Explorer
        }

        private void ClearProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
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
                {
                    Directory.Delete(GeneralSettings.Default.ProjectCacheFolderPath, true); // Delete the project cache folder and all its contents
                    SubtitleEntries.Clear(); // Clear the SubtitleEntries collection
                    ActorEnteries.Clear(); // Clear the ActorEnteries collection

                    SelectSubtitlesBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); // Trigger the ParseSubtitlesBtn_Click event
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void SaveSubtitlesBtn_Click(object sender, RoutedEventArgs e)
        {
            var saveSubtitlesWindow = new SaveSubtitlesWindow { Owner = this }; // Create a new instance of SaveSubtitlesWindow and set the owner to the current window
            saveSubtitlesWindow.ShowDialog(); // Show the save subtitles window as a dialog
        }

        private void ActorComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            JsonWriter.WriteCacheJson(); // Write the current state of the cache to a JSON file when the ActorComboBox loses focus
        }

        private void CommentsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            JsonWriter.WriteCacheJson(); // Write the current state of the cache to a JSON file when the CommentsTextBox loses focus
        }


        private void SetActorColorContext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedActors = ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList(); // Get the selected actors from the ActorsListView
                ChangeColorActorLabel.Content = selectedActors[0]; // Set the label content to the name of the first selected actor

                ColorPickerGrid.Visibility = Visibility.Visible;
                ActorReanameGrid.Visibility = Visibility.Collapsed;
                AddActorGrid.Visibility = Visibility.Collapsed; // Hide the AddActorGrid
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void RenameActorContext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedActors = ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList();
                ActorLabel.Content = selectedActors[0];
                ActorTextBox.Focus(); // Set focus to the ActorTextBox for renaming
                ActorTextBox.Text = selectedActors[0];

                ColorPickerGrid.Visibility = Visibility.Collapsed;
                ActorReanameGrid.Visibility = Visibility.Visible;
                AddActorGrid.Visibility = Visibility.Collapsed; // Hide the AddActorGrid
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void DeleteActorContext_Click(object sender, RoutedEventArgs e)
        {
            ActorsProcessing.DeleteActor();
        }


        private void SetColorBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ActorsProcessing.SetActorColor();
                ColorPickerGrid.Visibility = Visibility.Collapsed;
                JsonWriter.WriteCacheJson(); // Write the current state of the cache to a JSON file when the SetColorBtn is clicked
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void ColorPickerCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerGrid.Visibility = Visibility.Collapsed;
        }

        private void ActorReanameCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            ActorReanameGrid.Visibility = Visibility.Collapsed;
        }

        private void RenameBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ActorsProcessing.RenameActor();
                ActorReanameGrid.Visibility = Visibility.Collapsed;
                JsonWriter.WriteCacheJson(); // Write the current state of the cache to a JSON file when the RenameBtn is clicked
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
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


        private void UndoDeletedLines(object sender, RoutedEventArgs e)
        {
            try
            {
                var deletedEntries = JsonReader.ReadDeletedLinesJson(GeneralSettings.Default.DeletedLinesJsonPath);

                if (deletedEntries != null && deletedEntries.Count > 0)
                {
                    foreach (var entry in deletedEntries)
                    {
                        // Clamp the insert index to be within valid range
                        int insertIndex = Math.Clamp(entry.Number - 1, 0, SubtitleEntries.Count);
                        SubtitleEntries.Insert(insertIndex, entry);
                    }

                    // Renumber all entries
                    for (int i = 0; i < SubtitleEntries.Count; i++)
                    {
                        SubtitleEntries[i].Number = i + 1;
                    }

                    JsonWriter.WriteCacheJson(); // Save updated list
                    subtitleViewSource.View.Refresh(); // Refresh the ListView
                    File.WriteAllText(GeneralSettings.Default.DeletedLinesJsonPath, "[]");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name);
            }
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

                // Undo deleted lines hotkey (CTRL + Z)
                this.CommandBindings.Add(new CommandBinding(CtrlZ, UndoDeletedLines));
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions that occur during hotkey setup
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
            ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
            subtitleViewSource.View.Refresh(); // Refresh the view to apply changes

        }

        private void ViewOnlyWithCommentsBtn_Unchecked(object sender, RoutedEventArgs e)
        {

            subtitleViewSource.Filter -= Filters.FilterWithComment; // Remove the filter for comments
            subtitleViewSource.Filter -= Filters.ActorsFilter; // Remove the filter for actors
            subtitleViewSource.Filter -= CheckForMissing.FilterForMissing; // Remove the filter for missing entries
            ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
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
            try
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
                ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
                subtitleViewSource.View.Refresh();
                JsonWriter.WriteCacheJson();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions that occur during the deletion of comments
            }

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
            ReaperCommandsWriter.Write("UnColorizeAllTracks"); // Create regions without color command
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
            try
            {
                ReaperCommandsWriter.Write("CheckForMissing"); // Create only Missing regions command

                CheckForMissing.DelayedMissingsCheck(400); // Delay to allow Reaper to process the command
                ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
                subtitleViewSource.View.Refresh(); // Refresh the view to apply changes
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void CheckForMissingBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("CreateRegionsWithOutColor"); // Create regions without color command

            subtitleViewSource.Filter -= Filters.FilterWithComment; // Remove the filter for comments
            subtitleViewSource.Filter -= Filters.ActorsFilter; // Remove the filter for actors
            subtitleViewSource.Filter -= CheckForMissing.FilterForMissing; // Remove the filter for missing entries
            ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
            subtitleViewSource.View.Refresh(); // Refresh the view to apply changes
        }

        private void CheckForRepeatsBtn_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ReaperCommandsWriter.Write("CheckForRepeats"); // Create only Missing regions command

                CheckForMissing.DelayedMissingsCheck(400); // Delay to allow Reaper to process the command
                ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
                subtitleViewSource.View.Refresh(); // Refresh the view to apply changes
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void CheckForRepeatsBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            ReaperCommandsWriter.Write("CreateRegionsWithOutColor"); // Create regions without color command

            subtitleViewSource.Filter -= Filters.FilterWithComment; // Remove the filter for comments
            subtitleViewSource.Filter -= Filters.ActorsFilter; // Remove the filter for actors
            subtitleViewSource.Filter -= CheckForMissing.FilterForMissing; // Remove the filter for missing entries
            ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
            subtitleViewSource.View.Refresh(); // Refresh the view to apply changes
        }

        private void FindDemoPhrasesBtn_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                Calculation.FindLongestActorParts();
                ReaperCommandsWriter.Write("ColorizeSelectedActors", isDemoPhrases: true); // Create only Missing regions command
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void FindDemoPhrasesBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            ParseSubtitlesBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); // Trigger the ParseSubtitlesBtn_Click event
        }

        private void RegionManagerListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
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
            try
            {
                ReaperCommandsWriter.Write("SyncCursorPosition"); // Create regions without color command
                ReadFromReaper.DelayedReadReaperSyncFile(200); // Delay to allow Reaper to process the command
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void ActorsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ActorsListView.SelectedItems.Count > 0) // Check if any actor is selected
                ParseSubtitlesBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); // Trigger the ParseSubtitlesBtn_Click event
        }

        private void FilterActorsBtn_Checked(object sender, RoutedEventArgs e)
        {
            subtitleViewSource.Filter -= Filters.FilterWithComment;
            subtitleViewSource.Filter -= CheckForMissing.FilterForMissing;
            subtitleViewSource.Filter += Filters.ActorsFilter;
            ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
            subtitleViewSource.View.Refresh();
        }

        private void FilterActorsBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            subtitleViewSource.Filter -= Filters.FilterWithComment;
            subtitleViewSource.Filter -= CheckForMissing.FilterForMissing;
            subtitleViewSource.Filter -= Filters.ActorsFilter;
            ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
            subtitleViewSource.View.Refresh();
        }

        private void SyncProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReaperCommandsWriter.Write("SyncProject"); // Create regions without color command
                JsonWriter.WriteCacheJson(); // Write the current state of the cache to a JSON file when the SyncProjectBtn is clicked
                SubtitleEntries.Clear(); // Clear the SubtitleEntries collection
                ActorEnteries.Clear(); // Clear the ActorEnteries collection


                DelayedMissingsCheck(400); // Delay to allow Reaper to process the command and check for missing entries
                static async void DelayedMissingsCheck(int milliseconds)
                {
                    await Task.Delay(milliseconds); // Wait asynchronously for the specified delay
                    PreRunCheckAndRealize.CheckAndRealize(); // Check and realize the pre-run state of the application
                    PostRunCheckAndRealize.CheckAndRealize(); // Check and realize the post-run state of the application
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void AddActorContext_Click(object sender, RoutedEventArgs e)
        {
            AddActorGrid.Visibility = Visibility.Visible; // Show the AddActorGrid to allow adding a new actor
            ColorPickerGrid.Visibility = Visibility.Collapsed;
            ActorReanameGrid.Visibility = Visibility.Collapsed;

            AddActorTextBox.Focus(); // Set focus to the AddActorTextBox for user input
        }

        private void AddActorBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AddActorTextBox.Text)) // Check if the AddActorTextBox is empty or contains only whitespace
                {
                    if (GeneralSettings.Default.Language == "English")
                        MessageBox.Show("Please enter an actor name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("Пожалуйста, введите имя актёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Exit the method if the actor name is invalid
                }

                if (AvailableActors.Contains(AddActorTextBox.Text.Trim())) // Check if the actor already exists in the AvailableActors collection
                {
                    if (GeneralSettings.Default.Language == "English")
                        MessageBox.Show("This actor already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("Этот актёр уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Exit the method if the actor already exists
                }

                var actors = AddActorTextBox.Text.Split(','); // Split the text in the AddActorTextBox by commas to allow adding multiple actors at once
                foreach (string actorName in actors)
                {
                    if (!string.IsNullOrWhiteSpace(actorName.Trim())) // Check if the actor name is empty or contains only whitespace            
                        AddActor(actorName); // Call the AddActor method to add the actor to the collections
                }

                void AddActor(string actorName)
                {
                    ActorEnteries.Add(new ActorsEnteries
                    {
                        ActorsNumber = ActorEnteries.Count + 1, // Set the actor number to the current count of actors + 1
                        Actors = actorName, // Get the text from the AddActorTextBox and trim any whitespace
                        ActorsColor = AvailableActorsColors[0].ToString() // Set the actor color to the first available color in the AvailableActorsColors collection
                    });

                    AvailableActors.Add(actorName); // Add the new actor to the AvailableActors collection
                    ActorsAndColorsDict[actorName] = AvailableActorsColors[0]; // Add the new actor and its color to the ActorsAndColorsDict dictionary
                    AvailableActorsColors.Remove(AvailableActorsColors[0]); // Remove the first color from the AvailableActorsColors collection
                    AddActorTextBox.Clear(); // Clear the AddActorTextBox after adding the actor
                    AddActorGrid.Visibility = Visibility.Collapsed; // Hide the AddActorGrid after adding the actor
                    ListViewColumnsAutoResize.AutoResizeColumns(); // Auto-resize the columns in the ListView to fit the content
                    subtitleViewSource.View.Refresh(); // Refresh the view to apply changes
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void AddActorCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            AddActorGrid.Visibility = Visibility.Collapsed; // Hide the AddActorGrid when the cancel button is clicked
        }
    }
}