using Mega_Subtitles_Reborn.Utilitis.FileReader;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for FindWindow.xaml
    /// </summary>
    public partial class FindWindow : Window
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public FindWindow()
        {
            InitializeComponent();
            Loaded += FindWindow_Loaded;
            SearchTextBox.Focus();
        }

        private void FindWindow_Loaded(object sender, EventArgs e)
        {
            int id = 0;
            if (GeneralSettings.Default.Language == "Русский")
                id = 1;

            var lang = JsonReader.ReadLanguageJson(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LanguagesFile.json"));

            FindWindow1.Title = lang["FindWindow1"][id];
            FindBtn.Content = lang["FindTB"][id];
        }


        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchTextBox.Text.Trim();
            if (string.IsNullOrEmpty(keyword)) return;

            if (mainWindow._matchedIndices.Count == 0 || mainWindow._currentMatchIndex == -1 || mainWindow._lastKeyword != keyword)
            {
                // New search
                mainWindow._lastKeyword = keyword;
                mainWindow._matchedIndices = [.. mainWindow.SubtitleEntries
                    .Select((entry, index) => new { entry, index })
                    .Where(x =>
                        (!string.IsNullOrEmpty(x.entry.Text) && x.entry.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(x.entry.Comment) && x.entry.Comment.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    .Select(x => x.index)];

                mainWindow._currentMatchIndex = 0;
            }
            else
                mainWindow._currentMatchIndex = (mainWindow._currentMatchIndex + 1) % mainWindow._matchedIndices.Count;


            if (mainWindow._matchedIndices.Count > 0)
            {
                int matchIndex = mainWindow._matchedIndices[mainWindow._currentMatchIndex];

                mainWindow.RegionManagerListView.SelectedItems.Clear();
                mainWindow.RegionManagerListView.SelectedItems.Add(mainWindow.SubtitleEntries[matchIndex]);
                mainWindow.RegionManagerListView.ScrollIntoView(mainWindow.SubtitleEntries[matchIndex]);

            }
            else
            {
                if (GeneralSettings.Default.Language == "Русский")
                {
                    MessageBox.Show("Совпадений не найдено.", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                MessageBox.Show("No matches found.", "Search", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            mainWindow._matchedIndices.Clear();
            mainWindow._currentMatchIndex = -1;
            mainWindow._lastKeyword = "";
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Enter)
            {
                FindBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void SearchTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchTextBox.Clear();
        }
    }
}
