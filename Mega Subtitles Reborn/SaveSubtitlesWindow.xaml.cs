using Mega_Subtitles_Reborn.Utilities.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for SaveSubtitlesWindow.xaml
    /// </summary>
    public partial class SaveSubtitlesWindow : Window
    {
        private List<string?> actorNames = [];
        private readonly List<CheckBox> actorCheckBoxes = [];


        public SaveSubtitlesWindow()
        {
            InitializeComponent();
            Loaded += SaveSubtitlesWindow_Loaded;
        }

        private void SaveSubtitlesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            actorNames = [.. ((MainWindow)Application.Current.MainWindow)
                            .ActorEnteries
                            .Select(a => a.Actors)
                            .Distinct()
                            .OrderBy(name => name)];

            UpdateCheckboxGrid();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCheckboxGrid();
        }

        private void UpdateCheckboxGrid()
        {
            if (actorNames == null || actorNames.Count == 0)
                return;

            double windowWidth = this.ActualWidth;
            int estimatedColumnWidth = 200;
            int columnCount = Math.Max(1, (int)(windowWidth / estimatedColumnWidth));

            ActorsCheckBoxGrid.Columns = columnCount;
            ActorsCheckBoxGrid.Children.Clear();
            actorCheckBoxes.Clear();

            foreach (var actor in actorNames)
            {
                var checkBox = new CheckBox
                {
                    Content = actor,
                    Margin = new Thickness(1),
                    FontSize = 14,
                    FontFamily = new FontFamily("Nunito SemiBold"),
                    Foreground = Brushes.White
                };

                actorCheckBoxes.Add(checkBox);
                ActorsCheckBoxGrid.Children.Add(checkBox);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            List<string?> selectedActors = [.. actorCheckBoxes
            .Where(cb => cb.IsChecked == true)
            .Select(cb => cb.Content.ToString())];

            if (selectedActors == null || selectedActors.Count == 0)
                return;

            string FileOrPath = string.Empty;
            string filter = string.Empty;

            if (SingleFileRadioBtn.IsChecked == true)
            {
                if (AssRadioBtn.IsChecked == true)
                    filter = "ASS Subtitles|*.ass";
                else
                    filter = "SRT Subtitles|*.srt";

                SaveFileDialog saveFileDialog1 = new()
                {
                    Filter = filter,
                    Title = "Save the subtitles file",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)

                };
                bool? result = saveFileDialog1.ShowDialog();
                if (result == true)
                {
                    if (AssRadioBtn.IsChecked == true)
                        AssFileWriter.WriteAssFile(selectedActors, saveFileDialog1.FileName, isSingleFile: true);
                    else if (SrtRadioBtn.IsChecked == true)
                        SrtFileWriter.WriteSrtFile(selectedActors, saveFileDialog1.FileName, isSingleFile: true);
                }
            }
            else if (MultiFileRadioBtn.IsChecked == true)
            {
                var folderDialog = new OpenFolderDialog
                {
                    Title = "Select folder for saving subtitles files",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                if (folderDialog.ShowDialog() == true)
                {
                    string? folderName = folderDialog.FolderName;
                    if (AssRadioBtn.IsChecked == true)
                        AssFileWriter.WriteAssFile(selectedActors, folderDialog.FolderName, isSingleFile: false);
                    else if (SrtRadioBtn.IsChecked == true)
                        SrtFileWriter.WriteSrtFile(selectedActors, folderDialog.FolderName, isSingleFile: false);
                }
            }

        }

        private void SelectAllActorsBtn_Click(object sender, RoutedEventArgs e)
        {
            bool AllCheckboxesChecked = true;
            foreach (var cb in actorCheckBoxes)
            {
                if (cb.IsChecked == false)
                    AllCheckboxesChecked = false;
            }


            foreach (var cb in actorCheckBoxes)
            {
                if (AllCheckboxesChecked == false)
                    cb.IsChecked = true;
                else
                    cb.IsChecked = false;
            }

        }
    }
}
