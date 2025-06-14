using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileReader;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Mega_Subtitles_Reborn.Utilities
{
    public class PostRunCheckAndRealize
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void CheckAndRealize()
        {

            if (File.Exists(GeneralSettings.Default.ProjectCahceJsonPath))
            {
                var subtitlesData = JsonReader.ReadAssSubtitlesDataJson(GeneralSettings.Default.ProjectCahceJsonPath); // Read the subtitles data from the JSON file
                if (!File.Exists(subtitlesData.SubtitlesPath))
                {
                    MessageBox.Show("Файл субтитров не найден. Пожалуйста, выберите файл субтитров.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    mainWindow.SelectSubtitlesBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); // Trigger the SelectSubtitlesBtn_Click event to allow the user to select a subtitles file
                }
                else
                {
                    mainWindow.SubtitleEntries.Clear(); // Clear the existing entries in the SubtitleEntries collection
                    mainWindow.ActorEnteries.Clear(); // Clear the existing entries in the ActorEnteries collection
                    mainWindow.AvailableActors.Clear(); // Clear the existing actors in the AvailableActors collection

                    foreach (var entry in subtitlesData.Entries) // Iterate through each subtitle entry in the subtites data                    
                        mainWindow.SubtitleEntries.Add(entry); // Add the entry to the SubtitleEntries collection

                    mainWindow.subtitleViewSource.View.Refresh(); // Refresh the view to apply changes

                    List<SolidColorBrush> colorPalette = ListSolidColor.SolidColors();

                    HashSet<string> uniqueActors = [];
                    Dictionary<string, string> ActorsAndColors = [];

                    List<SubtitlesEnteries> subtitleseEntries = subtitlesData.Entries;
                    var actorsEntries = new List<ActorsEnteries>();
                    var entries = new List<SubtitlesEnteries>();
                    int colorIndex = 0;

                    foreach (var entry in subtitleseEntries)
                    {
                        if (entry.Actor != null)
                        {
                            uniqueActors.Add(entry.Actor);

                            if (!ActorsAndColors.ContainsKey(entry.Actor))
                                ActorsAndColors[entry.Actor] = entry.Color ?? colorPalette[colorIndex % colorPalette.Count].ToString();
                        }
                    }

                    List<string> UniqueActorsList = [.. uniqueActors.OrderBy(actor => actor)];

                    int actorIndex = 1;

                    foreach (var actor in UniqueActorsList)
                    {
                        mainWindow.AvailableActors.Add(actor);

                        Color color = (Color)ColorConverter.ConvertFromString(ActorsAndColors[actor]);
                        SolidColorBrush brush = new(color);

                        mainWindow.ActorsAndColorsDict[actor] = brush;

                        int lineCount = subtitleseEntries.Count(e => e.Actor == actor);
                        actorsEntries.Add(new ActorsEnteries
                        {
                            ActorsNumber = actorIndex++,
                            Actors = actor,
                            ActorsLineCount = lineCount
                        });
                    }

                    foreach (var entry in actorsEntries)
                        mainWindow.ActorEnteries.Add(entry);


                    mainWindow.AvailableActorsColors.Clear();
                    var usedColors = mainWindow.ActorsAndColorsDict.Values.Select(b => b.Color).ToHashSet();
                    foreach (var brush in colorPalette.Where(brush => !usedColors.Contains(brush.Color)))
                        mainWindow.AvailableActorsColors.Add(brush);



                }
            }
            else
            {
                MessageBox.Show("Файл кэша проекта не найден. Пожалуйста, выберите файл субтитров.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                mainWindow.SelectSubtitlesBtn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); // Trigger the SelectSubtitlesBtn_Click event to allow the user to select a subtitles file
            }
        }
    }
}
