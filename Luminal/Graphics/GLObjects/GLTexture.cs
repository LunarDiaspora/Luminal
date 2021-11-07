using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Luminal.Graphics.GLObjects
{
    public class GLTexture
    {
        public int Reference;

        public GLTexture()
        {
            Reference = GL.GenTexture();
        }

        public void Load2DTextureFromFile(string p)
        {
            var bmp = new Bitmap(p);
            var bmpRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bits = bmp.LockBits(
                bmpRect,
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, Reference);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte,
                bits.Scan0);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            bmp.UnlockBits(bits);
        }
    }
}
