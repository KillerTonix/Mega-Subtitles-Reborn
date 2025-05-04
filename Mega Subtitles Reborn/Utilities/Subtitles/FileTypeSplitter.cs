using Mega_Subtitles_Reborn.Utilities.Subtitles.SrtProcessing;
using Mega_Subtitles_Reborn.Utilitis.Logger;
using Mega_Subtitles_Reborn.Utilitis.Subtitles.AssProcessing;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilitis.Subtitles
{
    internal class FileTypeSplitter
    {
        public static void TypeSplitter(string InputFilePath, string? InputFileType)
        {
            try
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (InputFileType != null)
                {
                    switch (InputFileType)
                    {
                        case ".ass":
                            AssParser.ParseAssFile(InputFilePath);
                            break;
                        case ".srt":
                            SrtParser.ParseSrtFile(InputFilePath);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex, "FileTypeSplitter", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
        }
    }
}
