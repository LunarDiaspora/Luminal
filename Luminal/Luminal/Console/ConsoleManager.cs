using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using Luminal.Entities.Components;

namespace Luminal.Console
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
            {
                // Maybe this is a convar instead.
                if (!ConVars.ContainsKey(commandName))
                {
                    // Nah.
                    throw new ArgumentException($"Unknown command \"{commandName}\"");
                }

                // Oh, it is.
                var cv = ConVars[commandName];
                var attr = ConVarAttrs[commandName];
                var type = cv.t;
                var field = cv.fi;

                if (inp.Count == 0 || field.IsInitOnly)
                {
                    // There is no actual argument, print the value.
                    var val = field.GetValue(null);
                    var h = val.ToString();
                    DebugConsole.LogRaw($@"""{commandName}"" = ""{h}""");
                    // also display flags + description
                    if (attr.Flags != 0) // has flags!
                    {
                        DebugConsole.LogRaw($" {attr.Flags.GetFlagString()}");
                    }
                    DebugConsole.LogRaw($" - {attr.Description ?? "No description specified."}");

                    return;
                }

                if (!attr.Flags.Has(ConVarFlags.READONLY))
                {
                    // Okay, there's an input
                    var input = inp[0];

                    var value = ConVarAttribute.Parse(input, type);

                    field.SetValue(null, value);
                } else
                {
                    DebugConsole.LogRaw($"Convar {commandName} is read only.");
                }

                

                return;
            }
            var command = Commands[commandName];
            var a = BuildArgs(inp, command);
            command.Command.Run(a);
        }
        
        public static void FindConCommands(Assembly asm)
        {
            foreach (var t in asm.DefinedTypes)
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
                ccc.Description = j.Description;

                // Register it
                RegisterCommand(ccc);
            }
        }

        public static Dictionary<string, (FieldInfo fi, ConVarType t)> ConVars = new();
        public static Dictionary<string, ConVarAttribute> ConVarAttrs = new();

        public static void FindConVars(Assembly asm)
        {
            foreach (var t in asm.DefinedTypes)
            {
                foreach (var f in t.GetFields())
                {
                    var a = f.GetCustomAttribute<ConVarAttribute>();
                    if (a == null)
                        continue;

                    (FieldInfo fi, ConVarType t) n = new();

                    n.fi = f;
                    n.t = ConVarAttribute.ToConVarType(f.FieldType);

                    ConVars.Add(a.Name, n);
                    ConVarAttrs.Add(a.Name, a);
                }
            }
        }

        public static void FindAllEverywhere()
        {
            var ad = AppDomain.CurrentDomain;
            var asms = ad.GetAssemblies();

            foreach (var a in asms)
            {
                FindConCommands(a);
                FindConVars(a);
            }
        }
    }
}