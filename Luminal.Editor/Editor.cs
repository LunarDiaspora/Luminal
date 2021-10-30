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
using ImGuizmoNET;
using Luminal.OpenGL;
using Luminal.Console;
using Luminal.Player.Core;
using Luminal.Audio;
using Luminal.UI;

namespace Luminal.Editor
{
    internal enum EditorPhase
    {
        Welcome,
        Edit,
        Play
    }

    internal class SceneDebugger
    {
        public static void OnGUI()
        {
            ImGui.Begin("Scene debugger");

            if (ImGui.Button("Editor.Scene.SetActive()")) Editor.Scene.SetActive();
            if (ImGui.Button("Editor.WelcomeScene.SetActive()")) Editor.WelcomeScene.SetActive();
            if (ImGui.Button("Scene.Deactivate()")) Scene.Deactivate();

            ImGui.End();
        }
    }

    internal class TestComponent : Component3D
    {
        public override void Update()
        {
            Parent.Translate(Parent.Right * Timing.DeltaTime * 5.0f);
        }
    }

    internal class Editor
    {
        public static Object3D Camera;
        public static Object3D GUI;

        internal static Object3D TestModel;

        public static Component CurrentlySelected;

        public static Object3D SelectedObject;

        public static bool ObjectSelected = false;

        public static bool Playing = false;

        public static Scene WelcomeScene = new();

        public static Scene Scene = new();

        public static void Init()
        {
            ECSScene.DontDrawTextureToScreen = true;

            SetUpStoredScene();

            SwitchEditorPhase(EditorPhase.Welcome);
            Engine.VSync = VSyncMode.Sync;

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        }

        public static void SwitchEditorPhase(EditorPhase n)
        {
            switch (n)
            {
                case EditorPhase.Welcome:
                    BeginWelcomePhase();
                    break;
                case EditorPhase.Edit:
                    BeginEditPhase();
                    break;
                case EditorPhase.Play:
                    BeginPlayPhase();
                    break;
                default:
                    break;
            }
        }

        public static void BeginWelcomePhase()
        {
            ECSScene.Disable3D = true;
            WelcomeScene.SetActive();
        }

        public static void SetUpStoredScene()
        {
            WelcomeScene = new();

            WelcomeScene.SetActive();
            var o = new Object3D();
            o.CreateComponent<WelcomeMenuBar>();
            o.CreateComponent<WelcomeWindow>();
            Scene.Deactivate();


            Scene = new();

            Scene.SetActive();

            Camera = new Object3D("Main Camera");
            Camera.CreateComponent<Camera3D>();

            Camera.Position = new(0f, -3f, 0f);

            Camera.CreateComponent<SpotLight3D>();

            GUI = new Object3D("Editor GUI. You should not see this!");
            GUI.CreateComponent<Dockspace>();
            GUI.CreateComponent<ViewportWindow>();
            GUI.CreateComponent<MenuBar>();
            GUI.CreateComponent<SceneWindow>();
            GUI.CreateComponent<InspectorWindow>();
            GUI.CreateComponent<GridRenderer>();
            GUI.CreateComponent<Toolbar>();

            TestModel = new("Teapot");
            var m = TestModel.CreateComponent<ModelRenderer>();
            m.Model = new("test2.obj");
            TestModel.Position = new(0f, 0f, -5f);
            TestModel.CreateComponent<TestComponent>();

            var l = new Object3D("Light");
            //l.CreateComponent<PointLight3D>();

            // 3D audio test
            //var t = new Object3D("3D audio test");
            //var sr = t.CreateComponent<AudioSource>();
            //sr.Audio = new AudioFile("test.wav");
            //sr.PlayOnEnteringPlayMode = true;

            Scene.Deactivate();
        }

        public static void BeginEditPhase()
        {
            EnginePlayer.StopPlaying();

            Engine.ConsoleOpen = true;

            ECSScene.Disable3D = false;
            ECSScene.UseRenderTexture = true;
            Scene.SetActive();
        }

        public static void BeginPlayPhase()
        {
            EnginePlayer.Play();
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
            var pan = new FileBrowserPanel(System.IO.Directory.GetCurrentDirectory(),
                "Luminal projects,.luminal;All files,*");

            pan.Selected += e =>
            {
                Log.Debug($"Path: {e.AbsolutePath}");
            };
        }
    }
}
