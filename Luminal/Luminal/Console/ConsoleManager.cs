using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using Luminal.Entities.Components;
using static SDL2.SDL;
using Luminal.Core;

namespace Luminal.Console
{
    public class ConsoleChunk
    {
        public string Text;
        public bool IsMomentary = false;
        public bool MomentaryState = false;

        public static implicit operator string(ConsoleChunk h)
        {
            return h.Text;
        }
    }

    internal static class StringExtensions
    {
        public static ConsoleChunk ToChunk(this string h)
        {
            return new()
            {
                Text = h
            };
        }
    }

    internal static class ConsoleSplitter
    {

        internal static (List<ConsoleChunk> raw, string overflow) SplitArgs(string command, int overflowAt = 99999, bool handleAsMomentary = false,
                            bool keyState = true)
        {
            var args = new List<ConsoleChunk>();
            var currentArgument = "";
            var inQuotedArgument = false;
            var argumentCount = 0;
            var charsProcessed = 0;
            var overflow = "";
            var overflowed = false;
            var momentary = false;
            var state = false;

            bool Push()
            {
                var c = currentArgument.ToChunk();
                c.IsMomentary = momentary;
                c.MomentaryState = (handleAsMomentary ? keyState : state);
                momentary = false;
                if (currentArgument != "")
                    args.Add(c);
                currentArgument = "";
                charsProcessed++;
                if (argumentCount >= overflowAt)
                {
                    // Overflow here.
                    var s = command[charsProcessed..];
                    overflow = s;
                    overflowed = true;
                    return true;
                }
                argumentCount++;
                return false;
            }

            for (int i = 0; i < command.Length; i++)
            {
                var character = command[i];

                if (!inQuotedArgument && character == ' ')
                {
                    // This is a space break.
                    if (Push()) break;
                    continue;
                }

                // Momentary variables: make sure they only count at the front of the string
                if (!inQuotedArgument && (character == '-' || character == '+') && argumentCount == 0)
                {
                    // Momentary variable
                    momentary = true;
                    state = (character == '+');
                    continue; // Drop the character
                }

                if (character == '"')
                {
                    // This is a quote.
                    if (inQuotedArgument)
                    {
                        // This is a quote break.
                        if (Push()) break;
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
                    Push();
                    break;
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
        public static Dictionary<string, string> Aliases = new();

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

        public static Arguments BuildArgs(List<ConsoleChunk> ina, List<Argument> desired, string raw)
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
                n.Parse(p.Text);
                arg.values.Add(wanted.Name, n);
            }

            return arg;
        }

        public static Arguments BuildArgs(List<ConsoleChunk> ina, ConCommandContainer ccc, string raw)
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

        public static void RunConsole(string commandName, List<ConsoleChunk> inp, string raw, ConsoleChunk initial = null,
                                      bool momentaryOnly = false, bool isKeyEvent = false)
        {
            if (!Commands.ContainsKey(commandName))
            {
                // Maybe this is a convar instead.
                if (!ConVars.ContainsKey(commandName))
                {
                    // Nah.
                    // ...what if it's aliased?

                    var prefix = "";
                    if (initial != null && initial.IsMomentary)
                    {
                        prefix = initial.MomentaryState ? "+" : "-";

                        if (isKeyEvent && !momentaryOnly) return;
                    }

                    var ok = Aliases.TryGetValue(prefix + commandName, out string alias);
                    if (ok)
                    {
                        // It is, let's run it.
                        DebugConsole.HandleCommandSilently(alias);
                        return;
                    } else
                    {
                        // No dice. Fail.
                        throw new ArgumentException($"Unknown command \"{commandName}\"");
                    }
                }

                // Oh, it is.
                var cv = ConVars[commandName];
                var attr = ConVarAttrs[commandName];

                if (attr.Momentary && !attr.Flags.Has(ConVarFlags.READONLY) && initial.IsMomentary)
                {
                    if (cv.t != ConVarType.BOOL)
                        throw new Exception("Momentary variable is not a boolean!");
                    cv.fi.SetValue(null, initial.MomentaryState);
                    return;
                }

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

            // Avoid running any commands in a momentary keystate.
            if (momentaryOnly) return;

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
                DebugConsole.HandleCommand(v, false, false, false, true);
            }
        }

        public static void RunMomentaryBind(SDL_Scancode code, bool KeyState)
        {
            if (Binds.ContainsKey(code))
            {
                var v = Binds[code];
                DebugConsole.HandleCommand(v, true, KeyState, true, true);
            }
        }
    }
}