using Mega_Subtitles_Reborn.Utilities.Subtitles;
using System.Windows;

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
    }
}
