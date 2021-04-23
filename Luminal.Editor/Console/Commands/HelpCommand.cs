using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Editor.Console.Commands
{
    [ConCommand("help", "Lists all console commands and variables, or gets info on one.")]
    [OptionalArgument("command or field", ArgumentType.STRING)]
    public class HelpCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            var thing = (string)a.Get("command or field");

            if (thing == null)
            {
                var o = "";
                foreach (var attr in ConsoleManager.Commands)
                    o += $"{attr.Key}: {attr.Value.Description ?? "No description specified."}\n";
                foreach (var attr in ConsoleManager.ConVarAttrs)
                    o += $"{attr.Key}: {attr.Value.Description ?? "No description specified."}\n";

                Editor.LogRaw(o.Trim());

                return;
            }

            if (ConsoleManager.ConVars.ContainsKey(thing))
            {
                // This is a convar.
                var cv = ConsoleManager.ConVarAttrs[thing];

                var o = $"{cv.Name}: {cv.Description ?? "No description specified."}";
                
                Editor.LogRaw(o);
            }

            if (ConsoleManager.Commands.ContainsKey(thing))
            {
                // This is a console command.
                var cmd = ConsoleManager.Commands[thing];

                var o = $"{cmd.Name}: {cmd.Description ?? "No description specified."}";

                Editor.LogRaw(o);
            }


        }
    }
}
