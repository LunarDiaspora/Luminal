using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Luminal.Editor.Console
{
    public static class ConsoleManager
    {
        public static Dictionary<string, ConCommandContainer> Commands = new();

        public static void RegisterCommand(ConCommandContainer header)
        {
            Commands.Add(header.Name, header);
        }

        public static string GetUsage(List<Argument> a)
        {
            var j = new List<string>();
            foreach (var t in a)
            {
                var r = !t.Optional;
                j.Add($"{(r ? '<' : '[')}{t.Name}{(r ? '>' : ']')}");
            }
            return string.Join(' ', j);
        }

        public static Arguments BuildArgs(List<string> ina, List<Argument> desired)
        {
            var arg = new Arguments();

            for (int i=0; i<desired.Count; i++)
            {
                var wanted = desired[i];
                if ((ina.Count <= i) && !wanted.Optional)
                    throw new ArgumentOutOfRangeException();

                if (ina.Count <= i)
                    break; // No more of them

                var p = ina[i];
                var n = new ReceiveArgument(wanted.Type, wanted.Name);
                n.Parse(p);
                arg.values.Add(wanted.Name, n);
            }

            return arg;
        }

        public static Arguments BuildArgs(List<string> ina, ConCommandContainer ccc)
        {
            var a = ccc.Arguments ?? new();
            try
            {
                return BuildArgs(ina, a);
            } catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Usage: {ccc.Name} {GetUsage(a)}");
            }
        }

        public static void RunConsole(string commandName, List<string> inp)
        {
            if (!Commands.ContainsKey(commandName))
                throw new ArgumentException($"No such command: \"{commandName}\"");
            var command = Commands[commandName];
            var a = BuildArgs(inp, command);
            command.Command.Run(a);
        }
        
        public static void FindConCommands()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var t in asm.GetTypes())
            {
                // Make sure this is actually a console command
                if (!t.IsAssignableTo(typeof(IConCommand)))
                    continue;

                // Oops.
                if (t == typeof(IConCommand))
                    continue;

                // Get the concommand header
                var j = t.GetCustomAttribute<ConCommandAttribute>();
                var aa = t.GetCustomAttributes(false);
                var atg = new List<Argument>();
                foreach (var h in aa)
                {
                    if (h.GetType().IsAssignableTo(typeof(ArgumentAttribute)))
                    {
                        // This is an argument
                        var argobj = (ArgumentAttribute)h;
                        atg.Add(argobj.ToArg());
                    }
                }

                // Explode if that doesn't exist because it should in all cases
                if (j == null)
                    throw new Exception($"{t.Name} is an IConCommand and has no ConCommandAttribute on it. Fix that!");

                Log.Debug($"Adding concommand {j.Name} ({t.Name})");

                // Make the actual instance
                var instance = (IConCommand)Activator.CreateInstance(t);

                var ccc = new ConCommandContainer();
                ccc.Command = instance;
                ccc.Name = j.Name;
                ccc.Arguments = atg;

                // Register it
                RegisterCommand(ccc);
            }
        }
    }
}