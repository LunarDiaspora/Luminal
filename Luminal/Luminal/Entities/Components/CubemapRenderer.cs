using Luminal.OpenGL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Luminal.Entities.Components
{
    public class CubemapRenderer : Component3D
    {
        public GLCubemap Cubemap;

        private GLShaderProgram CubemapShader;

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

            VBO.Data(skyboxVertices, BufferUsageHint.StaticDraw);

            var VS = File.ReadAllText("EngineResources/cubemap.vert");
            var FS = File.ReadAllText("EngineResources/cubemap.frag");

            var VP = new GLShader(VS, GLShaderType.VERTEX);
            var FP = new GLShader(FS, GLShaderType.FRAGMENT);

            CubemapShader = new GLShaderProgram()
                .Attach(VP)
                .Attach(FP)
                .Label("Cubemap Shader")
                .Link();

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public override void LateRender3D()
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

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            GL.DepthMask(true);
        }
    }
}
