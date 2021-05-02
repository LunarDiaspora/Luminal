﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using ImGuizmoNET;
using Luminal.Console;
using Luminal.Entities;
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

            var l = ImGui.GetWindowContentRegionMin();
            var h = ImGui.GetWindowContentRegionMax();

            var size = h - l;

            ECSScene.RenderTexture.Size = size;
            ImGui.Image(new IntPtr(ECSScene.RenderTexture.GLTextureObject), size,
                new(0, 1), new(1, 0));
            // This renders the texture technically upside-down how OpenGL sees it
            // but since OpenGL textures are already upside-down it's uno reverse carding opengl's flip and it's normal now

            ImGui.End();
        }
    }
}
