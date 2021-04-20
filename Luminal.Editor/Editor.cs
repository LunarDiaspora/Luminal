using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Core;
using Luminal.Entities.Components;
using ImGuiNET;
using Luminal.Entities.World;
using Luminal.Editor.Components;
using Luminal.Entities;

namespace Luminal.Editor
{
    internal class Editor
    {
        public static Object3D Camera;
        public static Object3D GUI;

        internal static Object3D TestModel;

        public static void Init()
        {
            Camera = new Object3D();
            Camera.CreateComponent<Camera3D>();

            GUI = new Object3D();
            GUI.CreateComponent<EditorWindow>();
            GUI.CreateComponent<MenuBar>();

            TestModel = new();
            var m = TestModel.CreateComponent<ModelRenderer>();
            m.Model = new("test.obj");
            TestModel.Position = new(0f, 0f, -5f);

            var l = new Object3D();
            l.CreateComponent<PointLight3D>();

            ECSScene.RenderTexture = new();
        }
    }
}
