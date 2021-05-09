using Luminal.OpenGL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Luminal.Console;

namespace Luminal.Entities.Components
{
    public class CubemapRenderer : Component3D
    {
        public GLCubemap Cubemap;

        private GLShaderProgram CubemapShader;

        [ConVar("r_cubemaps", "Enable or disable rendering of cubemaps. (skyboxes)")]
        public static bool RenderCubemaps = true;

        private static float[] skyboxVertices = {
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
        };

        private GLVertexArrayObject VAO;
        private GLFloatBuffer VBO;

        public override void Create()
        {
            VAO = new("Cubemap VAO");
            VBO = new("Cubemap VBO");

            VAO.Bind();

            VBO.Bind(BufferTarget.ArrayBuffer);

            VBO.Data(skyboxVertices, BufferUsageHint.DynamicDraw);

            var VS = File.ReadAllText("EngineResources/Shaders/Cubemap/cubemap.vert");
            var FS = File.ReadAllText("EngineResources/Shaders/Cubemap/cubemap.frag");

            var VP = new GLShader(VS, GLShaderType.Vertex);
            var FP = new GLShader(FS, GLShaderType.Fragment);

            CubemapShader = new GLShaderProgram()
                .Label("Cubemap Shader")
                .Attach(VP)
                .Attach(FP)
                .Link();

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.BindVertexArray(0);
        }

        public override void EarlyRender3D()
        {
            VAO.Bind();
            Cubemap.Bind();

            GL.DepthMask(false);
            CubemapShader.Use();

            var view = ECSScene.Camera.View();
            var viewWithoutTrans = new Matrix4(new Matrix3(view));

            CubemapShader.UniformMatrix4("View", ref viewWithoutTrans);

            var proj = ECSScene.Camera.Projection();

            CubemapShader.UniformMatrix4("Projection", ref proj);

            CubemapShader.Uniform1i("Cubemap", 0);

            if (RenderCubemaps)
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            GL.DepthMask(true);

            GL.BindVertexArray(0);
        }
    }
}
