using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Core;
using OpenTK.Graphics.OpenGL;
using System.Numerics;

namespace Luminal.OpenGL
{
    public class GLRenderTexture
    {
        public int GLObject;
        public int GLTextureObject;
        public int GLRenderBufferObject;

        public Vector2 Size = new(Engine.Width, Engine.Height);

        private Vector2 SizeLastUse = new(Engine.Width, Engine.Height);

        public GLRenderTexture()
        {
            GL.CreateFramebuffers(1, out GLObject);
            GLHelper.LabelObj(ObjectLabelIdentifier.Framebuffer, GLObject, "Luminal RenderTexture FBO");

            GLHelper.Texture(TextureTarget.Texture2D, "Luminal RenderTexture", out GLTextureObject);

            GL.BindTexture(TextureTarget.Texture2D, GLTextureObject);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                (int)Size.X, (int)Size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLObject);
            GL.FramebufferParameter(FramebufferTarget.Framebuffer, FramebufferDefaultParameter.FramebufferDefaultSamples, 1);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, GLTextureObject, 0);

            GLRenderBufferObject = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, GLRenderBufferObject);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8,
                (int)Size.X, (int)Size.Y);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, GLRenderBufferObject);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Use()
        {
            Viewport.Size = Size;

            GL.BindTexture(TextureTarget.Texture2D, GLTextureObject);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                (int)Size.X, (int)Size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, GLRenderBufferObject);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLObject);

            if (SizeLastUse != Size)
            {
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8,
                    (int)Size.X, (int)Size.Y);
            }

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, GLRenderBufferObject);

            SizeLastUse = Size;
        }

        public static void Reset()
        {
            Viewport.Size = new(Engine.Width, Engine.Height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
