using Luminal.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Console.Commands
{
    [ConCommand("toggleconsole", "Toggles the console.")]
    public class ToggleConsoleCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            if (Engine.EnableConsole)
                Engine.ConsoleOpen = !Engine.ConsoleOpen;
        }
    }

    [ConCommand("quit", "Shuts down Luminal and exits.")]
    [OptionalArgument("code", ArgumentType.Integer)]
    public class QuitCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            Engine.Quit(a.Get("code", 0));
        }
    }

    [ConCommand("echo", "Prints what you type.")]
    [OverflowArgument("text")]
    public class EchoCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            DebugConsole.LogRaw(a.Get("text", ""));
        }
    }

    [ConCommand("bind", "Binds an action to a key.")]
    [RequiredArgument("key", ArgumentType.String)]
    [OverflowArgument("command")]
    public class BindCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            var c = ((string)a.Get("command")).Trim();
            try
            {
                var kc = Engine.StringToScancode(a.Get("key"));

                if (c.Length == 0)
                {
                    var has = ConsoleManager.Binds.TryGetValue(kc, out string bind);
                    if (!has)
                    {
                        DebugConsole.LogRaw($@"Key ""{a.Get("key")}"" is not bound.");
                        return;
                    }
                    DebugConsole.LogRaw($@"Key ""{a.Get("key")}"" is bound to ""{bind}"".");
                    return;
                }

                ConsoleManager.BindKeyCode(kc, c);
            } catch(ArgumentException)
            {
                DebugConsole.LogRaw($@"Failed to parse key ""{a.Get("key")}"".");
            }
        }
    }

    [ConCommand("alias", "Aliases a name to a command.")]
    [RequiredArgument("name", ArgumentType.String)]
    [OverflowArgument("command")]
    public class AliasCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            var t = a.Get("name");
            var c = ((string)a.Get("command")).Trim();
            if (c.Length == 0)
            {
                var has = ConsoleManager.Aliases.TryGetValue(t, out string v);
                if (has)
                {
                    DebugConsole.LogRaw($@"""{t}"" = ""{v}""");
                    return;
                } else
                {
                    DebugConsole.LogRaw($@"Alias ""{t}"" does not exist.");
                    return;
                }
            }

            ConsoleManager.Aliases[t] = c;
            DebugConsole.LogRaw($@"""{t}"" = ""{c}""");
        }
    }

    [ConCommand("debug_winapi_get_workarea")]
    public class WorkAreaCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            WinAPIUtilities.WorkArea();
        }
    }

    [ConCommand("clear", "Clears the console.")]
    public class ClearCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            DebugConsole.ConsoleOutput.Clear();
        }
    }

    [ConCommand("list_cvar", "Lists every console variable.")]
    public class CvarListCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            var t = "";
            foreach (var (name, cv) in ConsoleManager.ConVars)
            {
                var prop = cv.GetPropString();
                t += $"{name} ({cv.ValueType.ToString().ToLower()}) = \"{cv.GetValue()}\"" +
                    $"\n{(prop.Length > 0 ? prop + "\n" : "")}" +
                    $" - {cv.Description ?? "No description provided."}\n\n";
            }
            DebugConsole.LogRaw(t.Trim());
        }
    }

    [ConCommand("list_command", "Lists every console command.")]
    public class CommandListCommand : IConCommand
    {
        public void Run(Arguments a)
        {
            var t = "";
            foreach (var (name, cv) in ConsoleManager.Commands)
            {
                t += $"{name}\n - {cv.Description ?? "No description provided."}\n\n";
            }
            DebugConsole.LogRaw(t.Trim());
        }
    }
}
