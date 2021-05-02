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

        internal static (List<ConsoleChunk> raw, string overflow, bool error)
            SplitArgs(string command, int overflowAt = 99999, bool handleAsMomentary = false,
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
            var err = false;

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
                argumentCount++;
                return CheckOverflow();
            }

            bool CheckOverflow()
            {
                if (argumentCount > overflowAt)
                {
                    // Overflow here.
                    var range = (Math.Min(charsProcessed, command.Length));
                    var s = command[range..];
                    overflow = s;
                    overflowed = true;
                    inQuotedArgument = false;
                    return true;
                }
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
                if (!inQuotedArgument && (character == '-' || character == '+') && i == 0)
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
                    if (CheckOverflow()) break;
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
                err = true;

            var op = (args, overflow, err);
            return op;
        }
    }

    public static class ConsoleManager
    {
        public static Dictionary<string, ConCommandContainer> Commands = new();
        public static Dictionary<string, ConVarAttribute> ConVars = new();
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

                if ((ina.Count < i) && !wanted.Optional)
                    throw new ArgumentNullException();

                if (wanted.Overflow)
                {
                    var j = new ReceiveArgument(wanted.Type, wanted.Name);
                    var (_, overflow, _) = ConsoleSplitter.SplitArgs(raw, i);
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
            } catch (ArgumentNullException)
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

                    if (initial != null && initial.IsMomentary && isKeyEvent && !momentaryOnly)
                        return;

                    var prefix = "";
                    if (initial != null && initial.IsMomentary)
                    {
                        prefix = initial.MomentaryState ? "+" : "-";
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

                if (cv.Target == ConVarTarget.FIELD)
                {
                    if (!cv.Flags.Has(ConVarFlags.READONLY) && initial.IsMomentary)
                    {
                        if (cv.ValueType != ConVarType.BOOL)
                            return;
                        cv.FieldInfo.SetValue(null, initial.MomentaryState);
                        return;
                    }

                    var type = cv.ValueType;
                    var field = cv.FieldInfo;

                    if (inp.Count == 0 || field.IsInitOnly || cv.Flags.Has(ConVarFlags.READONLY))
                    {
                        // There is no actual argument, print the value.
                        var val = field.GetValue(null);
                        var h = val.ToString();
                        DebugConsole.LogRaw($@"""{commandName}"" = ""{h}""");
                        // also display flags + description
                        if (cv.Flags != 0) // has flags!
                        {
                            DebugConsole.LogRaw($" {cv.Flags.GetFlagString()}");
                        }
                        DebugConsole.LogRaw($" - {cv.Description ?? "No description specified."}");

                        return;
                    }
                    // Okay, there's an input
                    var input = inp[0];

                    var value = ConVarAttribute.Parse(input, type);

                    field.SetValue(null, value);
                } else if (cv.Target == ConVarTarget.PROPERTY)
                {
                    if (!cv.Flags.Has(ConVarFlags.READONLY) && initial.IsMomentary && cv.PropertyInfo.CanWrite)
                    {
                        // Write to it
                        if (cv.ValueType != ConVarType.BOOL)
                            return;
                        var value = ConVarAttribute.Parse(inp[0], cv.ValueType);
                        cv.PropertyInfo.SetValue(null, value);
                    }

                    var isRO = !cv.PropertyInfo.CanWrite || cv.Flags.Has(ConVarFlags.READONLY);

                    if (inp.Count == 0 || isRO)
                    {
                        if (cv.PropertyInfo.CanRead)
                        {
                            var val = cv.PropertyInfo.GetValue(null);
                            var h = val.ToString();
                            DebugConsole.LogRaw($@"""{commandName}"" = ""{h}""");
                            // also display flags + description

                            var fsa = new[] { "readonly" };

                            if (cv.Flags > 0 || isRO)
                                DebugConsole.LogRaw($" {cv.Flags.GetFlagString(isRO ? fsa : null)}");

                            DebugConsole.LogRaw($" - {cv.Description ?? "No description specified."}");
                        } else
                        {
                            DebugConsole.LogRaw($@"""{commandName}"" = (unavailable)");

                            var p = new[] { "writeonly" };
                            if (!cv.PropertyInfo.CanWrite)
                                p = new[] { "inaccessible" };

                            var fstr = cv.Flags.GetFlagString(p);

                            DebugConsole.LogRaw($" {fstr}");
                            DebugConsole.LogRaw($" - {cv.Description ?? "No description specified."}");
                        }

                        return;
                    }

                    var input = inp[0];
                    var v = ConVarAttribute.Parse(input, cv.ValueType);
                    cv.PropertyInfo.SetValue(null, v);
                }
                
                return;
            }

            // Avoid running any commands in a momentary keystate.
            if (isKeyEvent && momentaryOnly) return;

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

                    a.FieldInfo = f;
                    a.FieldType = f.FieldType;

                    a.SetType(f.FieldType);

                    a.Target = ConVarTarget.FIELD;

                    ConVars.Add(a.Name, a);
                }

                foreach (var f in t.GetProperties())
                {
                    var a = f.GetCustomAttribute<ConVarAttribute>();
                    if (a == null)
                        continue;

                    a.PropertyInfo = f;
                    a.PropertyType = f.PropertyType;

                    a.SetType(f.PropertyType);

                    a.Target = ConVarTarget.PROPERTY;

                    ConVars.Add(a.Name, a);
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