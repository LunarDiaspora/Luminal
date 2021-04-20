using ImGuiNET;
using Luminal.Entities;
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
            ImGui.Begin("Component Inspector");

            var c = Editor.CurrentlySelected;

            if (c == null)
            {
                ImGui.Text("Select a component from the scene view.");
            } else
            {
                ImGui.Text($"{((Component3D)c).Parent.Name} > {c.GetType().Name}");
                ImGui.Separator();

                ImGui.Checkbox("Enabled?", ref c.Enabled);
                ImGui.Separator();

                var props = c.GetType().GetFields();
                foreach (var p in props)
                {
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
                }
            }

            ImGui.End();
        }
    }
}
