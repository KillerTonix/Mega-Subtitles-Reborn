using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Mega_Subtitles_Reborn.Utilities.Subtitles.AssProcessing
{
    public static class ActorsProcessing
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void SetActorColor()
        {
            try
            {
                var selectedActors = mainWindow.ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList();
                List<SolidColorBrush> colorPalette = ListSolidColor.SolidColors();

                if (selectedActors[0] is string actorName)
                {
                    var selectedColor = mainWindow.ColorPickerCombobox.SelectedItem;

                    if (string.IsNullOrEmpty(actorName) || string.IsNullOrEmpty(selectedColor.ToString())) return;

                    foreach (var entry in mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>())
                    {
                        if (entry.Actor == actorName)
                        {
                            entry.Color = selectedColor.ToString();
                        }
                    }
                    foreach (var entry in mainWindow.ActorEnteries)
                    {
                        if (entry.Actors == actorName)
                        {
                            entry.ActorsColor = selectedColor.ToString();
                            mainWindow.ActorsAndColorsDict[actorName] = (SolidColorBrush)selectedColor;
                        }
                    }
                    mainWindow.ActorsListView.Items.Refresh();
                    mainWindow.RegionManagerListView.Items.Refresh();

                    mainWindow.AvailableActorsColors.Clear();
                    var usedColors = mainWindow.ActorsAndColorsDict.Values.Select(b => b.Color).ToHashSet();
                    foreach (var brush in colorPalette.Where(brush => !usedColors.Contains(brush.Color)))
                        mainWindow.AvailableActorsColors.Add(brush);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ActorsProcessing", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        public static void RenameActor()
        {
            try
            {
                var selectedActors = mainWindow.ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList();

                if (selectedActors[0] is not string selectedActor)
                {
                    MessageBox.Show("Please select an actor to rename.", "No Actor Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var item = mainWindow.ActorEnteries.Select((actor, index) => new { actor, index }).FirstOrDefault(a => a.actor.Actors == selectedActor);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                int index = item.index;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                string? newName = mainWindow.ActorTextBox.Text?.Trim();
                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show("The name cannot be empty.", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool nameExists = mainWindow.AvailableActors.Contains(newName);
                if (nameExists)
                {
                    string message = string.Empty;
                    string header = string.Empty;
                    if (GeneralSettings.Default.Language == "English")
                    {
                        message = $"The actor '{newName}' already exists. Do you want to combine the actors?";
                        header = "Notification";
                    }
                    else
                    {
                        message = $"Актер '{newName}' уже существует. Вы хотите объединить актеров?";
                        header = "Уведомление";
                    }

                    var result = MessageBox.Show(message, header, MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        string? targetColor = mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>()
                            .FirstOrDefault(e => e.Actor == newName && e.Color != null)?.Color;

                        foreach (var entry in mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>().Where(e => e.Actor == selectedActor))
                        {
                            entry.Actor = newName;
                            if (targetColor != null)
                                entry.Color = targetColor;
                        }

                        RefreshActorsList(newName, index: index);
                    }
                    else
                    {
                        newName = GetUniqueName(newName, [.. mainWindow.AvailableActors]);
                        UpdateActorNames(selectedActor, newName);
                        RefreshActorsList(newName, selectedActor, index);
                    }
                }
                else
                {
                    UpdateActorNames(selectedActor, newName);
                    RefreshActorsList(newName, selectedActor, index);
                }

                mainWindow.RegionManagerListView.Items.Refresh();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ActorsProcessing", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        public static void DeleteActor()
        {
            try
            {
                var selectedActors = mainWindow.ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList();
                if (selectedActors.Count == 0)
                {
                    if (GeneralSettings.Default.Language == "English")
                        MessageBox.Show("Please select an actor to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show("Пожалуйста, выберите актера для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Exit the method if no actors are selected
                }

                if (selectedActors[0] is string selectedActor)
                {
                    string message = string.Empty;
                    string header = string.Empty;
                    if (GeneralSettings.Default.Language == "English")
                    {
                        message = "Are you sure you want to delete the actor?";
                        header = "Notification";
                    }
                    else
                    {
                        message = "Вы уверены, что хотите удалить актера?";
                        header = "Уведомление";
                    }
                    var result = MessageBox.Show(message, header, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) return;

                    foreach (var entry in mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>())
                    {
                        if (entry.Actor == selectedActor)
                            UpdateActorNames(selectedActor, "𓈖Unknown_Actor𓈖");
                    }
                    RefreshActorsList("𓈖Unknown_Actor𓈖", selectedActor, 0);
                    JsonWriter.WriteCacheJson(); // Write the current state of the cache to a JSON file when the SetColorBtn is clicked

                    mainWindow.RegionManagerListView.Items.Refresh();

                    // Renumber all entries
                    for (int i = 0; i < mainWindow.ActorEnteries.Count; i++) // Iterate through all entries in the ActorEnteries list                
                        mainWindow.ActorEnteries[i].ActorsNumber = i + 1; // Set the ActorsNumber property of each entry to its index + 1                
                    mainWindow.ActorsListView.Items.Refresh(); // Refresh the ListView to reflect the changes made to the ActorsListView list
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ActorsProcessing", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        // --- Helper Methods ---

        private static void UpdateActorNames(string oldName, string newName)
        {
            try
            {
                foreach (var entry in mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>())
                {
                    if (entry.Actor == oldName)
                    {
                        entry.Actor = newName;
                    }
                }
                mainWindow.RegionManagerListView.Items.Refresh();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ActorsProcessing", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        private static void RefreshActorsList(string newName, string? removeName = null, int index = -1)
        {
            try
            {
                if (mainWindow.AvailableActors == null || mainWindow.ActorEnteries == null)
                    return;

                ActorsEnteries? oldEntry = null;

                // Remove old name from AvailableActors
                if (!string.IsNullOrEmpty(removeName))
                {
                    mainWindow.AvailableActors.Remove(removeName);

                    // Find and remove from ActorEnteries, store reference
                    oldEntry = mainWindow.ActorEnteries.FirstOrDefault(entry => entry.Actors == removeName);
                    if (oldEntry != null)
                    {
                        mainWindow.ActorEnteries.Remove(oldEntry);
                    }
                }

                // Update actor name in ActorsListView.ItemsSource
                if (!mainWindow.AvailableActors.Contains(newName))
                {
                    if (index != -1 && mainWindow.ActorsListView.ItemsSource is ObservableCollection<string> actorsSource)
                    {
                        if (index >= 0 && index < actorsSource.Count)
                            actorsSource[index] = newName;
                    }

                    mainWindow.AvailableActors.Add(newName);
                }

                // Check if newName already exists in ActorEnteries
                if (!mainWindow.ActorEnteries.Any(entry => entry.Actors == newName))
                {
                    var newEntry = new ActorsEnteries
                    {
                        ActorsNumber = index + 1,
                        Actors = newName,
                        ActorsColor = oldEntry?.ActorsColor,          // Preserve color
                        ActorsLineCount = oldEntry?.ActorsLineCount ?? 0 // Preserve line count
                    };

                    if (index >= 0 && index <= mainWindow.ActorEnteries.Count)
                    {
                        mainWindow.ActorEnteries.Insert(index, newEntry);
                    }
                    else
                    {
                        mainWindow.ActorEnteries.Add(newEntry);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ActorsProcessing", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }




        private static string GetUniqueName(string baseName, List<string> existingNames)
        {

            int counter = 1;
            string uniqueName = baseName;

            while (existingNames.Contains(uniqueName))
                uniqueName = $"{baseName} ({counter++})";

            return uniqueName;
        }
    }

}
