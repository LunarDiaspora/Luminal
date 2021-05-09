using Luminal.Entities;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.OpenGL
{
    public class GLOmnidirectionalShadowmap
    {
        public int Cubemap;
        public int DepthMapFBO;
        public GLShaderProgram ShadowShader;

        public const int SHADOW_WIDTH = 1024;
        public const int SHADOW_HEIGHT = 1024;

        public GLOmnidirectionalShadowmap()
        {
            Cubemap = GL.GenTexture();

            GL.BindTexture(TextureTarget.TextureCubeMap, Cubemap);

            for (uint i=0; i<6; i++)
            {
                GL.TexImage2D((TextureTarget)((uint)TextureTarget.TextureCubeMapPositiveX + i),
                    0, PixelInternalFormat.DepthComponent, SHADOW_WIDTH, SHADOW_HEIGHT, 0,
                    PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (uint)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (uint)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (uint)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (uint)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (uint)TextureWrapMode.ClampToEdge);

            GL.CreateFramebuffers(1, out int DepthMapFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, DepthMapFBO);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                Cubemap, 0);

            // We dc abt the colour data
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            var vsource = File.ReadAllText("EngineResources/Shaders/Shading/shadow_omnidir.vert");
            var fsource = File.ReadAllText("EngineResources/Shaders/Shading/shadow_omnidir.frag");
            var gsource = File.ReadAllText("EngineResources/Shaders/Shading/shadow_omnidir.geom");

            var vshader = new GLShader(vsource, GLShaderType.Vertex);
            var fshader = new GLShader(fsource, GLShaderType.Fragment);
            var gshader = new GLShader(gsource, GLShaderType.Geometry);

            ShadowShader = new GLShaderProgram()
                .Attach(vshader)
                .Attach(fshader)
                .Attach(gshader)
                .Label("Omnidirectional Shadowmap Shader")
                .Link();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // And unbind
        }

        public void RenderToDepth(System.Numerics.Vector3 LightPos)
        {
            GL.Viewport(0, 0, SHADOW_WIDTH, SHADOW_HEIGHT);
            var aspect = (float)SHADOW_WIDTH / SHADOW_HEIGHT;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, DepthMapFBO);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var near = 1.0f;
            var far = 25.0f;

            Matrix4 shadProj = Matrix4.CreatePerspectiveFieldOfView(
                GLHelper.DegRad(90.0f), aspect, near, far); // THIS NEEDS TO BE 90! I might make it a var anyway for shits and giggles

            List<Matrix4> Transforms = new();

            Vector3 right = new(1.0f, 0.0f, 0.0f);
            Vector3 left = new(-1.0f, 0.0f, 0.0f);
            Vector3 up = new(0.0f, 1.0f, 0.0f);
            Vector3 down = new(0.0f, -1.0f, 0.0f);
            Vector3 forward = new(0.0f, 0.0f, 1.0f);
            Vector3 back = new(0.0f, 0.0f, -1.0f);

            var lptk = LightPos.ToOpenTK();

            Transforms.Add(
                shadProj * Matrix4.LookAt(
                    lptk, lptk + right, down));
            Transforms.Add(
                shadProj * Matrix4.LookAt(
                    lptk, lptk + left, down));
            Transforms.Add(
                shadProj * Matrix4.LookAt(
                    lptk, lptk + up, forward));
            Transforms.Add(
                shadProj * Matrix4.LookAt(
                    lptk, lptk + down, back));
            Transforms.Add(
                shadProj * Matrix4.LookAt(
                    lptk, lptk + forward, down));
            Transforms.Add(
                shadProj * Matrix4.LookAt(
                    lptk, lptk + back, down));

            GL.BindTexture(TextureTarget.TextureCubeMap, Cubemap);



            ECSScene.ExplicitRender();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
