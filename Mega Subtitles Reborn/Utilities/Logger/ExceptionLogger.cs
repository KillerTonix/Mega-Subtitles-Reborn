using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Mega_Subtitles_Reborn.Utilitis.Logger
{
    class ExceptionLogger
    {
        private static readonly string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string LogDirectoryPath = Path.Combine(ApplicationPath, "Errors");
        private static readonly string LogFilePath = Path.Combine(LogDirectoryPath, "ExceptionLogs.log");

        public static void LogException(Exception ex, string? className, string? functionName)
        {

            try
            {
                DirectoryOrFileCheck.DirectoryCheck(LogDirectoryPath); //Check if directory wasnt exist then create.
                
                using StreamWriter writer = new(LogFilePath, true);
                writer.WriteLine("DateTime: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                writer.WriteLine("Class Name: " + className);
                writer.WriteLine("Function Name: " + functionName);
                writer.WriteLine("Exception Message: " + ex.Message);
                writer.WriteLine("Stack Trace: " + ex.StackTrace);
                writer.WriteLine("----------------------------------------");
                writer.WriteLine();
                MessageBox.Show($"Вовремя работы программы возникла ошибка.\nПодробнее об ошибке можно прочитать в файле{LogFilePath}", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.Start("explorer.exe", "\"" + LogFilePath + "\"");
            }
            catch (Exception loggingEx)
            {
                // Optionally, handle logging failure here
                MessageBox.Show("Failed to log exception: " + loggingEx.Message);
            }
        }
    }
}
