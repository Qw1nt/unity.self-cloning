#pragma warning disable RS1035

using System;
using System.IO;

namespace SourceGenerators.Generators.Utils;

public static class Logger
{
    private static readonly object Locker = new();
    private static readonly string RootDirectory;

    static Logger()
    {
        RootDirectory = Path.Combine(Path.GetTempPath(), "OppoSourceGenerators");
        Directory.CreateDirectory(RootDirectory);
    }

    public static void LogException(string pipelineName, string step, Exception exception)
    {
        lock (Locker)
        {
            var logFilePath = Path.Combine(RootDirectory, $"{pipelineName}.log");
            var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var threadId = Environment.CurrentManagedThreadId;
            var logLine = $"[{timeStamp}][{step}] [{threadId}] {exception.Message}{Environment.NewLine}{exception.StackTrace}";
            
            try
            {
                File.AppendAllText(logFilePath, logLine + Environment.NewLine);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}