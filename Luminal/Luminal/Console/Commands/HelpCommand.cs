using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Entities.Components;

namespace Luminal.Console.Commands
{
    [ConCommand("help", "Lists all console commands and variables, or gets info on one.")]
    [RequiredArgument("command or field", ArgumentType.String)]
    public class HelpCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            var thing = (string)a.Get("command or field");

            if (ConsoleManager.ConVars.ContainsKey(thing))
            {
                // This is a convar.
                var cv = ConsoleManager.ConVars[thing];

                var o = $"{cv.Name}: {cv.Description ?? "No description specified."}";
                
                DebugConsole.LogRaw(o);
            }

            if (ConsoleManager.Commands.ContainsKey(thing))
            {
                // This is a console command.
                var cmd = ConsoleManager.Commands[thing];

                var o = $"{cmd.Name}: {cmd.Description ?? "No description specified."}";

                DebugConsole.LogRaw(o);
            }
        }
    }
}
