using ImGuiNET;
using Luminal.Audio;
using Luminal.Console;
using Luminal.Entities.Components;
using Luminal.OpenGL;
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

        static int _AASamples = 2;

        [ConVar("r_aasamples", "Sets the amount of antialiasing samples. Cannot go below 1 or above 16.")]
        public static int AASamples // Lord forgive me for what I am about to do to this perfectly good language
        {
            get => _AASamples;

            set => _AASamples = Math.Max(Math.Min(value, 16), 1);
        }

        static int AAIndex
        {
            get
            {
                if (!GLRenderTexture.UseAntialiasing) return 0;

                return AASamples switch
                {
                    1 => 0,
                    2 => 1,
                    4 => 2,
                    8 => 3,
                    16 => 4,
                    _ => 0
                };
            }

            set
            {
                AASamples = value switch
                {
                    0 => 1,
                    1 => 2,
                    2 => 4,
                    3 => 8,
                    4 => 16,
                    _ => 1
                };

                GLRenderTexture.UseAntialiasing = AASamples != 1;
            }
        }

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

                        var ai = AAIndex;
                        if (ImGui.Combo("Antialiasing", ref ai, items, items.Length))
                        {
                            AAIndex = ai;
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
