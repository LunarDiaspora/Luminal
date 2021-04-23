using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Luminal.Logging
{
    public class ConsoleLogger : ILogger
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        public ConsoleLogger()
        {
            var stdoutHandle = GetStdHandle(-11); // STD_OUTPUT_HANDLE
            uint mode;
            GetConsoleMode(stdoutHandle, out mode);
            mode |= 4; // ENABLE_VIRTUAL_TERMINAL_PROCESSING, allows ANSI colour escaping
            SetConsoleMode(stdoutHandle, mode);

            //Console.WriteLine($"{Clr(255, 128, 128)}hello, world!{Rst()}");
        }

        public static string _DoSGR(string sgr)
        {
            return $"\x1B[{sgr}m"; // 0x1B = ESC
        }

        public static string Clr(uint r = 255, uint g = 255, uint b = 255)
        {
            return _DoSGR($"38;2;{r};{g};{b}");
        }

        public static string Rst()
        {
            return _DoSGR($"0");
        }

        public static readonly string C_DG = Clr(105, 105, 105);

        public static readonly string C_DEBUG = Clr(105, 105, 105);
        public static readonly string C_INFO = Clr(0, 214, 200);
        public static readonly string C_WARN = Clr(219, 179, 0);
        public static readonly string C_ERROR = Clr(214, 71, 71);
        public static readonly string C_FATAL = Clr(237, 0, 0);
        public static readonly string C_WTF = Clr(237, 12, 207);

        public Dictionary<LogLevel, string> Formats = new()
        {
            { LogLevel.DEBUG, $"{C_DG}[DEBUG]{Rst()} {{0}}" },
            { LogLevel.INFO, $"{C_DG}[ {C_INFO}INFO{C_DG}]{Rst()} {{0}}" },
            { LogLevel.WARNING, $"{C_DG}[ {C_WARN}WARN{C_DG}]{Rst()} {{0}}" },
            { LogLevel.ERROR, $"{C_DG}[{C_ERROR}ERROR{C_DG}]{Rst()} {{0}}" },
            { LogLevel.FATAL, $"{C_DG}[ {C_FATAL}CRIT{C_DG}]{Rst()} {{0}}" },
            { LogLevel.UNEXPECTED, $"{C_DG}[{C_WTF}WHAT?{C_DG}]{Rst()} {{0}}" },
        };

        public void Log(string msg, LogLevel ll)
        {
            var intlevel = (int)ll;
            if (intlevel < Logging.Log.MinLevel)
            {
                return;
            }

            var fmt = Formats[ll];
            if (msg != null)
            {
                var s = msg.Split("\n");
                foreach (var l in s)
                    System.Console.WriteLine(string.Format(fmt, l));
            }
            else
            {
                System.Console.WriteLine(string.Format(fmt, "null"));
            }
        }
    }
}