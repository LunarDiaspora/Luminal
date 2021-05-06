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
using Luminal.Core;
using System.Runtime.InteropServices;

namespace Luminal.Console
{
    public class DebugConsole
    {
        public class ConsoleLine
        {
            public LogLevel level;
            public string data;
            public bool raw = false;
        }

        public static List<ConsoleLine> ConsoleOutput = new();

        public static List<string> History = new();

        static int histItem = -1;

        static Dictionary<LogLevel, string> levels = new()
        {
            { LogLevel.Debug, "DEBUG" },
            { LogLevel.Info, " INFO" },
            { LogLevel.Warning, " WARN" },
            { LogLevel.Error, "ERROR" },
            { LogLevel.Fatal, "FATAL" },
            { LogLevel.Unexpected, "WHAT?" }
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
            {LogLevel.Debug, C_DEBUG},
            {LogLevel.Info, C_INFO},
            {LogLevel.Warning, C_WARN},
            {LogLevel.Error, C_ERROR},
            {LogLevel.Fatal, C_FATAL},
            {LogLevel.Unexpected, C_WTF}
        };

        private static bool isScrollingDown = false;

        public static void ScrollDown()
        {
            isScrollingDown = true;
        }

        public static void OnGUI()
        {
            ImGui.Begin("Console", ref Engine.ConsoleOpen, ImGuiWindowFlags.NoCollapse);

            var reservedHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing() + 1;

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, 0);

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

            // I don't know why this works, I don't *want* to know why this works,
            // but adding an invisible separator here fixes the problem where
            // console output is cut off on the bottom by one pixel.

            // God dammit ImGui.
            
            var sc = ImGui.GetColorU32(ImGuiCol.WindowBg);
            ImGui.PushStyleColor(ImGuiCol.Separator, sc);
            ImGui.Separator();
            ImGui.PopStyleColor();

            ImGui.EndChild();

            ImGui.Separator();

            string commandData = "";
            var reclaimFocus = false;

            ImGui.SetNextItemWidth(-1);

            unsafe
            {
                if (ImGui.InputTextWithHint("", "Enter a command...", ref commandData, 65536,
                    ImGuiInputTextFlags.EnterReturnsTrue |
                    ImGuiInputTextFlags.CallbackCompletion |
                    ImGuiInputTextFlags.CallbackHistory,
                    CommandCallback))
                {
                    if (commandData.Length != 0)
                    {
                        if (History.Contains(commandData))
                        {
                            History.Remove(commandData);
                        }

                        History.Insert(0, commandData);
                    }

                    histItem = -1;

                    LogRaw("] " + commandData);

                    HandleCommand(commandData);

                    ScrollDown();

                    reclaimFocus = true;
                }
            }

            ImGui.SetItemDefaultFocus();
            if (reclaimFocus)
                ImGui.SetKeyboardFocusHere(-1);

            ImGui.End();
        }

        private static unsafe int CommandCallback(ImGuiInputTextCallbackData* data)
        {
            if (data->EventFlag == ImGuiInputTextFlags.CallbackHistory)
            {
                if (data->EventKey == ImGuiKey.UpArrow)
                {
                    // Previous history item
                    histItem = Math.Min(histItem + 1, History.Count-1);

                    if (histItem >= 0)
                    {
                        ImGuiNative.ImGuiInputTextCallbackData_DeleteChars(data, 0, data->BufTextLen);
                        var h = History[histItem];
                        var p = Marshal.StringToHGlobalUni(h);
                        ImGuiNative.ImGuiInputTextCallbackData_InsertChars(data, 0, (byte*)p.ToPointer(), null);
                        ImGuiNative.ImGuiInputTextCallbackData_SelectAll(data);
                    }
                }

                if (data->EventKey == ImGuiKey.DownArrow)
                {
                    // Next history item
                    histItem = Math.Max(histItem - 1, 0);

                    if (histItem >= 0)
                    {
                        ImGuiNative.ImGuiInputTextCallbackData_DeleteChars(data, 0, data->BufTextLen);
                        var h = History[histItem];
                        var p = Marshal.StringToHGlobalUni(h);
                        ImGuiNative.ImGuiInputTextCallbackData_InsertChars(data, 0, (byte*)p.ToPointer(), null);
                        ImGuiNative.ImGuiInputTextCallbackData_SelectAll(data);
                    }
                }
            }

            return 0;
        }

        public static void HandleCommand(string command, bool momentaryOnly = false, bool keyState = false, bool momentary = false, bool isKeyEvent = false)
        {
            try
            {
                var (raw, overflow, _) = ConsoleSplitter.SplitArgs(command, 99999, momentary, keyState);
                if (raw.Count == 0) return;

                var cmdName = raw[0];
                ConsoleManager.RunConsole(cmdName, raw.Skip(1).ToList(), command, cmdName, momentaryOnly, isKeyEvent);
            } catch(ArgumentException e)
            {
                LogRaw(e.Message);
            }
        }

        public static void HandleCommandSilently(string command, bool momentaryOnly = false, bool keyState = false, bool momentary = false, bool isKeyEvent = false)
        {
            try
            {
                var (raw, overflow, _) = ConsoleSplitter.SplitArgs(command, 99999, momentary, keyState);
                if (raw.Count == 0) return;

                var cmdName = raw[0];
                ConsoleManager.RunConsole(cmdName, raw.Skip(1).ToList(), command, cmdName, momentaryOnly, isKeyEvent);
            }
            catch
            {
            }
        }

        public static void LogRaw(string o)
        {
            foreach (var s in o.Split("\n"))
            {
                var v = new ConsoleLine()
                {
                    data = s,
                    level = LogLevel.Debug,
                    raw = true
                };
                ConsoleOutput.Add(v);
            }

            ScrollDown();
        }
    }
}
