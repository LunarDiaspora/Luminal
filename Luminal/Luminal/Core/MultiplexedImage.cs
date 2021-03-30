using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Graphics;
using SDL2;

namespace Luminal.Core
{
    public class MultiplexedImage
    {
        public IntPtr Texture;
        public Dictionary<string, Rectangle> SpriteMap;

        public static bool LoadFrom(string path, out MultiplexedImage img)
        {
            var sur = SDL_image.IMG_Load(path);
            var tex = SDL_GPU.GPU_CopyImageFromSurface(sur);
            SDL.SDL_FreeSurface(sur);

            img = new MultiplexedImage()
            {
                Texture = tex
            };

            return true;
        }

        public void Draw(string Index, float x = 0, float y = 0)
        {
            if (!Engine.WindowOpen) return;
            var r = SpriteMap[Index];

            var t = (GPU_Rect)r;

            SDL_GPU.GPU_Blit(Texture, ref t, Engine.Screen, x, y);
        }
    }
}
