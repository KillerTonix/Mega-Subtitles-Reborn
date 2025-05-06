using System.Collections.ObjectModel;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.Subtitles.AssProcessing
{
    public static class ActorsProcessing
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void SetActorColor()
        {
            var selectedActors = mainWindow.ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList();

            if (selectedActors[0] is string actorName)
            {
                var selectedColor = mainWindow.ColorPickerCombobox.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(actorName) || string.IsNullOrEmpty(selectedColor)) return;

                foreach (var entry in mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>())
                {
                    if (entry.Actor == actorName)
                    {
                        entry.Color = selectedColor;
                    }
                }
                foreach (var entry in mainWindow.ActorEnteries)
                {
                    if (entry.Actors == actorName)
                    {
                        entry.ActorsColor = selectedColor;
                    }
                }
                mainWindow.ActorsListView.Items.Refresh();
                mainWindow.RegionManagerListView.Items.Refresh();
            }
        }

        public static void RenameActor()
        {
            var selectedActors = mainWindow.ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList();

            if (selectedActors[0] is not string selectedActor)
            {
                MessageBox.Show("Please select an actor to rename.", "No Actor Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var item = mainWindow.ActorEnteries.Select((actor, index) => new { actor, index }).FirstOrDefault(a => a.actor.Actors == selectedActor);
            int index = item.index;
            string? newName = mainWindow.ActorTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("The name cannot be empty.", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool nameExists = mainWindow.AvailableActors.Contains(newName);
            if (nameExists)
            {
                var result = MessageBox.Show($"The actor '{newName}' already exists. Do you want to combine the actors?",
                                             "Actors Combine", MessageBoxButton.YesNo, MessageBoxImage.Question);

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

        public static void DeleteActor()
        {
            var selectedActors = mainWindow.ActorsListView.SelectedItems.Cast<ActorsEnteries>().Select(a => a.Actors).ToList();

            if (selectedActors[0] is string selectedActor)
            {
                var result = MessageBox.Show("Are you sure you want to delete the actor?", "Notification", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;

                foreach (var entry in mainWindow.subtitleViewSource.View.OfType<SubtitlesEnteries>())
                {
                    if (entry.Actor == selectedActor)
                        UpdateActorNames(selectedActor, "_Unknown_Actor_");
                }
                RefreshActorsList("_Unknown_Actor_", selectedActor, 0);

                mainWindow.RegionManagerListView.Items.Refresh();
            }
        }

        // --- Helper Methods ---

        private static void UpdateActorNames(string oldName, string newName)
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

        private static void RefreshActorsList(string newName, string? removeName = null, int index = -1)
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
