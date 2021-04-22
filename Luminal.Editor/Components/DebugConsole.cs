using ImGuiNET;
using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Luminal.Logging;

namespace Luminal.Editor.Components
{
    class DebugConsole : Component3D
    {
        static Dictionary<LogLevel, string> levels = new()
        {
            { LogLevel.DEBUG, "DEBUG" },
            { LogLevel.INFO, " INFO" },
            { LogLevel.WARNING, " WARN" },
            { LogLevel.ERROR, "ERROR" },
            { LogLevel.FATAL, "FATAL" },
            { LogLevel.UNEXPECTED, "WHAT?" }
        };

        public static readonly Vector4 C_DG = new(105/255f, 105/255f, 105/255f, 1.0f);

        public static readonly Vector4 C_DEBUG = C_DG;
        public static readonly Vector4 C_INFO = new(0f, 214/255f, 200/255f, 1f);
        public static readonly Vector4 C_WARN = new(219/255f, 179/255f, 0, 1f);
        public static readonly Vector4 C_ERROR = new(214/255f, 71/255f, 71/255f, 1f);
        public static readonly Vector4 C_FATAL = new(237/255f, 0, 0, 1);
        public static readonly Vector4 C_WTF = new(237/255f, 12/255f, 207/255f, 1);

        public static readonly Vector4 C_RAW = new(200/255f, 200/255f, 200/255f, 1);

        static Dictionary<LogLevel, Vector4> colours = new()
        {
            {LogLevel.DEBUG, C_DEBUG},
            {LogLevel.INFO, C_INFO},
            {LogLevel.WARNING, C_WARN},
            {LogLevel.ERROR, C_ERROR},
            {LogLevel.FATAL, C_FATAL},
            {LogLevel.UNEXPECTED, C_WTF}
        };

        private static bool isScrollingDown = false;

        public static void ScrollDown()
        {
            isScrollingDown = true;
        }

        public override void OnGUI()
        {
            ImGui.Begin("Console");

            var reservedHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();
            ImGui.BeginChild("ConsoleScrollRegion", new(0, -reservedHeight), false, ImGuiWindowFlags.HorizontalScrollbar);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1));
            foreach (var item in Editor.ConsoleOutput)
            {
                if (item.raw)
                {
                    if (item.data.StartsWith("] "))
                    {
                        ImGui.TextColored(C_RAW, item.data);
                    } else
                    {
                        ImGui.TextUnformatted(item.data);
                    }
                    continue;
                }
                var c = colours[item.level];
                ImGui.TextColored(C_DG, "[");
                ImGui.SameLine();
                ImGui.TextColored(c, levels[item.level]);
                ImGui.SameLine();
                ImGui.TextColored(C_DG, "]");
                ImGui.SameLine();
                ImGui.TextUnformatted($" {item.data}");
            }

            if (isScrollingDown || ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
            {
                ImGui.SetScrollHereY();
            }
            isScrollingDown = false;

            ImGui.PopStyleVar();

            ImGui.EndChild();

            ImGui.Separator();

            string commandData = "";
            var reclaimFocus = false;
            if (ImGui.InputText("", ref commandData, 65536, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                Editor.ConsoleOutput.Add(new ConsoleLine
                {
                    data = "] " + commandData,
                    level = LogLevel.DEBUG,
                    raw = true
                });

                HandleCommand(commandData);

                ScrollDown();

                reclaimFocus = true;
            }

            ImGui.SetItemDefaultFocus();
            if (reclaimFocus)
                ImGui.SetKeyboardFocusHere(-1);

            ImGui.End();
        }

        private void HandleCommand(string command)
        {
            var args = command.Split(' ');
            var cmd = args[0];

            var characters = cmd.Length;
            var currentQuotedArg = "";
            var startedCurrentQuotedArg = 0;
            var currentArg = 0;

            var rebuiltArgs = new List<string>();
            var rawArgs = args.Skip(1).ToArray();

            foreach (var arg in rawArgs)
            {
                currentArg++;
                characters += arg.Length;

                if (arg.StartsWith('"'))
                {
                    if (currentQuotedArg != "")
                    {
                        CommandLineErrorShowerThingy(command, startedCurrentQuotedArg, 1,
                            "You cannot start a quoted argument inside of a quoted argument.");
                        return;
                    }

                    currentQuotedArg = arg.Substring(1);
                    startedCurrentQuotedArg = characters;

                    if (arg.EndsWith('"'))
                    {
                        currentQuotedArg = arg.Substring(1, arg.Length - 1);

                        rebuiltArgs.Add(currentQuotedArg);
                        currentQuotedArg = "";
                        startedCurrentQuotedArg = 0;
                    }

                    continue;
                }

                if (arg.EndsWith('"'))
                {
                    if (currentQuotedArg != "")
                    {
                        currentQuotedArg += " " + arg.Substring(0, arg.Length - 1);

                        rebuiltArgs.Add(currentQuotedArg);
                        currentQuotedArg = "";
                        startedCurrentQuotedArg = 0;

                        continue;
                    }
                }

                if (currentQuotedArg != "")
                {
                    if (currentArg == rawArgs.Length)
                    {
                        CommandLineErrorShowerThingy(command, startedCurrentQuotedArg, currentQuotedArg.Length + arg.Length + 2,
                            "Unbalanced quotes");

                        return;
                    }

                    currentQuotedArg += " " + arg;
                    continue;
                }

                rebuiltArgs.Add(arg);
            }

            Editor.ConsoleOutput.Add(new ConsoleLine
            {
                level = LogLevel.DEBUG,
                data = "No such command: " + cmd,
                raw = true
            });
        }

        private void CommandLineErrorShowerThingy(string commandLine, int position, int length, string error)
        {
            Editor.ConsoleOutput.Add(new ConsoleLine
            {
                level = LogLevel.ERROR,
                data = "Your command line has an error: " + error
            });

            Editor.ConsoleOutput.Add(new ConsoleLine
            {
                level = LogLevel.ERROR,
                data = commandLine
            });

            Editor.ConsoleOutput.Add(new ConsoleLine
            {
                level = LogLevel.ERROR,
                data = new string(' ', position - 1) + new string('^', length)
            }); ;
        }
    }
}
