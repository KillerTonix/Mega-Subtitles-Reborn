using System.Net;
using System.Windows;

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

            string? assemblyinfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            OldVersionText.Text = $"You have versio {assemblyinfo} installed.";

        }


        private void SkipBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
