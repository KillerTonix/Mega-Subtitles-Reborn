using Mega_Subtitles_Reborn.Utilitis.Logger;
using Microsoft.Win32;
using System.Reflection;

namespace Mega_Subtitles_Reborn.Utilities.FileReader
{
    internal class SubtitlesDistributionTermplateReader
    {
        public static string GetTemplate()
        {
            try
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Template (*.txt)| *.txt;",
                    Title = "Select the template file",
                    Multiselect = false,
                    CheckFileExists = true,   // Ensure selected file exists
                    CheckPathExists = true    // Ensure selected path exists
                };

                // Show the dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == true)
                    return openFileDialog.FileName; // Return the selected file path

                return string.Empty; // Return an empty string if no file was selected

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "SubtitlesDistributionTermplateReader", MethodBase.GetCurrentMethod()?.Name);
                return string.Empty; // Return an empty string if an error occurs
            }
        }
    }
}
