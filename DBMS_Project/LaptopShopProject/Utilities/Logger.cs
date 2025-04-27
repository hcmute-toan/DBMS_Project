using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopShopProject.Utilities
{
    public static class Logger
    {
        private static readonly string LogFilePath = "log.txt";

        public static void LogError(string message, Exception ex = null)
        {
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - ERROR: {message}";
                if (ex != null)
                    logMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
                File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                // Ignore logging errors
            }
        }

        public static void LogInfo(string message)
        {
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - INFO: {message}";
                File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}
