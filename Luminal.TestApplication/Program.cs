using System;
using Luminal.Core;
using Luminal.Graphics;
using ImGuiNET;
using Luminal.OpenGL;
using OpenTK.Graphics.OpenGL;
using System.IO;
using OpenTK.Mathematics;

using SC = SDL2.SDL.SDL_Scancode;

namespace Luminal.TestApplication
{
    class Main
    {
        public Main()
        {
            var e = new Engine();

            e.OnDraw += Draw;
            e.OnGUI += GUI;

            e.KeyDown += KeyDown;

            OpenGLManager.OnInitGL += InitGL;
            OpenGLManager.OnOpenGL += GLDraw;

            e.StartRenderer(1280, 720, "Luminal Engine", typeof(Main), LuminalFlags.ENABLE_USER_OPENGL);
        }

        public void Draw(Engine _)
        {
            Context.SetColour(255, 0, 0, 255);
            Render.Rectangle(100, 100, 100, 100, RenderMode.FILL);
        }

        public void GUI(Engine _)
        {
            ImGui.Begin("Sample IMGUI Window", ImGuiWindowFlags.NoCollapse);

            ImGui.Text("As you can see, it is working.");

            ImGui.End();
        }

        static GLTexture Texture;
        static GLVertexArrayObject VAO = new("My VAO");
        static GLUIntBuffer EBO = new();
        static GLFloatBuffer VBO = new();

        static GLShader VS;
        static GLShader FS;

        static GLShaderProgram Program;

        static Camera camera = new Camera(new Vector3(0f, 0f, -3.0f), Vector3.Zero);

        void InitGL()
        {
            var vsSource = File.ReadAllText("EngineResources/standard.vert");
            var fsSource = File.ReadAllText("EngineResources/standard.frag");

            VS = new GLShader(vsSource, GLShaderType.VERTEX);
            FS = new GLShader(fsSource, GLShaderType.FRAGMENT);

            VAO.Bind();

            Program = new GLShaderProgram().Attach(VS).Attach(FS).Link();

            GL.Disable(EnableCap.CullFace);

            Texture = new GLTexture("Texture", "file.jpg");

            Texture.ActiveBind(TextureUnit.Texture0);
        }

        void GLDraw()
        {
            // GL calls go here.
            // This is your draw loop.

            float[] vertices = // X Y Z R G B A U V
            {
                -0.5f, -0.5f, 0.5f,   1.0f, 1.0f, 1.0f, 1.0f,  0.0f, 1.0f, // BL
                0.5f, -0.5f, 0.5f,   1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f, // BR
                -0.5f, 0.5f, 0.5f,   1.0f, 1.0f, 1.0f, 1.0f,  0.0f, 0.0f, // TL
                0.5f, 0.5f, 0.5f,    1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 0.0f  // TR
            };

            uint[] indices =
            {
                0, 1, 3,
                3, 2, 0
            };

            VAO.Bind();

            EBO.Bind(BufferTarget.ElementArrayBuffer);
            EBO.Data(indices, BufferUsageHint.DynamicDraw);

            VBO.Bind(BufferTarget.ArrayBuffer);
            VBO.Data(vertices, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 7 * sizeof(float));

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            VAO.Bind();
            Program.Use();

            Texture.ActiveBind(TextureUnit.Texture0);

            Program.Uniform2("ScreenSize", Engine.Width, Engine.Height);


            var Model = Matrix4.CreateRotationY(0.0f);
            var View = camera.View();
            var Projection = camera.Projection();

            Program.UniformMatrix4("Model", ref Model);
            Program.UniformMatrix4("View", ref View);
            Program.UniformMatrix4("Projection", ref Projection);

            EBO.Bind(BufferTarget.ElementArrayBuffer);

            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 3);
        }

        void KeyDown(Engine _, SC s)
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

    class Entrypoint
    {
        static void Main(string[] args)
        {
            new Main();
        }
    }
}
