using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Mega_Subtitles_Reborn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string LogDirectoryPath = Path.Combine(ApplicationPath, "Errors");
        private static readonly string LogFilePath = Path.Combine(LogDirectoryPath, "AppCrash.log");
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Exit += App_Exit;

            if (!Directory.Exists(LogDirectoryPath))
            {
                Directory.CreateDirectory(LogDirectoryPath);
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //MessageBox.Show("An unexpected error occurred. The application will log this issue and try to recover.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            LogError("DispatcherUnhandledException", e.Exception);
            e.Handled = true;
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogError("UnhandledException", ex);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogError("UnobservedTaskException", e.Exception);
            e.SetObserved(); // Prevent the app from crashing immediately
        }

        private static void LogError(string exceptionType, Exception ex)
        {
            string logMessage = $"[{DateTime.Now}] {exceptionType}: {ex.Message}\n{ex.StackTrace}\n";

            File.AppendAllText(LogFilePath, logMessage);
        }


        private void App_Exit(object sender, ExitEventArgs e)
        {
            if (e.ApplicationExitCode != 0)
            {
                string logMessage = $"[{DateTime.Now}] Application exited with code {e.ApplicationExitCode}.\n";

                File.AppendAllText(LogFilePath, logMessage);
            }
        }
    }




}
