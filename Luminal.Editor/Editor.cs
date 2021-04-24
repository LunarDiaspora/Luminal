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
using Luminal.Logging;
using System.Windows.Forms;
using System.Threading;

namespace Luminal.Editor
{
    internal enum EditorPhase
    {
        WELCOME,
        EDIT,
        PLAY
    }

    internal class Editor
    {
        public static Object3D Camera;
        public static Object3D GUI;

        internal static Object3D TestModel;

        public static Component CurrentlySelected;

        public static void Init()
        {
            SwitchEditorPhase(EditorPhase.WELCOME);
        }

        public static void SwitchEditorPhase(EditorPhase n)
        {
            ECSScene.L3D_SceneEnding();

            switch (n)
            {
                case EditorPhase.WELCOME:
                    BeginWelcomePhase();
                    break;
                case EditorPhase.EDIT:
                    BeginEditPhase();
                    break;
                default:
                    break;
            }
        }

        public static void BeginWelcomePhase()
        {
            ECSScene.Disable3D = true;

            var o = new Object3D();
            o.CreateComponent<WelcomeMenuBar>();
            o.CreateComponent<WelcomeWindow>();
            o.CreateComponent<ConvarTest>();
        }

        public static void BeginEditPhase()
        {
            ECSScene.Disable3D = false;

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

            Engine.VSync = VSyncMode.SYNC;
        }

        class LoadThread : IDisposable
        {
            public delegate void File(string name);
            public event File OnFileChosen;
            private OpenFileDialog fod = new();

            private void procedure()
            {
                if (fod.ShowDialog() == DialogResult.OK)
                {
                    OnFileChosen?.Invoke(fod.FileName);
                }
            }

            public void Show(string filter)
            {
                fod.Filter = filter;
                var t = new Thread(procedure);
                t.SetApartmentState(ApartmentState.STA); // Necessary else winforms is not happy
                t.Start();
            }

            public void Dispose()
            {
                fod.Dispose();
            }
        }

        public static void DoLoadFlow()
        {
            using var fod = new LoadThread();
            var fil = "Luminal Engine project files (.luminal)|*.luminal";
            fod.OnFileChosen += d =>
            {
                Log.Debug(d);
            };
            fod.Show(fil);
        }
    }
}
