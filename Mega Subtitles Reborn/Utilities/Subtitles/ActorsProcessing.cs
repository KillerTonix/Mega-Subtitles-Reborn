using Mega_Subtitles_Reborn.Utilitis.Subtitles.AssProcessing;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities.Subtitles.AssProcessing
{
    public static class ActorsProcessing
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void SetActorColor()
        {
            if (mainWindow.ActorsList.SelectedItem is string actorName)
            {
                var selectedColor = mainWindow.ColorPickerCombobox.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(actorName) || string.IsNullOrEmpty(selectedColor)) return;

                foreach (var entry in mainWindow.subtitleViewSource.View.OfType<AssSubtitlesEnteries>())
                {
                    if (entry.Actor == actorName)
                    {
                        entry.Color = selectedColor;
                    }
                }

                mainWindow.RegionManagerListView.Items.Refresh();
            }
        }

        public static void RenameActor()
        {
            if (mainWindow.ActorsList.SelectedItem is not string selectedActor)
            {
                MessageBox.Show("Please select an actor to rename.", "No Actor Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int index = mainWindow.ActorsList.Items.IndexOf(selectedActor);

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
                    string? targetColor = mainWindow.subtitleViewSource.View.OfType<AssSubtitlesEnteries>()
                        .FirstOrDefault(e => e.Actor == newName && e.Color != null)?.Color;

                    foreach (var entry in mainWindow.subtitleViewSource.View.OfType<AssSubtitlesEnteries>().Where(e => e.Actor == selectedActor))
                    {
                        entry.Actor = newName;
                        if (targetColor != null)
                            entry.Color = targetColor;
                    }
                    mainWindow.AvailableActors?.Remove(selectedActor);
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
            if (mainWindow.ActorsList.SelectedItem is string selectedActor)
            {

                var result = MessageBox.Show("Are you sure you want to delete the actor?", "Notification", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;                

                foreach (var entry in mainWindow.subtitleViewSource.View.OfType<AssSubtitlesEnteries>())
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
            foreach (var entry in mainWindow.subtitleViewSource.View.OfType<AssSubtitlesEnteries>())
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
            if (!string.IsNullOrEmpty(removeName))
                mainWindow.AvailableActors?.Remove(removeName);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (!mainWindow.AvailableActors.Contains(newName))
            {
                if (index != -1)
                    ((ObservableCollection<string>)mainWindow.ActorsList.ItemsSource)[index] = newName;
                mainWindow.AvailableActors?.Add(newName);
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
