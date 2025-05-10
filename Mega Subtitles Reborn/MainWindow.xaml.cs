using Mega_Subtitles.Helper.Subtitles;
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
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
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


        public ObservableCollection<SubtitlesEnteries> SubtitleEntries { get; set; } = [];
        public ObservableCollection<ActorsEnteries> ActorEnteries { get; set; } = [];
        public CollectionViewSource subtitleViewSource = new();

        public ObservableCollection<string> AvailableActors { get; set; } = [];
        public Dictionary<string, SolidColorBrush> ActorsAndColorsDict = [];

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

            ColorPickerCombobox.ItemsSource = ListSolidColor.SolidColors();

            this.Title = "Mega Subtitles Reborn v" + Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        }
        private void MainWindow_Loaded(object sender, EventArgs e)
        {
            PreRunCheckAndRealize.CheckAndRealize();
            SelectSubtitlesBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
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

            if (GeneralSettings.Default.Language == "en")
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
            SaveJson();
        }

        private void CommentsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveJson();
        }

        private void SaveJson()
        {
            var data = new SubtitlesData
            {
                SubtitlesPath = GeneralSettings.Default.SubtitlesPath,
                Entries = [.. SubtitleEntries]
            };

            JsonWriter.WriteAssSubtitlesDataJson(data, GeneralSettings.Default.ProjectCahceJsonPath);
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

        private void GeneralWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                var findWindow = new FindWindow { Owner = this };
                findWindow.ShowDialog();
            }
        }

        private void RegionManagerListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RegionManagerLineUtil.SetActorHotkeys(e);
        }
    }
}