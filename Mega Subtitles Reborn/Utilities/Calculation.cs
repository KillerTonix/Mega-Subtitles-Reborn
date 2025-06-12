using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using System.IO;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities
{
    public class Calculation
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        public class ActorRange
        {
            public TimeSpan Start { get; set; }
            public TimeSpan End { get; set; }
            public string Actor { get; set; } = string.Empty;
            public double DurationSeconds => (End - Start).TotalSeconds;

            public override string ToString()
            {
                return $"{Start:c} - {End:c} {Actor}";
            }
        }

        public static void FindLongestActorParts()
        {
            var result = new Dictionary<string, ActorRange>();
            var entries = new List<SubtitlesEnteries>();
            string currentActor = string.Empty;
            TimeSpan groupStart = TimeSpan.Zero;
            TimeSpan groupEnd = TimeSpan.Zero;

            for (int i = 0; i < mainWindow.SubtitleEntries.Count; i++)
            {
                var entry = mainWindow.SubtitleEntries[i];
                var entryStart = TimeSpan.Parse(entry.Start);
                var entryEnd = TimeSpan.Parse(entry.End);

                if (entry.Actor != currentActor)
                {
                    // Start new group
                    currentActor = entry.Actor ?? string.Empty;
                    groupStart = entryStart;
                    groupEnd = entryEnd;

                    // End the previous group
                    if (currentActor != null)
                        UpdateLongest(result, currentActor, groupStart, groupEnd);
                }
                else
                {
                    // Continue same group
                    groupEnd = entryEnd;
                }
            }

            // Final group
            if (currentActor != null)
                UpdateLongest(result, currentActor, groupStart, groupEnd);

            int number = 1;
            foreach (var range in result.Values.OrderBy(r => r.Start))
            {
                entries.Add(new SubtitlesEnteries
                {
                    Number = number++,
                    Start = range.Start.ToString(@"hh\:mm\:ss\.fff"),
                    End = range.End.ToString(@"hh\:mm\:ss\.fff"),
                    Actor = range.Actor,
                    Text = range.Actor
                });

            }

            mainWindow.SubtitleEntries.Clear(); // Clear the existing entries in the SubtitleEntries collection
            foreach (var entry in entries)
            {
                mainWindow.SubtitleEntries.Add(entry); // Add the filtered entries to the SubtitleEntries collection
            }
            JsonWriter.WriteCacheJson(isDemoPhrases: true); // Write the current state of the cache to a JSON file when the ActorComboBox loses focus

        }

        private static void UpdateLongest(Dictionary<string, ActorRange> result, string actor, TimeSpan start, TimeSpan end)
        {
            double duration = (end - start).TotalSeconds;

            if (!result.ContainsKey(actor) || result[actor].DurationSeconds < duration)
            {
                result[actor] = new ActorRange
                {
                    Actor = actor,
                    Start = start,
                    End = end
                };
            }
        }
    }
}
