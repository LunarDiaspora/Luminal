using System.Linq;

namespace Luminal.Logging
{
    public class Log
    {
        public static ILogger Logger;
        public static int MinLevel;

        public static void SetLogger(ILogger logger)
        {
            Logger = logger;
        }

        public static void SetLogLevel(int logLevel)
        {
            MinLevel = logLevel;
        }

        public static void Debug(string msg)
        {
            Logger.Log(msg, LogLevel.Debug);
        }

        public static void Debug(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.Debug);
        }

        public static void Info(string msg)
        {
            Logger.Log(msg, LogLevel.Info);
        }

        public static void Info(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.Info);
        }

        public static void Warn(string msg)
        {
            Logger.Log(msg, LogLevel.Warning);
        }

        public static void Warn(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.Warning);
        }

        public static void Error(string msg)
        {
            Logger.Log(msg, LogLevel.Error);
        }

        public static void Error(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.Error);
        }

        public static void Fatal(string msg)
        {
            Logger.Log(msg, LogLevel.Fatal);
        }

        public static void Fatal(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.Fatal);
        }

        public static void Wtf(string msg)
        {
            Logger.Log(msg, LogLevel.Unexpected);
        }

        public static void Wtf(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.Unexpected);
        }
    }
}