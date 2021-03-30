using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Luminal.OpenGL
{
    public class GLTexture : IDisposable
    {
        public string Name;
        public int Width;
        public int Height;

        public int GLObject;

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

        public void Dispose()
        {
            GL.DeleteTexture(GLObject);
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
