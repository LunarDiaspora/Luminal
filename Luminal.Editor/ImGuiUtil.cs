using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace Luminal.Editor
{
    public static class ImGuiUtil
    {
        public static void RightAlignText(string text)
        {
            RightAlignSetup(text);
            ImGui.Text(text);
        }

        public static void RightAlignSetup(string text)
        {
            var c = ImGui.GetContentRegionMax().X;
            var w = ImGui.CalcTextSize(text).X;
            ImGui.SetCursorPosX(c - w);
        }
    }
}
