using Mega_Subtitles_Reborn.Utilitis.Logger;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Mega_Subtitles_Reborn.Utilities
{
    public class ListViewColumnsAutoResize
    {
        private static readonly MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public static void AutoResizeColumns()
        {
            try
            {
                ListView? listView = mainWindow.RegionManagerListView;
                if (listView.View is GridView gridView)
                {
                    foreach (var column in gridView.Columns)
                    {
                        if (double.IsNaN(column.Width))
                        {
                            column.Width = column.ActualWidth;
                        }
                        column.Width = double.NaN;
                    }
                }
                ListView? actorsListView = mainWindow.ActorsListView;
                if (actorsListView.View is GridView actorsGridView)
                {
                    foreach (var column in actorsGridView.Columns)
                    {
                        if (double.IsNaN(column.Width))
                        {
                            column.Width = column.ActualWidth;
                        }
                        column.Width = double.NaN;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "ListViewColumnsAutoResize", MethodBase.GetCurrentMethod()?.Name);
            }
        }
    }
}
