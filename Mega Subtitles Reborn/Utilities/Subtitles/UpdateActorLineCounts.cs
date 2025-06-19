using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.Subtitles
{
    public static class UpdateActorLineCounts
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void Recalculate()
        {          
            if (mainWindow.SubtitleEntries == null || mainWindow.ActorEnteries == null) return;

            // Group and count how many subtitle lines each actor has
            var actorCounts = mainWindow.SubtitleEntries
                .Where(s => !string.IsNullOrEmpty(s.Actor))
                .GroupBy(s => s.Actor!)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var actorEntry in mainWindow.ActorEnteries)
            {
                if (actorEntry.Actors != null && actorCounts.TryGetValue(actorEntry.Actors, out int count))
                    actorEntry.ActorsLineCount = count;
                else
                    actorEntry.ActorsLineCount = 0;
            }
        }
    }
}
