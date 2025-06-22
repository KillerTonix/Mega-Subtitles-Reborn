using System.Windows;
using System.Windows.Controls.Primitives;

namespace Mega_Subtitles_Reborn.Utilities
{
    public class UncheckOtherButtons
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void Uncheck(ToggleButton SelectedButton)
        {
            List<ToggleButton> buttons =
            [
                mainWindow.FilterActorsBtn,
                mainWindow.ViewOnlyWithCommentsBtn,
                mainWindow.RegionsOnlyWithCommentsBtn,
                mainWindow.ColorizeSelectedActorsBtn,
                mainWindow.ColorizeSelectedTracksBtn,
                mainWindow.ColorizeSelectedActorsCommentsBtn,
                mainWindow.CheckForMissingBtn,
                mainWindow.CheckForRepeatsBtn,
                mainWindow.FindDemoPhrasesBtn
            ];

            if (SelectedButton == mainWindow.RegionsOnlyWithCommentsBtn)
            {
               buttons.Remove(mainWindow.ViewOnlyWithCommentsBtn);
               buttons.Remove(mainWindow.ColorizeSelectedTracksBtn);
            }

            if (SelectedButton == mainWindow.ColorizeSelectedActorsBtn)
            {
                buttons.Remove(mainWindow.ViewOnlyWithCommentsBtn);
                buttons.Remove(mainWindow.ColorizeSelectedTracksBtn);
            }
            if (SelectedButton == mainWindow.ColorizeSelectedActorsCommentsBtn)
            {
                buttons.Remove(mainWindow.ViewOnlyWithCommentsBtn);
                buttons.Remove(mainWindow.ColorizeSelectedTracksBtn);
            }

            
            foreach (var button in buttons)
            {
                if (button.IsChecked == true && button != SelectedButton)
                    button.IsChecked = false;
            }

        }
    }
}
