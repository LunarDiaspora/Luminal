using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Luminal.Logging;

namespace Luminal.DebugUI
{
    public class ConsoleWindow : DebugWindow
    {
        static Dictionary<LogLevel, string> levels = new()
        {
            { LogLevel.Debug, "DEBUG" },
            { LogLevel.Info, " INFO" },
            { LogLevel.Warning, " WARN" },
            { LogLevel.Error, "ERROR" },
            { LogLevel.Critical, " CRIT" },
            { LogLevel.Unexpected, "WHAT?" }
        };

        public static readonly Vector4 C_DG = new(105 / 255f, 105 / 255f, 105 / 255f, 1.0f);

        public static readonly Vector4 C_DEBUG = C_DG;
        public static readonly Vector4 C_INFO = new(0f, 214 / 255f, 200 / 255f, 1f);
        public static readonly Vector4 C_WARN = new(219 / 255f, 179 / 255f, 0, 1f);
        public static readonly Vector4 C_ERROR = new(214 / 255f, 71 / 255f, 71 / 255f, 1f);
        public static readonly Vector4 C_CRIT = new(237 / 255f, 0, 0, 1);
        public static readonly Vector4 C_WTF = new(237 / 255f, 12 / 255f, 207 / 255f, 1);

        public static readonly Vector4 C_RAW = new(200 / 255f, 200 / 255f, 200 / 255f, 1);

        static Dictionary<LogLevel, Vector4> colours = new()
        {
            { LogLevel.Debug, C_DEBUG },
            { LogLevel.Info, C_INFO },
            { LogLevel.Warning, C_WARN },
            { LogLevel.Error, C_ERROR },
            { LogLevel.Critical, C_CRIT },
            { LogLevel.Unexpected, C_WTF }
        };

        string inputBuffer = "";

        public override void Render()
        {
            if (ImGui.Begin("Console##Luminal2_Engine_Internal_Debug_Console", ref Open))
            {
                var reservedHeight = ImGui.GetStyle().ItemSpacing.Y +
                    ImGui.GetFrameHeightWithSpacing() +
                    ImGui.GetStyle().FramePadding.Y
                    + 2;

                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, 0);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1));

                if (ImGui.BeginChild("Luminal2_Engine_Console_Scrolling_Child",
                    new(0, -reservedHeight),
                    false, ImGuiWindowFlags.HorizontalScrollbar))
                {
                    foreach (var ev in DebugConsoleLog.Entries)
                    {
                        ImGui.TextColored(C_DG, "[");
                        ImGui.SameLine();
                        var col = colours[ev.Level];
                        ImGui.TextColored(col, levels[ev.Level]);
                        ImGui.SameLine();
                        ImGui.TextColored(C_DG, "] ");
                        ImGui.SameLine();
                        ImGui.TextUnformatted(ev.Text);
                    }
                }
                ImGui.PopStyleVar();

                // Invisible separator trick
                var sc = ImGui.GetColorU32(ImGuiCol.WindowBg);
                ImGui.PushStyleColor(ImGuiCol.Separator, sc);
                ImGui.Separator();
                ImGui.PopStyleColor();

                ImGui.EndChild();

                ImGui.Separator();

                ImGui.Dummy(new Vector2(0, 1));

                ImGui.SetNextItemWidth(-1);
                if (ImGui.InputTextWithHint("", "Command", ref inputBuffer, 65535,
                    ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    inputBuffer = "";
                }

                ImGui.End();
            }
        }
    }
}
