using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for FindAndReplaceWindow.xaml
    /// </summary>
    public partial class FindAndReplaceWindow : Window
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        private static bool isFindTabOpen = true; // Default to Find tab open
        private static string? TextContent = string.Empty; // Static variable to hold the text content
        public FindAndReplaceWindow(bool IsFindTabOpen)
        {
            InitializeComponent();
            if (IsFindTabOpen)
            {
                this.Height = 120; // Set the height to 120 when the Find tab is open
                isFindTabOpen = true; // Set the flag to indicate Find tab is open
                FindHeader.Focus(); // Set focus to the Find header
                ReplaceAllButton.Visibility = Visibility.Hidden; // Hide the Replace All button
                ReplaceButton.Visibility = Visibility.Hidden; // Hide the Replace button
                FindNextButton.Visibility = Visibility.Visible; // Show the Find Next button
            }
            else
            {
                this.Height = 350; // Set the height to 350 when the Replace tab is open
                isFindTabOpen = false; // Set the flag to indicate Replace tab is open
                ReplaceHeader.Focus(); // Set focus to the Replace header
                ReplaceAllButton.Visibility = Visibility.Visible; // Show the Replace All button
                ReplaceButton.Visibility = Visibility.Visible; // Show the Replace button
                FindNextButton.Visibility = Visibility.Visible; // Show the Find Next button
            }
            Loaded += FindAndReplaceWindow_Loaded;
        }
        private void FindAndReplaceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int id = 0;
            if (GeneralSettings.Default.Language == "Русский")
                id = 1;

            var lang = JsonReader.ReadLanguageJson(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LanguagesFile.json"));

            FindTextBox.Text = lang["FindTextBox"][id];
            ReplaceTextBox.Text = lang["ReplaceTextBox"][id];

            FindNextButton.Content = lang["FindNextButton"][id];
            ReplaceButton.Content = lang["ReplaceButton"][id];
            ReplaceAllButton.Content = lang["ReplaceAllButton"][id];

            OptionsExpander.Header = lang["OptionsExpander"][id];
            ReplaceWhereLabel.Content = lang["ReplaceWhereLabel"][id];
            InTextColumnRadioBtn.Content = lang["InTextColumnRadioBtn"][id];
            InCommentsColumnRadioBtn.Content = lang["InCommentsColumnRadioBtn"][id];
            BatchReplaceLabel.Content = lang["BatchReplaceLabel"][id];
            BatchReplaceOpenBtn.Content = lang["BatchReplaceOpenBtn"][id];
            BatchReplaceReplaceBtn.Content = lang["BatchReplaceReplaceBtn"][id];
            FindTextBox.Focus();
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Enter)
                FindNextButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void FindNextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string keyword = FindTextBox.Text.Trim();
                if (isFindTabOpen)
                    keyword = FindFindTextBox.Text.Trim(); // Use FindFindTextBox when Find tab is open                
                else
                    keyword = FindTextBox.Text.Trim();

                if (string.IsNullOrEmpty(keyword)) return;

                mainWindow._lastKeyword = keyword;
                mainWindow._matchedIndices = [.. mainWindow.SubtitleEntries
                    .Select((entry, index) => new { entry, index })
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.entry.Text) && x.entry.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(x.entry.Comment) && x.entry.Comment.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    .Select(x => x.index)];

                if (mainWindow._matchedIndices.Count == 0 || mainWindow._currentMatchIndex == -1 || mainWindow._lastKeyword != keyword)
                    mainWindow._currentMatchIndex = 0;
                else
                    mainWindow._currentMatchIndex = (mainWindow._currentMatchIndex + 1) % mainWindow._matchedIndices.Count;


                if (mainWindow._matchedIndices.Count >= 1)
                {
                    int matchIndex = mainWindow._matchedIndices[mainWindow._currentMatchIndex];
                    mainWindow.RegionManagerListView.SelectedItems.Clear();
                    mainWindow.RegionManagerListView.SelectedItems.Add(mainWindow.SubtitleEntries[matchIndex]);
                    mainWindow.RegionManagerListView.ScrollIntoView(mainWindow.SubtitleEntries[matchIndex]);
                }
                else
                {
                    if (GeneralSettings.Default.Language == "Русский")
                        MessageBox.Show("Совпадений не найдено.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("No matches found.", "Search", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "FindWindow", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mainWindow._matchedIndices.Count == 0 || mainWindow._currentMatchIndex == -1)
                    FindNextButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                string findText = FindTextBox.Text.Trim();
                string replaceText = ReplaceTextBox.Text.Trim();
                if (string.IsNullOrEmpty(findText)) return;

                int matchIndex = mainWindow._matchedIndices[mainWindow._currentMatchIndex];
                var entry = mainWindow.SubtitleEntries[matchIndex];

                if (InTextColumnRadioBtn.IsChecked == true && !string.IsNullOrEmpty(entry.Text))
                    entry.Text = entry.Text.Replace(findText, replaceText, StringComparison.OrdinalIgnoreCase);
                else if (InCommentsColumnRadioBtn.IsChecked == true && !string.IsNullOrEmpty(entry.Comment))
                    entry.Comment = entry.Comment.Replace(findText, replaceText, StringComparison.OrdinalIgnoreCase);

                // Refresh ListView
                mainWindow.RegionManagerListView.Items.Refresh();

                // Proceed to next match
                FindNextButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ReplaceButton", MethodBase.GetCurrentMethod()?.Name);
            }
        }


        private void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string findText = FindTextBox.Text.Trim();
                string replaceText = ReplaceTextBox.Text.Trim();
                if (string.IsNullOrEmpty(findText)) return;

                int replaceCount = BatchReplace(findText, replaceText);

                // Refresh ListView
                mainWindow.RegionManagerListView.Items.Refresh();

                if (GeneralSettings.Default.Language == "Русский")
                    MessageBox.Show($"Заменено: {replaceCount}", "Замена", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show($"Replacements made: {replaceCount}", "Replace All", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ReplaceAllButton", MethodBase.GetCurrentMethod()?.Name);
            }
        }

        private int BatchReplace(string findText, string replaceText)
        {
            int replaceCount = 0;
            foreach (var entry in mainWindow.SubtitleEntries)
            {
                if (InTextColumnRadioBtn.IsChecked == true && !string.IsNullOrEmpty(entry.Text) &&
                    entry.Text.Contains(findText, StringComparison.OrdinalIgnoreCase))
                {
                    entry.Text = entry.Text.Replace(findText, replaceText, StringComparison.OrdinalIgnoreCase);
                    replaceCount++;
                }
                else if (InCommentsColumnRadioBtn.IsChecked == true && !string.IsNullOrEmpty(entry.Comment) &&
                    entry.Comment.Contains(findText, StringComparison.OrdinalIgnoreCase))
                {
                    entry.Comment = entry.Comment.Replace(findText, replaceText, StringComparison.OrdinalIgnoreCase);
                    replaceCount++;
                }
            }
            return replaceCount;
        }

        private void BatchReplaceOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Text file|*.txt;",
                Title = "Select the replacement file",
                Multiselect = false,
                CheckFileExists = true,   // Ensure selected file exists
                CheckPathExists = true    // Ensure selected path exists
            };

            // Show the dialog and check if the user selected a file
            if (openFileDialog.ShowDialog() == true)
            {
                // Load the file content into the BatchReplaceTextBox
                try
                {
                    string filePath = openFileDialog.FileName;
                    TextContent = File.ReadAllText(filePath);
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex, "BatchReplaceOpenBtn", MethodBase.GetCurrentMethod()?.Name);
                    if (GeneralSettings.Default.Language == "Русский")
                        MessageBox.Show("Ошибка при открытии файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("Error opening file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BatchReplaceReplaceBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var batchReplaceEntries = TextContent?.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                int replaceCount = 0;
                if (batchReplaceEntries == null || batchReplaceEntries.Length == 0)
                {
                    if (GeneralSettings.Default.Language == "Русский")
                        MessageBox.Show("Файл замены пуст или не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("Replacement file is empty or not selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (var entry in batchReplaceEntries)
                    replaceCount += BatchReplace(entry.Split(':')[0].Trim(), entry.Split(':')[1].Trim());

                // Refresh ListView
                mainWindow.RegionManagerListView.Items.Refresh();

                if (GeneralSettings.Default.Language == "Русский")
                    MessageBox.Show($"Заменено: {replaceCount}", "Замена", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show($"Replacements made: {replaceCount}", "Replace All", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ReplaceAllButton", MethodBase.GetCurrentMethod()?.Name);
            }
        }



        private void FindTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            mainWindow._matchedIndices.Clear();
            mainWindow._currentMatchIndex = -1;
            mainWindow._lastKeyword = "";
        }

        private void FindHeader_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Height = 120; // Set the height to 300 when the header is clicked
            isFindTabOpen = true; // Set the flag to indicate Find tab is open
            ReplaceAllButton.Visibility = Visibility.Hidden; // Show the Replace All button
            ReplaceButton.Visibility = Visibility.Hidden; // Show the Replace button
            FindNextButton.Visibility = Visibility.Visible; // Show the Find Next button

        }

        private void ReplaceHeader_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Height = 350; // Set the height to 300 when the header is clicked
            isFindTabOpen = false; // Set the flag to indicate Replace tab is open
            ReplaceAllButton.Visibility = Visibility.Visible; // Show the Replace All button
            ReplaceButton.Visibility = Visibility.Visible; // Show the Replace button
            FindNextButton.Visibility = Visibility.Visible; // Show the Find Next button
        }



        private void FindFindTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            mainWindow._matchedIndices.Clear();
            mainWindow._currentMatchIndex = -1;
            mainWindow._lastKeyword = "";
        }

        private void FindFindTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FindFindTextBox.Clear();
        }

        private void FindTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FindTextBox.Clear();
        }

        private void ReplaceTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ReplaceTextBox.Clear();
        }
    }
}
