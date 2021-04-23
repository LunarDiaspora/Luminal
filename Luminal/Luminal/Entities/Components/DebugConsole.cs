using ImGuiNET;
using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Luminal.Logging;
using Luminal.Console;

namespace Luminal.Entities.Components
{
    public class DebugConsole : Component3D
    {
        public class ConsoleLine
        {
            public LogLevel level;
            public string data;
            public bool raw = false;
        }

        public static List<ConsoleLine> ConsoleOutput = new();

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
            foreach (var item in ConsoleOutput)
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

            ImGui.SetNextItemWidth(-1);
            if (ImGui.InputText("", ref commandData, 65536, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                LogRaw("] " + commandData);

                HandleCommand(commandData);

                ScrollDown();

                reclaimFocus = true;
            }

            ImGui.SetItemDefaultFocus();
            if (reclaimFocus)
                ImGui.SetKeyboardFocusHere(-1);

            ImGui.End();
        }

        private List<string> SplitArgs(string command)
        {
            var args = new List<string>();
            var currentArgument = "";
            var inQuotedArgument = false;

            for (int i = 0; i < command.Length; i++)
            {
                var character = command[i];

                if (!inQuotedArgument && character == ' ')
                {
                    // This is a space break.
                    if (currentArgument != "")
                        args.Add(currentArgument);
                    currentArgument = "";
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
                        continue;
                    }
                    // This is not a quote break; begin a quoted argument.
                    inQuotedArgument = true;
                    continue;
                }

                currentArgument += character;

                if (command.Length-1 == i)
                {
                    // At the end of the string.
                    args.Add(currentArgument);
                }
            }

            if (inQuotedArgument)
                throw new ArgumentException("Syntax error: Unbalanced quotes.");

            return args;
        }

        private void HandleCommand(string command)
        {
            try
            {
                var a = SplitArgs(command);
                if (a.Count == 0) return;

                var cmdName = a[0];
                ConsoleManager.RunConsole(cmdName, a.Skip(1).ToList());
            } catch(ArgumentException e)
            {
                LogRaw(e.Message);
            }
        }

        public static void LogRaw(string o)
        {
            foreach (var s in o.Split("\n"))
            {
                var v = new ConsoleLine()
                {
                    data = s,
                    level = LogLevel.DEBUG,
                    raw = true
                };
                ConsoleOutput.Add(v);
            }

            ScrollDown();
        }
    }
}
