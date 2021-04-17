using ImGuiNET;
using Luminal.Core;
using Luminal.Entities;
using Luminal.Entities.Components;
using Luminal.Entities.Screen;
using Luminal.Entities.World;
using Luminal.Graphics;
using Luminal.OpenGL.Models;
using OpenTK.Mathematics;
using SC = SDL2.SDL.SDL_Scancode;

namespace Luminal.TestApplication
{
    internal class DemoWindowComponent : Component3D
    {
        public override void OnGUI()
        {
            ImGui.ShowDemoWindow(ref Enabled);
        }
    }

    internal class DebugTool : Component3D
    {
        public override void OnGUI()
        {
            ImGui.Begin("Debug Tool");

            ImGui.ColorEdit3("Ambient colour", ref Main.AmbientColour);
            ImGui.ColorEdit3("Diffuse colour", ref Main.DiffuseColour);
            ImGui.ColorEdit3("Object colour", ref Main.ObjectColour);

            ImGui.SliderFloat("Field of view", ref Main.camera.GetComponent<Camera3D>().FieldOfView, 1f, 179.9f);
            ImGui.SliderFloat("Model angle", ref Main.modelAngle, 0f, 360f);

            ImGui.DragFloat3("Player position", ref Main.PlayerPos, 0.25f);
            ImGui.DragFloat3("Light position", ref Main.LightPos, 0.25f);

            ImGui.SliderFloat("Shininess", ref Main.shininess, 1f, 128f);

            ImGui.Checkbox("Couple light position to camera position", ref Main.CoupleLightToCamera);

            if (ImGui.CollapsingHeader("Animation controls", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (ImGui.Button("Play"))
                {
                    AnimationManager.Play("test");
                }

                ImGui.SameLine();
                if (ImGui.Button("Pause"))
                {
                    AnimationManager.Pause("test");
                }
            }

            ImGui.Checkbox("Demo window visible", ref Main.camera.GetComponent<DemoWindowComponent>().Enabled);

            ImGui.End();
        }
    }

    internal class Main
    {
        public Main()
        {
            var e = new Engine();

            e.OnDraw += Draw;
            e.OnGUI += GUI;

            e.KeyDown += KeyDown;

            e.OnFinishedLoad += Init;
            e.OnUpdate += Update;

            AnimationManager.AddPaused("test", new()
            {
                Length = 2.0f,
                Loop = true,
                Min = 0.0f,
                Max = 500f,
                Ease = Easing.Exponential.InOut
            });

            e.StartRenderer(1920, 1080, "Luminal Engine 3D Demonstration", typeof(Main),
                LuminalFlags.ENABLE_KEY_REPEAT);
        }

        public void Draw(Engine _)
        {
            Context.SetColour(255, 0, 0, 255);
            Render.Rectangle(100 + AnimationManager.Value("test", 0.0f), 100, 100, 100, RenderMode.FILL);
        }

        internal static System.Numerics.Vector3 AmbientColour = new(1, 1, 1);
        internal static System.Numerics.Vector3 DiffuseColour = new(1, 1, 1);
        internal static System.Numerics.Vector3 ObjectColour = new(0.7f, 0.7f, 0.7f);

        internal static System.Numerics.Vector3 PlayerPos = new();
        internal static System.Numerics.Vector3 LightPos = new();

        internal static bool CoupleLightToCamera = true;

        internal static float shininess = 32.0f;

        public void GUI(Engine _)
        {
        }

        internal static Object3D camera = new();
        internal static Object3D light = new();
        internal static Object3D model = new();

        internal static Object2D test2d = new();

        private static Model testModel;

        private void Init(Engine _)
        {
            camera.Position = new Vector3(0f, 0f, -5.0f);
            camera.Euler = Vector3.Zero;

            camera.CreateComponent<Camera3D>();
            camera.CreateComponent<DebugTool>();
            camera.CreateComponent<DemoWindowComponent>();

            light.Position = camera.Position;
            light.Euler = Vector3.Zero;

            light.CreateComponent<PointLight3D>();

            var mr = model.CreateComponent<ModelRenderer>();

            testModel = new("teapot.obj");
            mr.Model = testModel;

            var ir = test2d.CreateComponent<ImageRenderer>();
            ir.LoadImage("file.jpg");

            test2d.Position.X = 300f;
            test2d.Position.Y = 300f;
        }

        internal static float modelAngle = 0.0f;

        private static System.Numerics.Vector3 TKToSysNum3(Vector3 tk)
        {
            return new(tk.X, tk.Y, tk.Z);
        }

        private static Vector3 SysNumToTK3(System.Numerics.Vector3 tk)
        {
            return new(tk.X, tk.Y, tk.Z);
        }

        private void Update(Engine _, float __)
        {
            camera.Position = SysNumToTK3(PlayerPos);

            if (CoupleLightToCamera)
            {
                light.Position = camera.Position;
            }
            else
            {
                light.Position = SysNumToTK3(LightPos);
            }

            model.Euler = new(0.0f, modelAngle, 0.0f);

            light.GetComponent<PointLight3D>().Shininess = shininess;
            light.GetComponent<PointLight3D>().Colour = DiffuseColour;
        }

        private void KeyDown(Engine _, SC s)
        {
            const float speed = 0.1f;
            const float turnSpeed = 0.7f;

            switch (s)
            {
                case SC.SDL_SCANCODE_S:
                    PlayerPos += TKToSysNum3(camera.Forward * -speed);
                    break;

                case SC.SDL_SCANCODE_W:
                    PlayerPos += TKToSysNum3(camera.Forward * speed);
                    break;

                case SC.SDL_SCANCODE_A:
                    PlayerPos += TKToSysNum3(camera.Right * -speed);
                    break;

                case SC.SDL_SCANCODE_D:
                    PlayerPos += TKToSysNum3(camera.Right * speed);
                    break;

                case SC.SDL_SCANCODE_LEFT:
                    camera.Rotate(new Vector3(0.0f, -turnSpeed, 0.0f));
                    break;

                case SC.SDL_SCANCODE_RIGHT:
                    camera.Rotate(new Vector3(0.0f, turnSpeed, 0.0f));
                    break;

                case SC.SDL_SCANCODE_Q:
                    modelAngle -= turnSpeed;
                    break;

                case SC.SDL_SCANCODE_E:
                    modelAngle += turnSpeed;
                    break;

                case SC.SDL_SCANCODE_R:
                    PlayerPos += TKToSysNum3(camera.Up * speed);
                    break;

                case SC.SDL_SCANCODE_F:
                    PlayerPos += TKToSysNum3(-camera.Up * speed);
                    break;

                case SC.SDL_SCANCODE_UP:
                    camera.Rotate(new Vector3(turnSpeed, 0.0f, 0.0f));
                    break;

                case SC.SDL_SCANCODE_DOWN:
                    camera.Rotate(new Vector3(-turnSpeed, 0.0f, 0.0f));
                    break;
            }
        }
    }

    internal class Entrypoint
    {
        private static void Main(string[] args)
        {
            new Main();
        }
    }
}