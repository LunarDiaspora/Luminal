using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace Luminal.Editor.Components
{
    class WelcomeMenuBar : Component3D
    {
        public override void OnGUI()
        {
            ImGui.BeginMainMenuBar();

            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open Luminal project", "ctrl+o"))
                {
                    Editor.DoLoadFlow();
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }
}
