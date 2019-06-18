using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
    internal static class Log
    {
        [Flags]
        public enum LogLevel
        {
            Debugging,
            Errors,
            Warnings,
            Info
        }

#if DEBUG
        public static LogLevel level = LogLevel.Debugging | LogLevel.Errors | LogLevel.Warnings | LogLevel.Info;
#else
		public static LogLevel level = LogLevel.Errors | LogLevel.Warnings | LogLevel.Info;
#endif

        private static string logName
        {
            get
            {
                return typeof(Log).Namespace;
            }
        }

        public static void Info(string info)
        {
            if (level.HasFlag(LogLevel.Info))
                LogOutput($"{TimeStamp()} [Info] [{logName}] {info}", ConsoleColor.Green);
        }

        public static void Error(string error)
        {
            if (level.HasFlag(LogLevel.Errors))
                LogOutput($"{TimeStamp()} [Error] [{logName}] {error}", ConsoleColor.Red);
        }

        public static void Warning(string warning)
        {
            if (level.HasFlag(LogLevel.Warnings))
                LogOutput($"{TimeStamp()} [Warning] [{logName}] {warning}", ConsoleColor.Yellow);
        }

        public static void Debug(string debug)
        {
            if (level.HasFlag(LogLevel.Debugging))
                LogOutput($"{TimeStamp()} [Debug] [{logName}] {debug}", ConsoleColor.Magenta);
        }

        public static void LogOutput(string output, ConsoleColor colour)
        {
            Console.CursorLeft = 0;
            Console.WriteLine();
            Console.CursorTop -= 1;

            Console.ForegroundColor = colour;
            Console.WriteLine(output);
            WriteToFile(output);

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void WriteToFile(string data)
        {
            File.AppendAllText($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}{logName}.log", data + Environment.NewLine);
        }

        private static void WriteToFile(string data, string file)
        {
            File.AppendAllText(file, data + Environment.NewLine);
        }

        private static string TimeStamp()
        {
            DateTime now = DateTime.Now;
            return $"[{now.ToShortDateString()} {now.ToLongTimeString()}]";
        }
    }
}
