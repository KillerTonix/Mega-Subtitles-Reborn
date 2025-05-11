using Mega_Subtitles_Reborn.Utilities.Subtitles;
using System;
using System.Windows;
using System.Windows.Input;

namespace Mega_Subtitles_Reborn.Utilities
{
    internal class RegionManagerLineUtil
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void DublicateOrDeleteLine(string type)
        {

            var selectedItems = mainWindow.RegionManagerListView.SelectedItems.Cast<SubtitlesEnteries>().ToList();

            foreach (var item in selectedItems)
            {
                switch (type)
                {
                    case "Dublicate":
                        mainWindow.SubtitleEntries.Insert(selectedItems.IndexOf(item), item);
                        break;
                    case "Delete":
                        mainWindow.SubtitleEntries.Remove(item);
                        break;
                }
            }

            mainWindow.RegionManagerListView.Items.Refresh();
        }

        public static void ClearComments()
        {
            foreach (var item in mainWindow.RegionManagerListView.SelectedItems.Cast<SubtitlesEnteries>())
            {
                item.Comment = string.Empty;
            }

            mainWindow.RegionManagerListView.Items.Refresh();
        }



        public static void ParseHotKeys(KeyEventArgs e)
        {
            if (mainWindow == null) return;
            int index = -1;

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
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
            else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
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
            if (index > 0)
                SetActorHotkeys(e, index);



        }

        public static void SetActorHotkeys(KeyEventArgs e, int index = -1)
        {

            if (index < 0) return;

            if (mainWindow.AvailableActors == null || index >= mainWindow.AvailableActors.Count)
                return;

            string actorToAssign = mainWindow.AvailableActors[index];

            foreach (var item in mainWindow.RegionManagerListView.SelectedItems)
            {
                if (item is SubtitlesEnteries entry)
                {
                    entry.Actor = actorToAssign;
                }
            }

            mainWindow.RegionManagerListView.Items.Refresh();
            e.Handled = true;
        }

    }
}
