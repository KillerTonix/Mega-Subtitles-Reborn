﻿using Mega_Subtitles.Helper.Subtitles;
using Mega_Subtitles_Reborn.Utilities;
using Mega_Subtitles_Reborn.Utilities.FileReader;
using Mega_Subtitles_Reborn.Utilities.FileWriter;
using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilities.Subtitles.AssProcessing;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using Mega_Subtitles_Reborn.Utilitis.PreRun;
using Mega_Subtitles_Reborn.Utilitis.Subtitles;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
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
            subtitleViewSource.Filter += Filters.ActorsFilter;
            subtitleViewSource.View.Refresh();
        }

        private void DeleteLineListViewContext_Click(object sender, RoutedEventArgs e)
        {
            RegionManagerLineUtil.DublicateOrDeleteLine("Delete");
        }

        private void DublicateLineListViewContext_Click(object sender, RoutedEventArgs e)
        {
            RegionManagerLineUtil.DublicateOrDeleteLine("Dublicate");
        }

        private void CopyTimingsListViewContext_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard.CopyText("timings");
        }

        private void CopyTextListViewContext_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard.CopyText("text");
        }

        private void CopyCommentsListViewContext_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard.CopyText("comments");
        }

        private void ClearCommentsListViewContext_Click(object sender, RoutedEventArgs e)
        {
            RegionManagerLineUtil.ClearComments();
        }

        private void RegionManagerListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RegionManagerLineUtil.ParseHotKeys(e);
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
                MessageBox.Show($"Error setting up hotkeys: {ex.Message}");
            }
        }


        private void OpenFindWindow(object sender, RoutedEventArgs e)
        {
            var findWindow = new FindWindow { Owner = this };
            findWindow.ShowDialog();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow { Owner = this };
            settingsWindow.ShowDialog();
        }

        private void ViewOnlyWithCommentsBtn_Checked(object sender, RoutedEventArgs e)
        {
            subtitleViewSource.Filter += Filters.FilterWithComment;
            subtitleViewSource.Filter -= Filters.ActorsFilter;
            subtitleViewSource.View.Refresh();

        }

        private void ViewOnlyWithCommentsBtn_Unchecked(object sender, RoutedEventArgs e)
        {

            subtitleViewSource.Filter -= Filters.FilterWithComment;
            subtitleViewSource.Filter -= Filters.ActorsFilter;
            subtitleViewSource.View.Refresh();
        }

        private void RegionsOnlyWithCommentsBtn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RegionsOnlyWithCommentsBtn_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteCommentsBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedActors = ActorsListView.SelectedItems
                      .Cast<ActorsEnteries>()
                      .Select(a => a.Actors)
                      .Where(name => !string.IsNullOrWhiteSpace(name))
                      .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in SubtitleEntries)
            {
                if (entry.Actor != null && selectedActors.Contains(entry.Actor))
                {
                    entry.Comment = string.Empty; // Clear the comment
                }
            }
            subtitleViewSource.View.Refresh();
            JsonWriter.WriteCacheJson();
        }

        private void ColorizeSelectedActorsBtn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ColorizeSelectedActorsBtn_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void ColorizeSelectedTracksBtn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ColorizeSelectedTracksBtn_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void ColorizeSelectedActorsCommentsBtn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ColorizeSelectedActorsCommentsBtn_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckForMissingBtn_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckForMissingBtn_Unchecked(object sender, RoutedEventArgs e)
        {


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


        private void GeneralWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GeneralSettings.Default.MainWindowHeight = GeneralWindow.ActualHeight;
            GeneralSettings.Default.MainWindowWidth = GeneralWindow.ActualWidth;
            GeneralSettings.Default.Save();
        }
    }
}