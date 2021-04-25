using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace Luminal.OpenGL
{
    public enum CubemapFace
    {
        FRONT,
        BACK,
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }

    public class GLCubemap
    {
        public int GLObject;

        public static Dictionary<CubemapFace, TextureTarget> TextureTargetMapping = new()
        {
            { CubemapFace.RIGHT, TextureTarget.TextureCubeMapPositiveX },
            { CubemapFace.LEFT, TextureTarget.TextureCubeMapNegativeX },
            { CubemapFace.TOP, TextureTarget.TextureCubeMapPositiveY },
            { CubemapFace.BOTTOM, TextureTarget.TextureCubeMapNegativeY },
            { CubemapFace.FRONT, TextureTarget.TextureCubeMapPositiveZ },
            { CubemapFace.BACK, TextureTarget.TextureCubeMapNegativeZ }
        };

        public GLCubemap(string root, string name)
        {
            var dir = Path.GetDirectoryName(root);
            var ext = Path.GetExtension(root);
            var noExt = Path.GetFileNameWithoutExtension(root);

            var inp = new Dictionary<CubemapFace, string>();

            var d = new Dictionary<CubemapFace, string>()
            {
                { CubemapFace.RIGHT, "-right" },
                { CubemapFace.LEFT, "-left" },
                { CubemapFace.TOP, "-top" },
                { CubemapFace.BOTTOM, "-bottom" },
                { CubemapFace.FRONT, "-front" },
                { CubemapFace.BACK, "-back" }
            };

            foreach (var (key, value) in d)
            {
                inp[key] = Path.Combine(dir, $"{noExt}{value}{ext}");
            }

            Load(inp, name);
        }

        public GLCubemap(Dictionary<CubemapFace, string> paths, string name)
        {
            Load(paths, name);
        }

        private void Load(Dictionary<CubemapFace, string> paths, string name)
        {
            GLObject = GL.GenTexture();
            GLHelper.LabelObj(ObjectLabelIdentifier.Texture, GLObject, $"Luminal Cubemap: {name}");

            Bind();

            foreach (var (face, path) in paths)
            {
                var bmp = new Bitmap(path);

                var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                var data = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                var tgt = TextureTargetMapping[face];
                GL.TexImage2D(tgt, 0, PixelInternalFormat.Rgb, bmp.Width, bmp.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, data.Scan0);

                bmp.UnlockBits(data);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.TextureCubeMap, GLObject);
        }
    }
}
