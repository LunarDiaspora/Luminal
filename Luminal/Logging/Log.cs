using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Logging
{
    public class Log
    {
        private static void AddLog(LogLevel l, string text)
        {
            var s = text.Split('\n');
            foreach (var i in s)
            {
                var ent = new LogEntry()
                {
                    Text = i,
                    Level = l
                };
                DebugConsoleLog.Add(ent);
            }
        }

        public static void Debug(object m)
            => AddLog(LogLevel.Debug, m.ToString());
        public static void Info(object m)
            => AddLog(LogLevel.Info, m.ToString());
        public static void Warn(object m)
            => AddLog(LogLevel.Warning, m.ToString());
        public static void Error(object m)
            => AddLog(LogLevel.Error, m.ToString());
        public static void Critical(object m)
            => AddLog(LogLevel.Critical, m.ToString());
        public static void Wtf(object m)
            => AddLog(LogLevel.Unexpected, m.ToString());
    }
}
