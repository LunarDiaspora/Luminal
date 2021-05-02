using ImGuiNET;
using Luminal.Entities;
using Luminal.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Editor.Components
{
    class InspectorWindow : Component3D
    {
        public override void OnGUI()
        {
            ImGui.Begin("Inspector");

            if (!Editor.ObjectSelected)
                DoComponent();
            else
                DoObject();

            ImGui.End();
        }

        void DoObject()
        {
            var c = Editor.SelectedObject;

            if (c == null)
            {
                ImGui.Text("Select an item from the scene view.");
            } else
            {
                ImGui.SetNextItemWidth(-1);
                ImGui.Text($"{c.Name}");
                if (ImGui.BeginPopupContextWindow("Edit name", ImGuiPopupFlags.MouseButtonLeft | ImGuiPopupFlags.MouseButtonRight))
                {
                    ImGui.InputTextWithHint("", "object name", ref c.Name, 65536);
                    ImGui.EndPopup();
                }

                ImGui.Separator();

                ImGui.DragFloat3("Position", ref c.Position);
                var a = c.Euler;
                ImGui.DragFloat3("Rotation", ref a);
                c.Euler = a;

                ImGui.Separator();

                foreach (var i in c.components)
                {
                    ImGui.SetNextItemWidth(-1);
                    
                    if (ImGui.Selectable($"{i.GetType().Name}##{i.ID}"))
                    {
                        Editor.ObjectSelected = false;
                        Editor.CurrentlySelected = i;
                    }

                    ImGui.Separator();
                }
            }
        }

        void DoComponent()
        {
            var c = Editor.CurrentlySelected;

            if (c == null)
            {
                ImGui.Text("Select an item from the scene view.");
            }
            else
            {
                if (ImGui.Selectable($"{((Component3D)c).Parent.Name}"))
                {
                    Editor.SelectedObject = ((Component3D)c).Parent;
                    Editor.ObjectSelected = true;
                    return;
                }

                ImGui.SameLine();
                ImGui.Text($"> {c.GetType().Name}");
                ImGui.Separator();

                ImGui.Checkbox("Enabled?", ref c.Enabled);
                ImGui.Separator();

                var props = c.GetType().GetFilteredFields();
                foreach (var p in props)
                {
                    if (p.Name == "Type") continue; // hidden.

                    var v = p.GetValue(c);
                    if (p.FieldType == typeof(float))
                    {
                        var fl = (float)v;
                        ImGui.DragFloat(p.Name, ref fl);
                        p.SetValue(c, fl);
                    }
                    else if (p.FieldType == typeof(string))
                    {
                        var s = (string)v;
                        ImGui.InputText(p.Name, ref s, 65536);
                        p.SetValue(c, s);
                    }
                    else if (p.FieldType == typeof(bool))
                    {
                        var b = (bool)v;
                        ImGui.Checkbox(p.Name, ref b);
                        p.SetValue(c, b);
                    }
                }
            }
        }
    }
}
