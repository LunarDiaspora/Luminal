using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Logging
{
    public struct LogEntry
    {
        public string Text;
        public LogLevel Level;
    }

    public class DebugConsoleLog
    {
        public static List<LogEntry> Entries = new();

        public static void Add(LogEntry e) => Entries.Add(e);
        public static void Clear() => Entries.Clear();
    }
}
