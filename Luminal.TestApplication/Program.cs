using ImGuiNET;
using Luminal.Core;
using Luminal.Graphics;
using Luminal.OpenGL;
using Luminal.OpenGL.Models;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.IO;
using SC = SDL2.SDL.SDL_Scancode;

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

            OpenGLManager.OnInitGL += InitGL;
            OpenGLManager.OnEarlyOpenGL += GLDraw;

            e.StartRenderer(1280, 720, "Luminal Engine", typeof(Main), LuminalFlags.ENABLE_USER_OPENGL);
        }

        public void Draw(Engine _)
        {
            Context.SetColour(255, 0, 0, 255);
            Render.Rectangle(100, 100, 100, 100, RenderMode.FILL);
        }

        public void GUI(Engine _)
        {
            ImGui.ShowDemoWindow();

            ImGui.Begin("Sample IMGUI Window", ImGuiWindowFlags.NoCollapse);

            ImGui.Text("As you can see, it is working.");

            ImGui.End();
        }

        private static GLShader VS;
        private static GLShader FS;

        private static GLShaderProgram Program;

        private static Camera camera = new(new Vector3(0f, 0f, -5.0f), Vector3.Zero);

        private static Model testModel;

        private void InitGL()
        {
            var vsSource = File.ReadAllText("EngineResources/mesh.vert");
            var fsSource = File.ReadAllText("EngineResources/untextured.frag");

            VS = new GLShader(vsSource, GLShaderType.VERTEX);
            FS = new GLShader(fsSource, GLShaderType.FRAGMENT);

            Program = new GLShaderProgram().Label("Scene").Attach(VS).Attach(FS).Link();

            testModel = new("test.obj");
        }

        private void GLDraw()
        {
            // GL calls go here.
            // This is your draw loop.
            //GL.BindTexture(TextureTarget.Texture2D, 0);

            Program.Use();

            var model = Matrix4.CreateRotationY(0.0f);
            var view = camera.View();
            var proj = camera.Projection();

            Program.UniformMatrix4("Model", ref model);
            Program.UniformMatrix4("View", ref view);
            Program.UniformMatrix4("Projection", ref proj);

            testModel.Draw();
        }

        private void KeyDown(Engine _, SC s)
        {
            const float speed = 0.1f;
            const float turnSpeed = 0.7f;

            switch (s)
            {
                case SC.SDL_SCANCODE_S:
                    camera.Translate(camera.Forward * -speed);
                    break;

                case SC.SDL_SCANCODE_W:
                    camera.Translate(camera.Forward * speed);
                    break;

                case SC.SDL_SCANCODE_A:
                    camera.Translate(camera.Right * speed);
                    break;

                case SC.SDL_SCANCODE_D:
                    camera.Translate(camera.Right * -speed);
                    break;

                case SC.SDL_SCANCODE_LEFT:
                    camera.Rotate(new Vector3(0.0f, -turnSpeed, 0.0f));
                    break;

                case SC.SDL_SCANCODE_RIGHT:
                    camera.Rotate(new Vector3(0.0f, turnSpeed, 0.0f));
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