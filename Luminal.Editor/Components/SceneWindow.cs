using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Luminal.Entities.World;

namespace Luminal.Editor.Components
{
    class SceneWindow : Component3D
    {
        public override void Create()
        {
            var _ = Parent.GetOrCreateComponent<InternalComponent>();
        }

        public override void OnGUI()
        {
            ImGui.Begin("Scene");

            int i = 0;
            foreach (var obj in ECSScene.objects)
            {
                if (obj.GetComponent<InternalComponent>() != null)
                {
                    continue;
                }

                i++;

                bool h = ImGui.TreeNodeEx($"{obj.Name} (#{i})", ImGuiTreeNodeFlags.AllowItemOverlap);

                if (ImGui.BeginPopupContextItem())
                {
                    if (ImGui.Selectable("Delete"))
                        obj.Destroy();

                    ImGui.EndPopup();
                }

                if (h)
                {
                    if (typeof(Object3D).IsAssignableFrom(obj.GetType()))
                    {
                        var o3d = (Object3D)obj;
                        var a = o3d.Euler;

                        ImGui.DragFloat3("Position", ref o3d.Position, 0.25f);
                        ImGui.DragFloat3("Angle", ref a, 0.25f);

                        o3d.Euler = a;
                    }

                    ImGui.Separator();

                    int j = 0;
                    foreach (var c in obj.components)
                    {
                        j++;

                        var treeflag = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

                        ImGui.TreeNodeEx($"{c.GetType().Name}##Component{j} - {c.GetType()}", treeflag);
                        if (ImGui.IsItemClicked())
                        {
                            Editor.CurrentlySelected = c;
                        }
                        ImGui.Separator();
                    }

                    ImGui.TreePop();

                }

                ImGui.NewLine();
            }

            ImGui.End();
        }
    }
}
