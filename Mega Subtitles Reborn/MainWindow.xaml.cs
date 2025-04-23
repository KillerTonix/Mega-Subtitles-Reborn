using Mega_Subtitles.Helper.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.PreRun;
using Mega_Subtitles_Reborn.Utilitis.Subtitles;
using System.Windows;


namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentProjectName = String.Empty;
       

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

        }
        private void MainWindow_Loaded(object sender, EventArgs e)
        {
           PreRunCheckAndRealize.CheckAndRealize();
        }


        private void SelectSubtitlesBtn_Click(object sender, RoutedEventArgs e)
        {
            (string? InputFilePath, string? InputFileType) = SelectSubtitlesFile.ChooseFile();
            if (InputFilePath != null && InputFileType != null)
            {
                FileTypeSplitter.TypeSplitter(InputFilePath, InputFileType);
            }
        }
    }
}