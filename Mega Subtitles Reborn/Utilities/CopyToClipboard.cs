using Mega_Subtitles_Reborn.Utilities.Subtitles;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Reflection;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilities
{
    internal class CopyToClipboard
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void CopyText(string type)
        {
            try
            {
                var selectedItems = mainWindow.RegionManagerListView.SelectedItems.Cast<SubtitlesEnteries>().ToList();

                string timings = string.Empty;
                string text = string.Empty;
                string comments = string.Empty;

                foreach (var item in selectedItems)
                {
                    timings += $"{item.Start} --> {item.End}";
                    text += $"{item.Text}";
                    comments += $"{item.Comment}";
                }

                switch (type)
                {
                    case "timings":
                        Clipboard.SetText(timings);
                        break;
                    case "text":
                        Clipboard.SetText(text);
                        break;
                    case "comments":
                        Clipboard.SetText(comments);
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "CopyToClipboard", MethodBase.GetCurrentMethod()?.Name); // Log any exceptions 
            }
        }
    }
}
