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
            Logger.Log(msg, LogLevel.DEBUG);
        }

        public static void Debug(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.DEBUG);
        }

        public static void Info(string msg)
        {
            Logger.Log(msg, LogLevel.INFO);
        }

        public static void Info(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.INFO);
        }

        public static void Warn(string msg)
        {
            Logger.Log(msg, LogLevel.WARNING);
        }

        public static void Warn(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.WARNING);
        }

        public static void Error(string msg)
        {
            Logger.Log(msg, LogLevel.ERROR);
        }

        public static void Error(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.ERROR);
        }

        public static void Fatal(string msg)
        {
            Logger.Log(msg, LogLevel.FATAL);
        }

        public static void Fatal(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.FATAL);
        }

        public static void Wtf(string msg)
        {
            Logger.Log(msg, LogLevel.UNEXPECTED);
        }

        public static void Wtf(string msg, params object[] args)
        {
            Logger.Log(string.Format(msg, args.Select(t => t.ToString()).ToArray()), LogLevel.UNEXPECTED);
        }
    }
}