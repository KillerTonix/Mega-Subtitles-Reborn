using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Mega_Subtitles_Reborn.Utilities
{
    public class Filters
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;


        public static void FilterWithComment(object sender, FilterEventArgs e)
        {
            try
            {
                if (e.Item is SubtitlesEnteries subtitlesEntry)
                {
                    e.Accepted = !string.IsNullOrEmpty(subtitlesEntry.Comment);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "Filters", MethodBase.GetCurrentMethod()?.Name);
            }
        }


        public static void ActorsFilter(object sender, FilterEventArgs e)
        {
            try
            {
                var selectedActors = mainWindow.ActorsListView.SelectedItems
                           .Cast<ActorsEnteries>()
                           .Select(a => a.Actors)
                           .Where(name => !string.IsNullOrWhiteSpace(name))
                           .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (e.Item is SubtitlesEnteries subtitlesEntry)
                {
                    // Show only rows where the actor matches the selected actors
                    e.Accepted = selectedActors.Contains(subtitlesEntry.Actor);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "Filters", MethodBase.GetCurrentMethod()?.Name);
            }
        }      
    }
}
