using Mega_Subtitles.Helper.Subtitles;
using Mega_Subtitles_Reborn.Helper.Subtitles.ASS;
using Mega_Subtitles_Reborn.Utilities.FileWriter;
using Mega_Subtitles_Reborn.Utilities.Subtitles.AssProcessing;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using Mega_Subtitles_Reborn.Utilitis.PreRun;
using Mega_Subtitles_Reborn.Utilitis.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.Subtitles.AssProcessing;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;


namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public ObservableCollection<AssSubtitlesEnteries> SubtitleEntries { get; set; } = [];
        public CollectionViewSource subtitleViewSource = new();

        public ObservableCollection<string> AvailableActors { get; set; } = [];
        public Dictionary<string, SolidColorBrush> ActorsAndColorsDict = [];



        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            SubtitleEntries = [];
            AvailableActors = [""];

            subtitleViewSource = new CollectionViewSource
            {
                Source = SubtitleEntries
            };
            ActorsList.ItemsSource = AvailableActors;
            RegionManagerListView.ItemsSource = subtitleViewSource.View;


            this.DataContext = this;
            ActorsList.DataContext = this;

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

                if (ActorsList.SelectedItems.Count > 0)
                {
                    //var jsonRead = JsonReader.ReadAssSubtitlesEnteriesJson(ProjectCacheFolderPath);
                    var subtitlesData = JsonReader.ReadAssSubtitlesDataJson(GeneralSettings.Default.ProjectCahceJsonPath);
                    List<AssSubtitlesEnteries> entries = subtitlesData.Entries;

                    var selectedActors = ActorsList.SelectedItems
                        .Cast<string>() // Change if using a different type
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

#pragma warning disable CS8604 // Possible null reference argument.
                    var filteredEntries = entries
                        .Where(entry => selectedActors.Contains(entry.Actor)).ToList();
#pragma warning restore CS8604 // Possible null reference argument.

                    // Clear current and add new items
                    SubtitleEntries.Clear();
                    foreach (var entry in filteredEntries)
                    {
                        SubtitleEntries.Add(entry);
                    }

                    RegionManagerListView.Items.Refresh(); // Optional, in case bindings don’t update immediately                   

                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "MainWindow", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }

        private void SelectAllActorsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ActorsList.SelectedItems.Count == ActorsList.Items.Count)
                ActorsList.UnselectAll();
            else
                ActorsList.SelectAll();
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
            {
                Directory.Delete(GeneralSettings.Default.ProjectCacheFolderPath, true);
            }
        }

        private void SaveSubtitlesBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new()
            {
                Filter = "ASS Subtitles|*.ass|SRT Subtitles|*.srt",
                Title = "Save the subtitle file"
            };
            bool? result = saveFileDialog1.ShowDialog();
            if (result == true)
            {
                AssFileWriter.WriteAssFile(saveFileDialog1.FileName);
            }
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
            if (ActorsList.SelectedItem is string _)
            {
                ColorPickerGrid.Visibility = Visibility.Visible;
                ActorReanameGrid.Visibility = Visibility.Hidden;
            }
        }

        private void RenameActorContext_Click(object sender, RoutedEventArgs e)
        {
            if (ActorsList.SelectedItem is string actorName)
            {
                ActorLabel.Content = actorName;
                ActorTextBox.Text = actorName;
                ActorReanameGrid.Visibility = Visibility.Visible;
                ColorPickerGrid.Visibility = Visibility.Hidden;
            }
        }

        private void DeleteActorContext_Click(object sender, RoutedEventArgs e)
        {
            ActorsProcessing.DeleteActor();
        }


        private void SetColorBtn_Click(object sender, RoutedEventArgs e)
        {
            ActorsProcessing.SetActorColor();
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
        }

        private void SeparateExportCommentsBtn_Click(object sender, RoutedEventArgs e)
        {
            CommentsWriter.WriteSeparatedComments();
        }

        private void FullExportCommentsBtn_Click(object sender, RoutedEventArgs e)
        {
            CommentsWriter.WriteFullComments();
        }
    }
}