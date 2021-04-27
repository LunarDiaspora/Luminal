using Luminal.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message, LogLevel level)
        {
            foreach (var s in message.Split("\n"))
            {
                System.Console.WriteLine($"{level}: {message}");

                var v = new DebugConsole.ConsoleLine()
                {
                    data = s,
                    level = level
                };
                DebugConsole.ConsoleOutput.Add(v);
            }

            DebugConsole.ScrollDown();
        }
    }
}
