﻿using Discord;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Utilities.LoggingService
{
    using System.Runtime.CompilerServices;
    using Utilities.RainbowUtilities;

    public static class Logging
    {
        private static readonly string _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        private static readonly string _logFile = Path.Combine(_logDirectory, $"{DateTime.UtcNow:yyyy-MM-dd}.txt");

        public static Task LogAsync(LogMessage msg)
        {
            if (!Directory.Exists(_logDirectory))     // Create the log directory if it doesn't exist
                Directory.CreateDirectory(_logDirectory);
            if (!File.Exists(_logFile))               // Create today's log file if it doesn't exist
                File.Create(_logFile).Dispose();

            string logText = $"{DateTime.UtcNow:hh:mm:ss} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            try
            {
                File.AppendAllText(_logFile, logText + "\n");     // Write the log text to a file
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd} [Critical - Ignore] {e.Message}");
            }

            Console.ForegroundColor = msg.Severity switch
            {
                LogSeverity.Critical => ConsoleColor.DarkRed,
                LogSeverity.Error => ConsoleColor.Red,
                LogSeverity.Warning => ConsoleColor.Yellow,
                LogSeverity.Info => ConsoleColor.Cyan,
                LogSeverity.Verbose => ConsoleColor.Blue,
                LogSeverity.Debug => ConsoleColor.Green,
                _ => ConsoleColor.Magenta,
            };
            return Console.Out.WriteLineAsync(logText);       // Write the log text to the console
        }

        public static Task LogReadyMessage<T>(T Class)
        {
            string source = Class.GetType().Name;
            LogMessage logMessage = new LogMessage(LogSeverity.Info, source, "Ready");
            return LogAsync(logMessage);
        }

        public static Task LogDebugMessage(string source, string message)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Debug, source, message);
            return LogAsync(logMessage);
        }

        public static Task LogErrorMessage(string source, string message)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Error, source, message);
            return LogAsync(logMessage);
        }

        public static Task LogCriticalMessage(string source, string message)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Critical, source, message);
            return LogAsync(logMessage);
        }

        public static Task LogInfoMessage(string source, string message)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Info, source, message);
            return LogAsync(logMessage);
        }

        public static Task LogWarningMessage(string source, string message)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Warning, source, message);
            return LogAsync(logMessage);
        }


        public static void LogRainbowMessage(string source, string message)
        {
            if (!Directory.Exists(_logDirectory))     // Create the log directory if it doesn't exist
                Directory.CreateDirectory(_logDirectory);
            if (!File.Exists(_logFile))               // Create today's log file if it doesn't exist
                File.Create(_logFile).Dispose();

            string logText = $"{DateTime.UtcNow:hh:mm:ss} [Rainbow] {source}: {message}";
            try
            {
                File.AppendAllText(_logFile, logText + "\n");     // Write the log text to a file
            }

            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Critical - Ignore] {e.Message}");
            }

            foreach(char c in logText)
            {
                Console.ForegroundColor = RainbowUtilities.CreateConsoleRainbowColor();
                Console.Write(c);
            }
            Console.WriteLine();
        }
    }
}