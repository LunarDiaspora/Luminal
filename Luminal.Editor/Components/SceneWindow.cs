using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Luminal.Entities.World;
using Luminal.Core;
using Luminal.Reflection;

namespace Luminal.Editor.Components
{
    [Skip]
    class SceneWindow : Component3D
    {
        public override void Create()
        {
            var _ = Parent.GetOrCreateComponent<InternalComponent>();

            UpdateComponents();
        }

        static HashSet<Type> types = new(); // Hashsets are automatically deduped

        void UpdateComponents()
        {
            var t = AppDomain.CurrentDomain;
            var asms = t.GetAssemblies();
            foreach (var asm in asms)
            {
                foreach (var tp in asm.GetTypes())
                {
                    // Get all of them.
                    if (tp.IsAssignableTo(typeof(Component3D)))
                    {
                        // This is a component3d
                        if (tp == typeof(Component3D))
                            continue;

                        var skips = tp.GetCustomAttributes(typeof(SkipAttribute), false);

                        if (skips.Length != 0)
                            continue;

                        types.Add(tp);
                    }
                }
            }
        }

        public override void OnGUI()
        {
            ImGui.Begin("Scene");

            int i = 0;
            foreach (var obj in ECSScene.CurrentScene.Objects)
            {
                if (obj.GetComponent<InternalComponent>() != null)
                {
                    continue;
                }

                i++;

                bool h = ImGui.TreeNodeEx($"{obj.Name}##O {obj.ID}", ImGuiTreeNodeFlags.AllowItemOverlap);

                if (ImGui.IsItemClicked())
                {
                    Editor.SelectedObject = (Object3D)obj;
                    Editor.ObjectSelected = true;
                }

                if (ImGui.BeginPopupContextItem())
                {
                    if (ImGui.Selectable("Delete"))
                        obj.Destroy();

                    ImGui.Separator();

                    foreach (var t in types)
                    {
                        if (ImGui.Selectable(t.Name))
                        {
                            var gen = typeof(Object3D)
                                .GetMethod("CreateComponent")
                                .MakeGenericMethod(t);
                            gen.Invoke(obj, Array.Empty<object>());
                        }
                    }

                    ImGui.EndPopup();
                }

                if (h)
                {
                    if (typeof(Object3D).IsAssignableFrom(obj.GetType()))
                    {
                        var o3d = (Object3D)obj;
                        var a = o3d.Euler;

                        if (!Engine.Playing)
                        {
                            ImGui.DragFloat3("Position", ref o3d.Position, 0.25f);
                            ImGui.DragFloat3("Angle", ref a, 0.25f);
                        }

                        o3d.Euler = a;
                    }

                    ImGui.Separator();

                    int j = 0;
                    foreach (var c in obj.Components)
                    {
                        j++;

                        var treeflag = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

                        ImGui.TreeNodeEx($"{c.GetType().Name}##C {c.ID}", treeflag);
                        if (ImGui.IsItemClicked())
                        {
                            Editor.CurrentlySelected = (Component)c;
                            Editor.ObjectSelected = false;
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
