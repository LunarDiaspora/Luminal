using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Console;
using Luminal.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace Luminal.Entities.Components
{
    public class GridRenderer : Component3D
    {
        private GLShader VS;
        private GLShader FS;
        private GLShaderProgram Program;

        private GLVertexArrayObject VAO = new();

        [ConVar("r_gridsize", "The size of the grid to use for GridRenderers.")]
        public static float GridSize = 1.0f;

        [ConVar("r_gridfalloff", "The distance the grid will cover from your camera, in world units.")]
        public static float GridFalloff = 70f;

        [ConVar("r_gridfalloffcoefficient", "The strength of the grid's fading effect.")]
        public static float FalloffCoefficient = 0.6f;

        [ConVar("r_grids", "Enable or disable rendering of grids engine-wide.")]
        public static bool EnableGrids = true;

        public override void Create()
        {
            VAO.Bind();

            var vsource = File.ReadAllText("EngineResources/Shaders/Grid/grid.vert");
            var fsource = File.ReadAllText("EngineResources/Shaders/Grid/grid.frag");

            VS = new GLShader(vsource, GLShaderType.Vertex);
            FS = new GLShader(fsource, GLShaderType.Fragment);

            Program = new GLShaderProgram()
                .Attach(VS)
                .Attach(FS)
                .Label("GridRenderer Shader")
                .Link();

            GL.BindVertexArray(0);
        }

        public override void EarlyRender3D()
        {
            VAO.Bind();

            Program.Use();

            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            var v = ECSScene.Camera.View();
            var p = ECSScene.Camera.Projection();
            Program.UniformMatrix4("viewMat", ref v);
            Program.UniformMatrix4("projMat", ref p);
            Program.Uniform1("gridSize", GridSize);
            Program.Uniform3("viewPos", ECSScene.Camera.Parent.Position);
            Program.Uniform1("near", ECSScene.Camera.Near);
            Program.Uniform1("gridFalloff", GridFalloff);
            Program.Uniform1("falloffCoeff", FalloffCoefficient);

            // All the geometry calculation is being done in the shader,
            // so we actually need no buffers whatsoever.
            // Just tell it to draw 6 vertices, and you're done.
            if (EnableGrids)
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.Enable(EnableCap.CullFace);

            GL.BindVertexArray(0);
        }
    }
}
