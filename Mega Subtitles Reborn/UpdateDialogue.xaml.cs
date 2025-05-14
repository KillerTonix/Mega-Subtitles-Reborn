using AutoUpdaterDotNET;
using System.Windows;

#pragma warning disable CA1416 // Validate platform compatibility
namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for UpdateDialogue.xaml
    /// </summary>
    public partial class UpdateDialogue : Window
    {

        public UpdateDialogue()
        {
            InitializeComponent();

            string? assemblyinfo =  System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            OldVersionText.Text = $"You have versio {assemblyinfo} installed.";

        }

        private void YesClick(object sender, RoutedEventArgs e)
        {
            AutoUpdater.HttpUserAgent = "AutoUpdater";
            AutoUpdater.ShowSkipButton = true;
            AutoUpdater.ShowRemindLaterButton = true;
            AutoUpdater.LetUserSelectRemindLater = true;

            AutoUpdater.Start(@"https://github.com/KillerTonix/Mega-Subtitles-Reborn/blob/master/Mega%20Subtitles%20Reborn/updates.xml");

        }

        private void Skip(object sender, RoutedEventArgs e)
        {

        }
    }
}
