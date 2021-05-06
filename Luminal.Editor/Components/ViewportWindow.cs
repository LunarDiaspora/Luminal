using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using ImGuizmoNET;
using Luminal.Console;
using Luminal.Entities;
using Luminal.Entities.World;
using Luminal.OpenGL;
using OpenTK.Mathematics;

namespace Luminal.Editor.Components
{
    class ViewportWindow : Component3D
    {
        public override void Create()
        {
            var _ = Parent.GetOrCreateComponent<InternalComponent>();
        }

        public override void OnGUI()
        {
            ImGui.Begin("Viewport");

            if (ECSScene.Camera == null)
            {
                ImGui.Text("Camera missing.");

                ImGui.End();
            }

            var l = ImGui.GetWindowContentRegionMin();
            var h = ImGui.GetWindowContentRegionMax();

            var size = h - l;

            ECSScene.RenderTexture.Size = size;
            ImGui.Image(new IntPtr(ECSScene.RenderTexture.ResolveTex), size,
                new(0, 1), new(1, 0));
            // This renders the texture technically upside-down how OpenGL sees it
            // but since OpenGL textures are already upside-down it's uno reverse carding opengl's flip and it's normal now

            var wp = ImGui.GetWindowPos();
            ImGuizmo.Enable(ImGui.IsMouseHoveringRect(wp, wp+size));

            // Gizmos
            var dl = ImGui.GetForegroundDrawList();
            ImGuizmo.SetOrthographic(false);

            var cr = ImGui.GetWindowContentRegionMax();

            ImGuizmo.SetRect(wp.X, wp.Y, cr.X, cr.Y);

            ImGuizmo.SetDrawlist(dl);

            if (Editor.SelectedObject != null && Editor.ObjectSelected)
            {
                var v = ECSScene.Camera.View();
                var p = ECSScene.Camera.Projection();

                var a = Editor.SelectedObject.ModelMatrix;

                var op = Toolbar.ActiveMode switch
                {
                    MovementMode.Translate => OPERATION.TRANSLATE,
                    MovementMode.Rotate => OPERATION.ROTATE,
                    _ => OPERATION.TRANSLATE
                };

                var m = ImGuizmo.Manipulate(ref v.Row0.X, ref p.Row0.X, op, MODE.LOCAL, ref a.Row0.X);

                if (m)
                {
                    Editor.SelectedObject.ModelMatrix = a;
                }
            }

            ImGui.End();
        }
    }
}
