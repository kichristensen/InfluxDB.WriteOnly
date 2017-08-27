using System;

namespace InfluxDB.WriteOnly
{
    public class DefaultLogger : ILogger
    {
        public void Trace(string message)
        {
            System.Diagnostics.Debug.WriteLine(FormatMessage("TRACE", message));
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(FormatMessage("DEBUG", message));
        }

        public void Info(string message)
        {
            Console.WriteLine(FormatMessage("INFO", message));
        }

        public void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(FormatMessage("WARNING", message));
            Console.ResetColor();
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(FormatMessage("ERROR", message));
            Console.ResetColor();
        }

        private static string FormatMessage(string logLevel, string message)
        {
            return $"${DateTime.Now:g}\t${logLevel}: ${message}";
        }
    }
}