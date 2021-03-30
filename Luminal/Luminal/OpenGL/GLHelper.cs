using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Luminal.OpenGL
{
    public static class GLHelper
    {
        public static void LabelObj(ObjectLabelIdentifier ident, int obj, string name)
        {
            GL.ObjectLabel(ident, obj, name.Length, name);
        }

        public static void Texture(TextureTarget target, string Name, out int Tex)
        {
            GL.CreateTextures(target, 1, out Tex);
            LabelObj(ObjectLabelIdentifier.Texture, Tex, $"Luminal GLTexture: {Name}");
        }
    }
}
