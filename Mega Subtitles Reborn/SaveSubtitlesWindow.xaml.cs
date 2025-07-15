using Mega_Subtitles_Reborn.Utilities.FileReader;
using Mega_Subtitles_Reborn.Utilities.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;


        public SaveSubtitlesWindow()
        {
            InitializeComponent();
            Loaded += SaveSubtitlesWindow_Loaded;
        }

        private void SaveSubtitlesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int id = 0;
            if (GeneralSettings.Default.Language == "Русский")
            {
                id = 1;
                SelectAllActorsTB.FontSize = 12;
            }
            else
            {
                id = 0;
                SelectAllActorsTB.FontSize = 14;
            }
            var lang = JsonReader.ReadLanguageJson(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LanguagesFile.json"));

            SaveSubtitlesWindow1.Title = lang["SaveSubtitlesWindow1"][id];
            SaveSettingsLabel.Content = lang["SaveSettingsLabel"][id];
            SingleFileRadioBtn.Content = lang["SingleFileRadioBtn"][id];
            MultiFileRadioBtn.Content = lang["MultiFileRadioBtn"][id];
            SaveFileFormatLabel.Content = lang["SaveFileFormatLabel"][id];
            SelectAllActorsTB.Text = lang["SelectAllActorsTB"][id];
            AddZeroLineChkBox.Content = lang["AddZeroLineChkBox"][id];
            AddTenSecForNoiseChkBox.Content = lang["AddTenSecForNoiseChkBox"][id];
            SaveTB.Text = lang["SaveTB"][id];
            SaveWithTemplateTB.Text = lang["SaveWithTemplateTB"][id];

            actorNames = [.. ((MainWindow)Application.Current.MainWindow)
                            .ActorEnteries
                            .Select(a => a.Actors)
                            .Distinct()
                            .OrderBy(name => name)];

            AddTenSecForNoiseChkBox.IsChecked = GeneralSettings.Default.AddTenSecForNoiseChkBox; // Load the state of the checkbox
            AddZeroLineChkBox.IsChecked = GeneralSettings.Default.AddZeroLineChkBox; // Load the state of the checkbox


            if (actorNames.Count < 10)
            {
                this.Height = 300;
                this.MinHeight = 300;
            }
            else if (actorNames.Count < 20)
            {
                this.Height = 400;
                this.MinHeight = 400;
            }

            UpdateCheckboxGrid();
        }

        private void UpdateCheckboxGrid()
        {
            try
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
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "SaveSubtitlesWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GeneralSettings.Default.AddTenSecForNoiseChkBox = AddTenSecForNoiseChkBox.IsChecked ?? false; // Save the state of the checkbox
                GeneralSettings.Default.AddZeroLineChkBox = AddZeroLineChkBox.IsChecked ?? false; // Save the state of the checkbox
                GeneralSettings.Default.Save(); // Save the settings

                List<string?> selectedActors = [.. actorCheckBoxes
                    .Where(cb => cb.IsChecked == true)
                    .Select(cb => cb.Content.ToString())];

                if (selectedActors.Count == 0) //repla
                {
                    MessageBox.Show("Please select at least one actor to save subtitles.", "No Actors Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                    if (GeneralSettings.Default.Language == "Русский")
                        MessageBox.Show("Пожалуйста, выберите хотя бы одного актера для сохранения субтитров.", "Актеры не выбраны", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                string FileOrPath = string.Empty;
                string filter = string.Empty;
                bool _zeroline = AddZeroLineChkBox.IsChecked ?? false;
                bool _addTenSec = AddTenSecForNoiseChkBox.IsChecked ?? false;
                if (SingleFileRadioBtn.IsChecked == true)
                {
                    if (AssRadioBtn.IsChecked == true)
                        filter = "ASS Subtitles|*.ass";
                    else if (SrtRadioBtn.IsChecked == true)
                        filter = "SRT Subtitles|*.srt";
                    else
                        filter = "Text Subtitles|*.txt";

                    string title = "Save the subtitles file";
                    if (GeneralSettings.Default.Language == "Русский")
                        title = "Сохраните файл субтитров";

                    SaveFileDialog saveFileDialog1 = new()
                    {
                        Filter = filter,
                        Title = title,
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)

                    };
                    bool? result = saveFileDialog1.ShowDialog();
                    if (result == true)
                    {
                        if (AssRadioBtn.IsChecked == true)
                            AssFileWriter.WriteAssFile(selectedActors, saveFileDialog1.FileName, isSingleFile: true, zeroLine: _zeroline, addTenSec: _addTenSec);
                        else if (SrtRadioBtn.IsChecked == true)
                            SrtFileWriter.WriteSrtFile(selectedActors, saveFileDialog1.FileName, isSingleFile: true, zeroLine: _zeroline, addTenSec: _addTenSec);
                        else
                            TxtFileWriter.WriteSrtFile(selectedActors, saveFileDialog1.FileName, isSingleFile: true, zeroLine: _zeroline, addTenSec: _addTenSec);
                    }
                }
                else if (MultiFileRadioBtn.IsChecked == true)
                {
                    string title = "Select folder for saving subtitles";
                    if (GeneralSettings.Default.Language == "Русский")
                        title = "Выберите папку для сохранения субтитров";

                    var folderDialog = new OpenFolderDialog
                    {
                        Title = title,
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    };

                    if (folderDialog.ShowDialog() == true)
                    {
                        string? folderName = folderDialog.FolderName;
                        if (AssRadioBtn.IsChecked == true)
                            AssFileWriter.WriteAssFile(selectedActors, folderDialog.FolderName, zeroLine: _zeroline, addTenSec: _addTenSec);
                        else if (SrtRadioBtn.IsChecked == true)
                            SrtFileWriter.WriteSrtFile(selectedActors, folderDialog.FolderName, zeroLine: _zeroline, addTenSec: _addTenSec);
                        else
                            TxtFileWriter.WriteSrtFile(selectedActors, folderDialog.FolderName, zeroLine: _zeroline, addTenSec: _addTenSec);
                    }
                }


            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "SaveSubtitlesWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
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

        private void SaveWithTemplateBtn_Click(object sender, RoutedEventArgs e)
        {
            GeneralSettings.Default.AddTenSecForNoiseChkBox = AddTenSecForNoiseChkBox.IsChecked ?? false; // Save the state of the checkbox
            GeneralSettings.Default.AddZeroLineChkBox = AddZeroLineChkBox.IsChecked ?? false; // Save the state of the checkbox
            GeneralSettings.Default.Save(); // Save the settings

            bool _zeroline = AddZeroLineChkBox.IsChecked ?? false;
            bool _addTenSec = AddTenSecForNoiseChkBox.IsChecked ?? false;

            string? filePath = SubtitlesDistributionTermplateReader.GetTemplate();
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Template file not selected or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dubberActors = ParseDubberActorTemplate(filePath);
            string title = "Select folder for saving subtitles";
            if (GeneralSettings.Default.Language == "Русский")
                title = "Выберите папку для сохранения субтитров";

            var folderDialog = new OpenFolderDialog
            {
                Title = title,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (folderDialog.ShowDialog() == true)
            {
                string? folderName = folderDialog.FolderName;
                int template = dubberActors.Keys.Count;
                foreach (var kvp in dubberActors)
                {
                    IEnumerable<string?> existedActors = kvp.Value.Intersect(actorNames);
                    var actors = existedActors.ToList();

                    if (AssRadioBtn.IsChecked == true)
                    {
                        folderName = Path.Combine(folderName, kvp.Key + ".ass");
                        AssFileWriter.WriteAssFile(actors, folderName, isSingleFile: true, zeroLine: _zeroline, addTenSec: _addTenSec, template);
                    }
                    else if (SrtRadioBtn.IsChecked == true)
                    {
                        folderName = Path.Combine(folderName, kvp.Key + ".srt");
                        SrtFileWriter.WriteSrtFile(actors, folderName, isSingleFile: true, zeroLine: _zeroline, addTenSec: _addTenSec, template);
                    }
                    else
                    {
                        folderName = Path.Combine(folderName, kvp.Key + ".txt");
                        TxtFileWriter.WriteSrtFile(actors, folderName, isSingleFile: true, zeroLine: _zeroline, addTenSec: _addTenSec, template);
                    }
                    folderName = folderDialog.FolderName;
                    template -= 1; // Decrease the template count for each dubber processed
                }
            }
        }


        // Add this method to your SaveSubtitlesWindow class
        private static Dictionary<string, List<string>> ParseDubberActorTemplate(string filePath)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var line in File.ReadLines(filePath))
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || !trimmedLine.Contains(':'))
                    continue;

                var parts = trimmedLine.Split(':', 2);
                var dubber = parts[0].Trim();
                var actors = parts[1].Split(',')
                    .Select(a => a.Trim())
                    .Where(a => !string.IsNullOrEmpty(a))
                    .ToList();

                if (!string.IsNullOrEmpty(dubber) && actors.Count > 0)
                    result[dubber] = actors;
            }
            return result;
        }

        private void InfoSaveWithTemplateBtn_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "SaveWithTemplate_Help.md";
            if (GeneralSettings.Default.Language == "Русский")
                fileName = "SaveWithTemplate_RuHelp.md";

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Help", fileName);
            var helpGuidesMarkDownWindow = new HelpGuidesMarkDownWindow(path) { Owner = this }; // Create a new instance of ReplaceWindow and set the owner to the current window
            helpGuidesMarkDownWindow.Show(); // Show the replace line text window as a dialog
        }

        private void SaveSubtitlesWindow1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainWindow.ActorsListView.SelectedItems.Clear(); // Clear the selected items in the ActorsListView
            foreach (int index in mainWindow.parsedActorsID)
            {
                mainWindow.ActorsListView.SelectedItems.Add(mainWindow.ActorEnteries[index]);
                MessageBox.Show($"Actor {mainWindow.ActorEnteries[index].Actors} has been selected in the main window.", "Actor Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            mainWindow.ActorsListView.Items.Refresh(); // Refresh the ActorsListView to show the selected actors
            mainWindow.sendCommandToReaper = true;
            mainWindow.ParseSubtitlesBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); // Trigger the ParseSubtitlesBtn_Click event
            mainWindow.RegionManagerListView.Items.Refresh(); // Refresh the ActorsListView to show the selected actors*/

        }
    }
}
