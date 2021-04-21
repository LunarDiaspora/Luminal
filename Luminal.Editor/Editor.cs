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
using OpenTK.Mathematics;

namespace Luminal.Editor
{
    internal class Editor
    {
        public static Object3D Camera;
        public static Object3D GUI;

        internal static Object3D TestModel;

        public static Component CurrentlySelected;

        public static void Init()
        {
            Camera = new Object3D("Main Camera");
            Camera.CreateComponent<Camera3D>();

            GUI = new Object3D("Editor GUI. You should not see this!");
            GUI.CreateComponent<ViewportWindow>();
            GUI.CreateComponent<MenuBar>();
            GUI.CreateComponent<SceneWindow>();
            GUI.CreateComponent<InspectorWindow>();

            TestModel = new("Teapot");
            var m = TestModel.CreateComponent<ModelRenderer>();
            m.Model = new("test.obj");
            TestModel.Position = new(0f, 0f, -5f);

            var l = new Object3D("Light");
            l.CreateComponent<PointLight3D>();

            ECSScene.RenderTexture = new();

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            var o = new Newtonsoft.Json.JsonSerializerSettings
            {
                Converters =
                {
                    new Luminal.Player.Core.QuatSerializer()
                }
            };
            var q = Newtonsoft.Json.JsonConvert.DeserializeObject<Quaternion>("{\"X\":1.0}", o);

            Engine.VSync = VSyncMode.SYNC;
        }
    }
}
