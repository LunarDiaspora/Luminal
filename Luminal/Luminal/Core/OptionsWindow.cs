using ImGuiNET;
using Luminal.Audio;
using Luminal.Console;
using Luminal.Entities.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Core
{
    public class OptionsWindow
    {
        [ConVar("options", "Dictates whether the options window is open or not.")]
        public static bool Open = false;

        public static int AASamples = 2;

        [ConVar("r_aasamples", "Sets the amount of antialiasing samples. Cannot go below 1 or above 16.")]
        public static int AASamplesConVar // Lord forgive me for what I am about to do to this perfectly good language
        {
            get => AASamples;
            set => AASamples = Math.Max(Math.Min(value, 16), 1);
        }

        static int AAIndex = 0;

        public static void Draw()
        {
            if (Open)
            {
                ImGui.Begin("Options", ref Open);

                if (ImGui.BeginTabBar("Options Tab Bar"))
                {
                    if (ImGui.BeginTabItem("Graphics"))
                    {
                        ImGui.BeginGroup();
                        ImGui.Checkbox("Cubemaps", ref CubemapRenderer.RenderCubemaps);
                        ImGui.SameLine();
                        ImGui.Checkbox("Grids", ref GridRenderer.EnableGrids);
                        ImGui.EndGroup();

                        var items = new[]
                        {
                            "Disabled",
                            "2x",
                            "4x",
                            "8x",
                            "16x"
                        };

                        var results = new[]
                        {
                            1,
                            2,
                            4,
                            8,
                            16
                        };

                        if (ImGui.Combo("Antialiasing", ref AAIndex, items, items.Length))
                        {
                            var setTo = results[AAIndex];
                            AASamples = setTo;
                        }

                        if (ImGui.IsItemHovered())
                            ImGui.SetTooltip("Only applies to rendertextures!");

                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Audio"))
                    {
                        var av = AudioEngine.AudioVolume;
                        var avString = $"{(int)Math.Floor(av * 100)}%%";
                        ImGui.SliderFloat("Volume", ref av, 0.0f, 1.0f, avString);
                        AudioEngine.AudioVolume = av;

                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }

                ImGui.End();
            }
        }
    }
}
