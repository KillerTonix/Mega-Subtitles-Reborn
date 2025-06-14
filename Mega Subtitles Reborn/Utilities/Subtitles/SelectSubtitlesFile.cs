using Mega_Subtitles_Reborn;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace Mega_Subtitles.Helper.Subtitles
{
    public class SelectSubtitlesFile
    {
        public static (string?, string?) ChooseFile()
        {
            try
            {  
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "Subtitles (*.ass; *.srt;)| *.ass; *.srt;",
                    Title = "Select the subtitle file",
                    Multiselect = false,
                    CheckFileExists = true,   // Ensure selected file exists
                    CheckPathExists = true    // Ensure selected path exists
                };

                // Show the dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == true)
                {
                    GeneralSettings.Default.SubtitlesPath = openFileDialog.FileName;
                    GeneralSettings.Default.Save();
                    return (openFileDialog.FileName, Path.GetExtension(openFileDialog.FileName).ToLowerInvariant());
                }
                else
                    return (null, null);                
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "SelectSubtitlesFile", MethodBase.GetCurrentMethod()?.Name);
                return (null, null);
            }
        }
    }
}
