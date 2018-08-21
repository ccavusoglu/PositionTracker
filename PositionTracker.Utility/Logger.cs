using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace PositionTracker.Utility
{
    /// <summary>
    /// Shameless Logger. Not better than log4net.
    /// </summary>
    public class Logger
    {
        public static LogLevel LogLevel;
        public static bool WriteToFile;

        private const int LogBufferSize = 65536;
        private static readonly string FilePath;
        private static readonly StringBuilder LogCache;

        static Logger()
        {
            LogLevel = LogLevel.Fatal | LogLevel.Error | LogLevel.Info |
                       LogLevel.Debug | LogLevel.Order | LogLevel.Perf | LogLevel.Proxy;
            WriteToFile = true;
            FilePath = $"{FileHelper.FilesDir}/Logs-{DateTime.Now:ddMMyyyy}.calog";
            LogCache = new StringBuilder();
        }

        public static void FlushLogger()
        {
            FileHelper.WriteToFile(LogCache.ToString(), FilePath);
            LogCache.Clear();
        }

        public static void LogDebug(string text, [CallerFilePath] string file = "", [CallerMemberName] string method = "")
        {
            Log(LogLevel.Debug, text, file, method);
        }

        public static void LogError(string text, Exception exception, [CallerFilePath] string file = "",
            [CallerMemberName] string method = "")
        {
            var stack = exception.StackTrace;
            var message = exception.Message;
            var innerException = exception.InnerException;

            while (innerException != null)
            {
                message += $"{Environment.NewLine}---------------------{Environment.NewLine}{innerException.Message}";
                stack += $"{Environment.NewLine}---------------------{Environment.NewLine}{innerException.StackTrace}";

                innerException = innerException.InnerException;
            }

            text = $"{text} Exception: {message} Stack: {stack}";
            Log(LogLevel.Error, text, file, method);
        }

        public static void LogFatal(string text, [CallerFilePath] string file = "", [CallerMemberName] string method = "")
        {
            Log(LogLevel.Fatal, text, file, method);
        }

        public static void LogInfo(string text, [CallerFilePath] string file = "", [CallerMemberName] string method = "")
        {
            Log(LogLevel.Info, text, file, method);
        }

        public static void LogOrder(string text, [CallerFilePath] string file = "", [CallerMemberName] string method = "")
        {
            Log(LogLevel.Order, text, file, method);
        }

        public static void LogPerf(string text, [CallerFilePath] string file = "", [CallerMemberName] string method = "")
        {
            Log(LogLevel.Perf, text, file, method);
        }

        public static void LogProxy(string text, [CallerFilePath] string file = "", [CallerMemberName] string method = "")
        {
            Log(LogLevel.Proxy, text, file, method);
        }

        public static void LogProxy(string text, object message, [CallerFilePath] string file = "",
            [CallerMemberName] string method = "")
        {
            var s = "";

            if (message != null)
                s = JsonConvert.SerializeObject(message);

            Log(LogLevel.Proxy, $"{text} {s}", file, method);
        }

        public static void LogTrace(string text, [CallerFilePath] string file = "", [CallerMemberName] string method = "")
        {
            Log(LogLevel.Trace, text, file, method);
        }

        private static string FormatLog(LogLevel logLevel, string log, string file, string method)
        {
            var fileName = file;

            if (file != null && file.Contains("\\")) { fileName = file.TrimEnd(".cs".ToCharArray()).Split('\\').Last(); }

            return $"{DateTime.Now:dd/MM/yyyy HH:mm:ss:fff}|{logLevel.ToString().PadRight(5)}|[{fileName}::{method}]|{log}";
        }

        private static void Log(LogLevel logLevel, string text, string file, string method)
        {
            var log = FormatLog(logLevel, text, file, method);

            if (WriteToFile) PersistLog(log);

            if (LogLevel.HasFlag(logLevel)) Console.WriteLine(log);
        }

        private static void PersistLog(string log)
        {
            LogCache.Append(log);

            if (LogCache.Length >= LogBufferSize)
            {
                if (FileHelper.WriteToFile(LogCache.ToString(), FilePath)) LogCache.Clear();
            }
        }
    }

    [Flags]
    public enum LogLevel
    {
        None = 0,
        Fatal = 1 << 0,
        Error = 1 << 1,
        Info = 1 << 2,
        Debug = 1 << 3,
        Order = 1 << 4,
        Perf = 1 << 5,
        Proxy = 1 << 6,
        Trace = 1 << 7
    }
}