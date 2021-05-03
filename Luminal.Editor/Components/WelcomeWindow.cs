using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Luminal.Entities;

namespace Luminal.Editor.Components
{
    public class WelcomeWindow : Component3D
    {
        public override void OnGUI()
        {
            var s = ImGui.GetWindowViewport().GetCenter();
            ImGui.SetNextWindowPos(s, ImGuiCond.Appearing, new(0.5f, 0.5f));

            ImGui.Begin("Welcome to Luminal", flags: ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);

            ImGui.Text("Welcome to Luminal. Please select an option:");

            if (ImGui.Button("Load project", new(320, 120)))
            {
                Editor.DoLoadFlow();
            }

            if (ImGui.Button("Create new project", new(320, 120)))
            {
                Editor.SetUpStoredScene();
                Editor.SwitchEditorPhase(EditorPhase.Edit);
            }

            ImGui.End();
        }
    }
}
