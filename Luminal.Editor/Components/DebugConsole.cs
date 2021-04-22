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

            ImGui.BeginChild("ConsoleScrollRegion", new(0, -1), false, ImGuiWindowFlags.HorizontalScrollbar);

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1));
            foreach (var item in Editor.ConsoleOutput)
            {
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

            ImGui.End();
        }
    }
}
