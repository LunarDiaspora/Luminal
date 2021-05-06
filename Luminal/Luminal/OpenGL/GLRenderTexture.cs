using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Core;
using OpenTK.Graphics.OpenGL;
using System.Numerics;
using Luminal.Logging;
using Luminal.Console;

namespace Luminal.OpenGL
{
    public class GLRenderTexture
    {
        public int GLObject;
        public int GLTextureObject;
        public int GLRenderBufferObject;

        public int NoAA_GLObject;
        public int NoAA_GLTexture;
        public int NoAA_GLRBO;

        public int ResolveGLObject;
        public int ResolveTex;

        public Vector2 Size = new(Engine.Width, Engine.Height);

        private Vector2 SizeLastUse = new(Engine.Width, Engine.Height);
        private int samplesLastUse = 2;

        [ConVar("r_antialiasing", "Dictates whether rendertextures use the antialiased versions or not.")]
        public static bool UseAntialiasing = true;

        static bool lastUse = true;

        public GLRenderTexture()
        {
            GL.CreateFramebuffers(1, out GLObject);

            GLTextureObject = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2DMultisample, GLTextureObject);

            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, OptionsWindow.AASamples, PixelInternalFormat.Rgb,
                (int)Size.X, (int)Size.Y, true);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLObject);

            GL.FramebufferParameter(FramebufferTarget.Framebuffer, FramebufferDefaultParameter.FramebufferDefaultSamples,
                OptionsWindow.AASamples);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2DMultisample, GLTextureObject, 0);

            GLRenderBufferObject = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, GLRenderBufferObject);

            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, OptionsWindow.AASamples,
                RenderbufferStorage.Depth24Stencil8,
                (int)Size.X, (int)Size.Y);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, GLRenderBufferObject);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Now time for the non-antialiased calls
            GL.CreateFramebuffers(1, out NoAA_GLObject);

            NoAA_GLTexture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, NoAA_GLTexture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                (int)Size.X, (int)Size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, NoAA_GLObject);

            GL.FramebufferParameter(FramebufferTarget.Framebuffer, FramebufferDefaultParameter.FramebufferDefaultSamples,
                1);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, NoAA_GLTexture, 0);

            NoAA_GLRBO = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, NoAA_GLRBO);

            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8,
                (int)Size.X, (int)Size.Y);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, NoAA_GLRBO);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Resolving
            GL.CreateFramebuffers(1, out ResolveGLObject);
            GLHelper.LabelObj(ObjectLabelIdentifier.Framebuffer, ResolveGLObject, "Luminal RenderTexture Resolution FBO");

            GLHelper.Texture(TextureTarget.Texture2D, "Luminal RenderTexture Resolution Texture", out ResolveTex);

            GL.BindTexture(TextureTarget.Texture2D, ResolveTex);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                (int)Size.X, (int)Size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ResolveGLObject);
            GL.FramebufferParameter(FramebufferTarget.Framebuffer, FramebufferDefaultParameter.FramebufferDefaultSamples, 1);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, ResolveTex, 0);
        }

        public void Use()
        {
            Viewport.Size = Size;

            if (UseAntialiasing)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLObject);

                GL.BindTexture(TextureTarget.Texture2DMultisample, GLTextureObject);
                GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, OptionsWindow.AASamples,
                    PixelInternalFormat.Rgb,
                    (int)Size.X, (int)Size.Y, true);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2DMultisample, GLTextureObject, 0);

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, GLRenderBufferObject);

                if (SizeLastUse != Size || samplesLastUse != OptionsWindow.AASamples || lastUse != UseAntialiasing)
                {
                    GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer,
                        OptionsWindow.AASamples, RenderbufferStorage.Depth24Stencil8,
                        (int)Size.X, (int)Size.Y);
                }
            }
            else
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, NoAA_GLObject);

                GL.BindTexture(TextureTarget.Texture2D, NoAA_GLTexture);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                    (int)Size.X, (int)Size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D, NoAA_GLTexture, 0);

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, NoAA_GLRBO);

                if (SizeLastUse != Size || lastUse != UseAntialiasing)
                {
                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,
                        RenderbufferStorage.Depth24Stencil8,
                        (int)Size.X, (int)Size.Y);
                }
            }

            GL.BindTexture(TextureTarget.Texture2D, ResolveTex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                (int)Size.X, (int)Size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            SizeLastUse = Size;
            samplesLastUse = OptionsWindow.AASamples;
            lastUse = UseAntialiasing;
        }

        public void AfterFrame()
        {
            if (UseAntialiasing)
            {
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, GLObject);
            } else
            {
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, NoAA_GLObject);
            }

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, ResolveGLObject);

            // Blit
            GL.BlitFramebuffer(0, 0, Viewport.Width, Viewport.Height, 0, 0, Viewport.Width, Viewport.Height,
                ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
        }

        public static void Reset()
        {
            Viewport.Size = new(Engine.Width, Engine.Height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
