using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Luminal.OpenGL
{
    public class GLTexture : IDisposable
    {
        public string Name;
        public int Width;
        public int Height;

        public int GLObject;

        public string Path = null;

        public GLTexture(string name, int w, int h, IntPtr data)
        {
            Name = name;
            Width = w;
            Height = h;

            GLHelper.Texture(TextureTarget.Texture2D, name, out int gt);
            GL.TextureStorage2D(gt, 1, SizedInternalFormat.Rgba16, w, h);

            GL.TextureSubImage2D(gt, 0, 0, 0, w, h, PixelFormat.Bgra, PixelType.UnsignedByte, data);

            GLObject = gt;

            SetMinFilter(TextureMinFilter.Nearest);
            SetMagFilter(TextureMagFilter.Nearest);
        }

        public unsafe GLTexture(string name, SDL2.SDL.SDL_Surface source)
        {
            Name = name;
            Width = source.w;
            Height = source.h;

            GLHelper.Texture(TextureTarget.Texture2D, name, out int gt);
            GL.BindTexture(TextureTarget.Texture2D, gt);

            GL.TextureStorage2D(gt, 1, SizedInternalFormat.Rgba16, Width, Height);

            var fmt = PixelInternalFormat.Rgb;

            var surfmt = (SDL2.SDL.SDL_PixelFormat*)(source.format.ToPointer());

            if (surfmt->BytesPerPixel == 4)
            {
                // We're in RGBA land
                fmt = PixelInternalFormat.Rgba;
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, fmt, Width, Height, 0, (PixelFormat)fmt, PixelType.UnsignedByte, source.pixels);

            GLObject = gt;

            SetMinFilter(TextureMinFilter.Nearest);
            SetMagFilter(TextureMagFilter.Nearest);
        }

        public GLTexture(string name, string file, bool isSRGB = false)
        {
            var bmp = new Bitmap(file);

            GLHelper.Texture(TextureTarget.Texture2D, name, out int obj);
            GLObject = obj;

            Bind();
            var ir = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var data = bmp.LockBits(ir, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var fmt = isSRGB ? PixelInternalFormat.SrgbAlpha : PixelInternalFormat.Rgba;

            GL.TexImage2D(TextureTarget.Texture2D, 0, fmt, bmp.Width, bmp.Height, 0,
                          PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            SetMinFilter(TextureMinFilter.Linear);
            SetMagFilter(TextureMagFilter.Linear);

            SetWrappingRules(TextureWrapMode.Repeat);
        }

        public GLTexture(string name, Bitmap bmp)
        {
            GLHelper.Texture(TextureTarget.Texture2D, name, out int obj);
            GLObject = obj;

            Bind();
            var ir = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var data = bmp.LockBits(ir, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var fmt = PixelInternalFormat.Rgba;

            GL.TexImage2D(TextureTarget.Texture2D, 0, fmt, bmp.Width, bmp.Height, 0,
              PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            SetMinFilter(TextureMinFilter.Linear);
            SetMagFilter(TextureMagFilter.Linear);

            SetWrappingRules(TextureWrapMode.Repeat);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, GLObject);
        }

        public static void Active(TextureUnit tu)
        {
            GL.ActiveTexture(tu);
        }

        public void ActiveBind(TextureUnit tu)
        {
            Active(tu);
            Bind();
        }

        public void Dispose()
        {
            //GL.DeleteTexture(GLObject);
        }

        public void SetWrappingRules(TextureWrapMode mode)
        {
            Bind();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)mode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)mode);
        }

        public void SetMinFilter(TextureMinFilter f)
        {
            GL.TextureParameter(GLObject, TextureParameterName.TextureMinFilter, (int)f);
        }

        public void SetMagFilter(TextureMagFilter f)
        {
            GL.TextureParameter(GLObject, TextureParameterName.TextureMagFilter, (int)f);
        }
    }
}