using Mega_Subtitles_Reborn.Utilities;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private bool SaveWindowSize = false;
        private bool SaveDublicateCache = false;

        public SettingsWindow()
        {
            InitializeComponent();
            Loaded += SettingsWindow_Loaded;

        }


        private void SettingsWindow_Loaded(object sender, EventArgs e)
        {
            int id = 0;
            if (GeneralSettings.Default.Language == "Русский")
            {
                id = 1;
                LanguageComboBox.SelectedIndex = 1;
            }
            else
            {
                id = 0;
                LanguageComboBox.SelectedIndex = 0;
            }


            var lang = JsonReader.ReadLanguageJson("LanguagesFile.json");

            SettingsWindow1.Title = lang["SettingsWindow1"][id];
            LanguageLabel.Content = lang["LanguageLabel"][id];
            ThemeLabel.Content = lang["ThemeLabel"][id];
            SaveMainWindowSizeLabel.Content = lang["SaveMainWindowSizeLabel"][id];
            KeepDublicateCacheLabel.Content = lang["KeepDublicateCacheLabel"][id];
            DarkComboBoxItem.Content = lang["DarkComboBoxItem"][id];
            LightComboBoxItem.Content = lang["LightComboBoxItem"][id];


            SaveSizeRadioBtn.Content = lang["SaveSizeRadioBtn"][id];
            DontSaveSizeRadioBtn.Content = lang["DontSaveSizeRadioBtn"][id];
            SaveDublicateCacheRadioBtn.Content = lang["SaveDublicateCacheRadioBtn"][id];
            DontSaveDublicateCacheRadioBtn.Content = lang["DontSaveDublicateCacheRadioBtn"][id];
            OpenDublicatedCacheFolderLabel.Content = lang["OpenDublicatedCacheFolderLabel"][id];
            OpenDublicatedCacheFolderTB.Text = lang["OpenDublicatedCacheFolderTB"][id];
            ApplySettingsTB.Text = lang["ApplySettingsTB"][id];


            if (GeneralSettings.Default.Theme == "Dark")
                ThemeComboBox.SelectedIndex = 0;
            else
                ThemeComboBox.SelectedIndex = 1;


            if (GeneralSettings.Default.SaveWindowSize)
                SaveSizeRadioBtn.IsChecked = true;
            if (GeneralSettings.Default.SaveDublicateCache)
                SaveDublicateCacheRadioBtn.IsChecked = true;
            if (GeneralSettings.Default.SaveWindowSize)
                SaveSizeRadioBtn.IsChecked = true;
            if (GeneralSettings.Default.SaveDublicateCache)
                SaveDublicateCacheRadioBtn.IsChecked = true;
        }


        private void SaveSizeRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SaveWindowSize = true;
        }

        private void DontSaveSizeRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SaveWindowSize = false;
        }

        private void SaveDublicateCacheRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SaveDublicateCache = true;
            if (OpenDublicatedCacheFolderBtn != null && OpenDublicatedCacheFolderLabel != null)
            {
                OpenDublicatedCacheFolderBtn.Visibility = Visibility.Visible;
                OpenDublicatedCacheFolderLabel.Visibility = Visibility.Visible;
            }
        }

        private void DontSaveDublicateCacheRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SaveDublicateCache = false;
            if (OpenDublicatedCacheFolderBtn != null && OpenDublicatedCacheFolderLabel != null)
            {
                OpenDublicatedCacheFolderBtn.Visibility = Visibility.Hidden;
                OpenDublicatedCacheFolderLabel.Visibility = Visibility.Hidden;
            }
        }


        private void ApplySettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem LanguageItem = (ComboBoxItem)LanguageComboBox.SelectedItem;
            GeneralSettings.Default.Language = LanguageItem.Content.ToString();

            ComboBoxItem ThemeItem = (ComboBoxItem)ThemeComboBox.SelectedItem;
            GeneralSettings.Default.Theme = ThemeItem.Content.ToString();

            GeneralSettings.Default.SaveDublicateCache = SaveDublicateCache;
            GeneralSettings.Default.SaveWindowSize = SaveWindowSize;
            GeneralSettings.Default.Save();

            LanguageChanger.UpdateLanguage();
            this.Close();
        }

        private void OpenDublicatedCacheFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "\"" + GeneralSettings.Default.DublicateCachePath + "\"");
        }


    }
}
