using ImGuiNET;
using Luminal.Core;
using Luminal.Graphics;
using Luminal.OpenGL;
using Luminal.OpenGL.Models;
using Luminal.Entities.World;
using OpenTK.Mathematics;
using System.IO;
using SC = SDL2.SDL.SDL_Scancode;
using Luminal.Entities.Components;

namespace Luminal.TestApplication
{
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
            Render.Rectangle(100 + AnimationManager.Get("test", 0.0f), 100, 100, 100, RenderMode.FILL);
        }

        private static System.Numerics.Vector3 AmbientColour = new(1, 1, 1);
        private static System.Numerics.Vector3 DiffuseColour = new(1, 1, 1);
        private static System.Numerics.Vector3 ObjectColour = new(0.7f, 0.7f, 0.7f);

        private static System.Numerics.Vector3 PlayerPos = new();
        private static System.Numerics.Vector3 LightPos = new();

        private static bool CoupleLightToCamera = true;

        private static float shininess = 32.0f;

        public void GUI(Engine _)
        {
            ImGui.Begin("Debug Tool");

            ImGui.ColorEdit3("Ambient colour", ref AmbientColour);
            ImGui.ColorEdit3("Diffuse colour", ref DiffuseColour);
            ImGui.ColorEdit3("Object colour", ref ObjectColour);

            ImGui.SliderFloat("Field of view", ref camera.GetComponent<Camera3D>().FieldOfView, 1f, 179.9f);
            ImGui.SliderFloat("Model angle", ref modelAngle, 0f, 360f);

            ImGui.DragFloat3("Player position", ref PlayerPos, 0.25f);
            ImGui.DragFloat3("Light position", ref LightPos, 0.25f);

            ImGui.SliderFloat("Shininess", ref shininess, 1f, 128f);

            ImGui.Checkbox("Couple light position to camera position", ref CoupleLightToCamera);

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

            ImGui.End();
        }

        private static Object3D camera = new();
        private static Object3D light = new();
        private static Object3D model = new();

        private static Model testModel;

        private void Init(Engine _)
        {
            camera.Position = new Vector3(0f, 0f, -5.0f);
            camera.Euler = Vector3.Zero;

            camera.CreateComponent<Camera3D>();

            light.Position = camera.Position;
            light.Euler = Vector3.Zero;

            light.CreateComponent<PointLight3D>();

            var mr = model.CreateComponent<ModelRenderer>();

            testModel = new("teapot.obj");
            mr.Model = testModel;
        }

        static float modelAngle = 0.0f;

        static System.Numerics.Vector3 TKToSysNum3(Vector3 tk)
        {
            return new(tk.X, tk.Y, tk.Z);
        }

        static Vector3 SysNumToTK3(System.Numerics.Vector3 tk)
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