using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using Luminal.Entities.Components;
using static SDL2.SDL;
using Luminal.Core;

namespace Luminal.Console
{
    internal static class ConsoleSplitter
    {
        internal static (List<string> raw, string overflow) SplitArgs(string command, int overflowAt = 99999)
        {
            var args = new List<string>();
            var currentArgument = "";
            var inQuotedArgument = false;
            var argumentCount = 0;
            var charsProcessed = 0;
            var overflow = "";
            var overflowed = false;

            for (int i = 0; i < command.Length; i++)
            {
                var character = command[i];

                if (!inQuotedArgument && character == ' ')
                {
                    // This is a space break.
                    if (currentArgument != "")
                        args.Add(currentArgument);
                    currentArgument = "";
                    charsProcessed++;
                    if (argumentCount >= overflowAt)
                    {
                        // Overflow here.
                        var s = command.Substring(charsProcessed);
                        overflow = s;
                        overflowed = true;
                        break;
                    }
                    argumentCount++;
                    continue;
                }

                if (character == '"')
                {
                    // This is a quote.
                    if (inQuotedArgument)
                    {
                        // This is a quote break.
                        args.Add(currentArgument);
                        currentArgument = "";
                        inQuotedArgument = false;
                        charsProcessed++;
                        if (argumentCount >= overflowAt)
                        {
                            // Overflow here.
                            var s = command.Substring(charsProcessed);
                            overflow = s;
                            overflowed = true;
                            break;
                        }
                        argumentCount++;
                        continue;
                    }
                    // This is not a quote break; begin a quoted argument.
                    inQuotedArgument = true;
                    continue;
                }

                currentArgument += character;
                charsProcessed++;

                if (command.Length - 1 == i)
                {
                    // At the end of the string.
                    args.Add(currentArgument);
                }
            }

            if (inQuotedArgument && !overflowed)
                throw new ArgumentException("Syntax error: Unbalanced quotes.");

            var op = (args, overflow);
            return op;
        }
    }

    public static class ConsoleManager
    {
        public static Dictionary<string, ConCommandContainer> Commands = new();
        public static Dictionary<string, (FieldInfo fi, ConVarType t)> ConVars = new();
        public static Dictionary<string, ConVarAttribute> ConVarAttrs = new();
        public static Dictionary<SDL_Scancode, string> Binds = new()
        {
            { SDL_Scancode.SDL_SCANCODE_F10, "toggleconsole" } // Default console bind.
        };

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

        public static Arguments BuildArgs(List<string> ina, List<Argument> desired, string raw)
        {
            var arg = new Arguments();

            for (int i=0; i<desired.Count; i++)
            {
                var wanted = desired[i];
                if ((ina.Count <= i) && !wanted.Optional)
                    throw new ArgumentOutOfRangeException();

                if (wanted.Overflow)
                {
                    var j = new ReceiveArgument(wanted.Type, wanted.Name);
                    var (_, overflow) = ConsoleSplitter.SplitArgs(raw, i);
                    j.Parse(overflow);
                    arg.values.Add(wanted.Name, j);
                    continue;
                }

                if (ina.Count <= i)
                    break; // No more of them

                var p = ina[i];
                var n = new ReceiveArgument(wanted.Type, wanted.Name);
                n.Parse(p);
                arg.values.Add(wanted.Name, n);
            }

            return arg;
        }

        public static Arguments BuildArgs(List<string> ina, ConCommandContainer ccc, string raw)
        {
            var a = ccc.Arguments ?? new();
            try
            {
                return BuildArgs(ina, a, raw);
            } catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException($"Usage: {ccc.Name} {GetUsage(a)}");
            }
        }

        public static void RunConsole(string commandName, List<string> inp, string raw)
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
            var a = BuildArgs(inp, command, raw);
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

        public static void BindKeyString(string key, string command)
        {
            try
            {
                var c = Engine.StringToScancode(key);
                Binds[c] = command;
            } catch(ArgumentException)
            {
                return;
            }
        }

        public static void BindKeyCode(SDL_Scancode sc, string cmd)
        {
            Binds[sc] = cmd;
        }

        public static void RunBind(SDL_Scancode code)
        {
            if (Binds.ContainsKey(code))
            {
                var v = Binds[code];
                DebugConsole.HandleCommand(v);
            }
        }
    }
}