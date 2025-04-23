using Mega_Subtitles_Reborn.Utilitis.Logger;
using Microsoft.Win32;
using System.IO;

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
                    Filter = "Subtitles (*.ass; *.srt; *.vtt)| *.ass; *.srt; *.vtt",
                    Title = "Select the subtitle file",
                    Multiselect = false,
                    CheckFileExists = true,   // Ensure selected file exists
                    CheckPathExists = true    // Ensure selected path exists
                };

                // Show the dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == true)                                   
                    return (openFileDialog.FileName, Path.GetExtension(openFileDialog.FileName));                
                else                
                    return (null, null);                
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "SelectSubtitlesFile", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
                return (null, null);
            }
        }
    }
}
