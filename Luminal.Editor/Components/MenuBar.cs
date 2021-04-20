using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Luminal.Core;

namespace Luminal.Editor.Components
{
    public class MenuBar : Component3D
    {
        public override void OnGUI()
        {
            ImGui.BeginMainMenuBar();

            ImGuiUtil.RightAlignText($"Luminal v{EngineVersion.Current} - {Timing.FrameRate} fps");

            ImGui.EndMainMenuBar();
        }
    }
}
