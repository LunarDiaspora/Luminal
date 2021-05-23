using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Luminal.Core;
using Luminal.Logging;
using Luminal.Entities.World;
using Luminal.Player.Core;
using Luminal.Reflection;

namespace Luminal.Editor.Components
{
    [Skip]
    public class ImGuiDemoWindowComponent : Component3D
    {
        public bool open = true;
        public override void OnGUI()
        {
            ImGui.ShowDemoWindow(ref open);

            if (!open)
                Enabled = false;
        }
    }

    [Skip]
    public class MenuBar : Component3D
    {
        private string objName = "";

        public override void Create()
        {
            var _ = Parent.GetOrCreateComponent<InternalComponent>();
        }

        public override void OnGUI()
        {
            bool open = false;
            bool _ = true;


            if (ImGui.BeginMainMenuBar())
            {

                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Save scene..."))
                    {
                        var s = SceneSerialiser.Serialise();
                        Log.Debug(s);
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Object"))
                {
                    if (ImGui.MenuItem("Create new object..."))
                    {
                        open = true;
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Mode"))
                {
                    if (ImGui.MenuItem("Edit"))
                        Editor.SwitchEditorPhase(EditorPhase.Edit);

                    if (ImGui.MenuItem("Play"))
                        Editor.SwitchEditorPhase(EditorPhase.Play);

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Tools"))
                {
                    if (ImGui.MenuItem("Show ImGui demo window..."))
                    {
                        var cp = Parent.GetOrCreateComponent<ImGuiDemoWindowComponent>();
                        cp.open = true;
                        cp.Enabled = true;
                    }

                    ImGui.EndMenu();
                }

                ImGuiUtil.RightAlignText($"Luminal v{EngineVersion.Current} - {Timing.FrameRate} fps");

                ImGui.EndMainMenuBar();

            }

            if (open)
            {
                ImGui.OpenPopup("Create new object");
            }

            var sc = ImGui.GetWindowViewport().GetCenter();
            ImGui.SetNextWindowPos(sc, ImGuiCond.Appearing, new(0.5f, 0.5f));

            if (ImGui.BeginPopupModal("Create new object", ref _, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text("You are creating an object.\nEnter its name:");
                var h = ImGui.InputText("Name", ref objName, 65536, ImGuiInputTextFlags.EnterReturnsTrue);

                if (ImGui.Button("No, don't", new(180, 0)))
                {
                    objName = "";
                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();

                if (ImGui.Button("Create!", new(180, 0)) || h)
                {
                    if (objName != "")
                        new Object3D(objName);
                    objName = "";
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

        }
    }
}
