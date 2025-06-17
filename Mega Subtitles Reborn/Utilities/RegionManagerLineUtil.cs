using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.FileWriter;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Mega_Subtitles_Reborn.Utilities
{
    internal class RegionManagerLineUtil
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void DublicateOrDeleteLine(string type)
        {
            try
            {
                var selectedItems = mainWindow.RegionManagerListView.SelectedItems.Cast<SubtitlesEnteries>().ToList(); // Get selected items from the ListView
                var listViewItems = mainWindow.RegionManagerListView.Items.Cast<SubtitlesEnteries>().ToList(); // Get all items from the ListView
                var entries = new List<SubtitlesEnteries>();

                foreach (var item in selectedItems) // Iterate through each selected item
                {
                    int index = listViewItems.IndexOf(item); // Get the index of the selected item in the ListView
                    var data = new SubtitlesEnteries //Create a new instance of SubtitlesEnteries
                    {
                        Number = item.Number,
                        Start = item.Start,
                        End = item.End,
                        Text = item.Text,
                        Layer = item.Layer,
                        Style = item.Style,
                        Effect = item.Effect,
                        Actor = item.Actor,
                        Comment = item.Comment,
                        Color = item.Color
                    };

                    switch (type)
                    {
                        case "Dublicate": // If the type is "Dublicate", create a new entry with the same properties

                            mainWindow.SubtitleEntries.Insert(index, data); // Insert the new entry at the same index as the original item
                            break;
                        case "Delete":
                            mainWindow.SubtitleEntries.Remove(item); // If the type is "Delete", remove the selected item from the list
                            entries.Add(data); // Add the deleted item to the entries list for further processing
                            break;
                    }
                }
                JsonWriter.WriteDeletedLinesJson(entries, GeneralSettings.Default.DeletedLinesJsonPath); // Write the deleted item to a JSON file

                // Renumber all entries
                for (int i = 0; i < mainWindow.SubtitleEntries.Count; i++) // Iterate through all entries in the SubtitleEntries list
                {
                    mainWindow.SubtitleEntries[i].Number = i + 1; // Set the Number property of each entry to its index + 1
                }
                mainWindow.RegionManagerListView.Items.Refresh(); // Refresh the ListView to reflect the changes made to the SubtitleEntries list
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "RegionManagerLineUtil", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }


        public static void ClearComments()
        {
            try
            {
                foreach (var item in mainWindow.RegionManagerListView.SelectedItems.Cast<SubtitlesEnteries>()) // Iterate through each selected item in the ListView
                {
                    item.Comment = string.Empty; // Set the Comment property of the selected item to an empty string
                }

                mainWindow.RegionManagerListView.Items.Refresh(); // Refresh the ListView to reflect the changes made to the Comment properties of the selected items
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "RegionManagerLineUtil", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }



        public static void ParseHotKeys(KeyEventArgs e)
        {
            try
            {
                if (mainWindow == null) return;
                int index = -1;

                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) // Check if the Control key is pressed
                {
                    index = e.Key switch
                    {
                        Key.D1 => 0,
                        Key.D2 => 1,
                        Key.D3 => 2,
                        Key.D4 => 3,
                        Key.D5 => 4,
                        Key.D6 => 5,
                        Key.D7 => 6,
                        Key.D8 => 7,
                        Key.D9 => 8,
                        Key.D0 => 9,
                        _ => -1
                    };
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) // Check if the Shift key is pressed
                {
                    index = e.Key switch
                    {
                        Key.D1 => 10,
                        Key.D2 => 11,
                        Key.D3 => 12,
                        Key.D4 => 13,
                        Key.D5 => 14,
                        Key.D6 => 15,
                        Key.D7 => 16,
                        Key.D8 => 17,
                        Key.D9 => 18,
                        Key.D0 => 19,
                        _ => -1
                    };
                }
                if (index > -1)
                    SetActorHotkeys(e, index);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "RegionManagerLineUtil", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

        public static void SetActorHotkeys(KeyEventArgs e, int index = -1)
        {
            try
            {
                if (index < 0) return; // Ensure index is valid

                if (mainWindow.AvailableActors == null || index >= mainWindow.AvailableActors.Count) // Check if AvailableActors is null or index is out of bounds
                    return;

                string actorToAssign = mainWindow.AvailableActors[index]; // Get the actor to assign based on the index

                foreach (var item in mainWindow.RegionManagerListView.SelectedItems) // Iterate through each selected item in the ListView
                {
                    if (item is SubtitlesEnteries entry) // Check if the item is of type SubtitlesEnteries
                    {
                        entry.Actor = actorToAssign;
                    }
                }

                mainWindow.RegionManagerListView.Items.Refresh(); // Refresh the ListView to reflect the changes made to the Actor properties of the selected items
                e.Handled = true; // Mark the event as handled to prevent further processing
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "RegionManagerLineUtil", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }

    }
}
