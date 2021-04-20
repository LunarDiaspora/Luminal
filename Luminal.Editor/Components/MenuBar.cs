using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Luminal.Core;
using Luminal.Logging;

namespace Luminal.Editor.Components
{
    public class MenuBar : Component3D
    {
        public override void Create()
        {
            var _ = Parent.GetOrCreateComponent<InternalComponent>();
        }

        public override void OnGUI()
        {
            ImGui.BeginMainMenuBar();

            bool _ = true;

            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("About Luminal"))
                {
                    
                }

                ImGui.EndMenu();
            }

            ImGuiUtil.RightAlignText($"Luminal v{EngineVersion.Current} - {Timing.FrameRate} fps");

            ImGui.EndMainMenuBar();
        }
    }
}
