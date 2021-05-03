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
    public enum FPSCounterMode
    {
        Disabled,
        Standard,
        Advanced
    }

    public class PersistentUI
    {
        [ConVar("r_showfps", "Enables an FPS counter.")]
        public static FPSCounterMode ShowFPSCounter = FPSCounterMode.Disabled;

        [ConVar("r_showfps_padding", "Padding for the FPS counter.")]
        public static float FPSCounterPad = 0.0f;

        [ConVar("r_showfps_opacity", "How opaque the FPS counter is.")]
        public static float FPSCounterOpacity = 0.90f;

        public static float[] FrameTimings = new float[] { 0 };
        public static float[] FrameTimes = new float[] { 0 };

        public static float MaxFPS = 0.0f;
        public static float MaxFrameTime = 0.0f;

        public static float MinFPS = 0.0f;
        public static float MinFrameTime = 0.0f;

        [ConVar("r_showfps_frametime_count")]
        public static int FrameTimeCount = 1000;

        public static void Draw()
        {
            if (ShowFPSCounter != FPSCounterMode.Disabled)
            {

                var io = ImGui.GetIO();
                var vp = ImGui.GetMainViewport();

                var workPos = vp.WorkPos;
                var workSize = vp.WorkSize;

                var pos = new Vector2(FPSCounterPad, FPSCounterPad);

                var pivot = new Vector2(0.0f, 0.0f);

                ImGui.SetNextWindowPos(pos, ImGuiCond.Always, pivot);

                var flags = NoDecoration | NoMove | NoDocking | AlwaysAutoResize | NoSavedSettings | NoFocusOnAppearing | NoNav | NoNavFocus;

                ImGui.SetNextWindowViewport(vp.ID);

                ImGui.SetNextWindowBgAlpha(FPSCounterOpacity);

                if (ImGui.Begin("Luminal: FPS Counter", flags))
                {
                    ImGui.Text($"FPS:  {Timing.FrameRate}");
                    ImGui.Text($"Frame {Timing.FrameNumber}");
                    ImGui.Text($"/sec: {Timing.frameCount}");

                    if (ShowFPSCounter == FPSCounterMode.Advanced)
                    {
                        var howmany = Math.Max(Math.Min(FrameTimings.Length, 10), 1);
                        var fromwhere = Math.Max(FrameTimings.Length - 10, 0);

                        ImGui.PlotLines("", ref FrameTimings[fromwhere],
                            howmany, 0, "Frame rate", MinFPS, MaxFPS, new(500, 80));

                        var hm2 = Math.Max(Math.Min(FrameTimes.Length, FrameTimeCount), 1);

                        ImGui.PlotLines("", ref FrameTimes[0],
                            hm2, 0, $"Frame time: {Timing.DeltaTime}", MinFrameTime, MaxFrameTime, new(500, 80));
                    }


                    ImGui.End();
                }

            }
        }

        internal static void _updateGraph()
        {
            FrameTimings = FrameTimings.Append(Timing.FrameRate).ToArray();
            MaxFPS = FrameTimings.TakeLast(15).Max();
            MinFPS = FrameTimings.TakeLast(15).Min();
        }

        internal static void _afterFrame()
        {
            FrameTimes = FrameTimes.Append(Timing.DeltaTime).TakeLast(FrameTimeCount).ToArray();
            MaxFrameTime = FrameTimes.Max();
            MinFrameTime = FrameTimes.Min();
        }
    }
}
