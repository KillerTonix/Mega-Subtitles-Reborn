using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Reflection;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities
{
    public static class RecoverButtonsState
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void RecoverState(ButtonsStateData data)
        {
            try
            {
                mainWindow.ActorsListView.SelectedItems.Clear();
                foreach (var actor in data.SelectedActors)
                {
                    var entry = mainWindow.ActorsListView.Items.Cast<ActorsEnteries>().FirstOrDefault(a => a.Actors == actor);
                    if (entry != null)
                        mainWindow.ActorsListView.SelectedItems.Add(entry);
                }

                mainWindow.FilterActorsBtn.IsChecked = data.FilterActorsBtnIsChecked;
                mainWindow.ViewOnlyWithCommentsBtn.IsChecked = data.ViewOnlyWithCommentsBtnIsChecked;
                mainWindow.RegionsOnlyWithCommentsBtn.IsChecked = data.RegionsOnlyWithCommentsBtnIsChecked;
                mainWindow.ColorizeSelectedActorsBtn.IsChecked = data.ColorizeSelectedActorsBtnIsChecked;
                mainWindow.ColorizeSelectedTracksBtn.IsChecked = data.ColorizeSelectedTracksBtnIsChecked;
                mainWindow.ColorizeSelectedActorsCommentsBtn.IsChecked = data.ColorizeSelectedActorsCommentsBtnIsChecked;
                mainWindow.CheckForMissingBtn.IsChecked = data.CheckForMissingBtnIsChecked;
                mainWindow.CheckForRepeatsBtn.IsChecked = data.CheckForRepeatsBtnIsChecked;
                mainWindow.FindDemoPhrasesBtn.IsChecked = data.FindDemoPhrasesBtnIsChecked;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "RecoverButtonsState", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }

        }
    }
}
