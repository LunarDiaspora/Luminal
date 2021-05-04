using ImGuiNET;
using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImGuiNET.ImGuiWindowFlags;

namespace Luminal.Editor.Components
{
    public class Dockspace : Component3D
    {
        public override void OnGUI()
        {
            var flags = ImGuiDockNodeFlags.None;

            var vp = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(vp.WorkPos);
            ImGui.SetNextWindowSize(vp.WorkSize);
            ImGui.SetNextWindowViewport(vp.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);

            var winflags = NoTitleBar | NoCollapse | NoResize | NoMove |
                           NoBringToFrontOnFocus | NoNavFocus;

            ImGui.Begin("Custom Dockspace", winflags);

            ImGui.PopStyleVar(2);

            var id = ImGui.GetID("DockspaceComponent");
            ImGui.DockSpace(id, new(0f, 0f), flags);

            ImGui.End();
        }
    }
}
