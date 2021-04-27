using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Luminal.Console;
using static ImGuiNET.ImGuiWindowFlags;

namespace Luminal.Core
{
    public class PersistentUI
    {
        [ConVar("r_showfps", "Enables an FPS counter.")]
        public static bool ShowFPSCounter = false;

        [ConVar("r_showfps_padding", "Padding for the FPS counter.")]
        public static float FPSCounterPad = 10.0f;

        [ConVar("r_showfps_opacity", "How opaque the FPS counter is.")]
        public static float FPSCounterOpacity = 0.60f;

        public static void Draw()
        {
            if (ShowFPSCounter)
            {

                var io = ImGui.GetIO();
                var vp = ImGui.GetMainViewport();

                var workPos = vp.GetWorkPos();
                var workSize = vp.GetWorkSize();

                var pos = new Vector2(FPSCounterPad, FPSCounterPad);

                var pivot = new Vector2(0.0f, 0.0f);

                ImGui.SetNextWindowPos(pos, ImGuiCond.Always, pivot);

                var flags = NoDecoration | NoMove | NoDocking | AlwaysAutoResize | NoSavedSettings | NoFocusOnAppearing | NoNav | NoNavFocus;

                ImGui.SetNextWindowViewport(vp.ID);

                ImGui.SetNextWindowBgAlpha(FPSCounterOpacity);

                if (ImGui.Begin("Luminal: FPS Counter", ref ShowFPSCounter, flags))
                {
                    ImGui.Text($"FPS:  {Timing.FrameRate}");
                    ImGui.Text($"Frame {Timing.FrameNumber}");
                    ImGui.Text($"/sec: {Timing.frameCount}");

                    ImGui.End();
                }

            }
        }
    }
}
