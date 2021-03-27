using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var tex = SDL.SDL_CreateTextureFromSurface(Engine.Renderer, sur);
            SDL.SDL_FreeSurface(sur);

            img = new MultiplexedImage()
            {
                Texture = tex
            };

            return true;
        }

        public void Draw(string Index, int x = 0, int y = 0)
        {
            if (!Engine.WindowOpen) return;
            var r = SpriteMap[Index];

            var destinationRect = new SDL.SDL_Rect()
            {
                x = x,
                y = y,
                w = (int)r.w,
                h = (int)r.h
            };

            var t = (SDL.SDL_Rect)r;

            SDL.SDL_RenderCopy(Engine.Renderer, Texture, ref t, ref destinationRect);
        }
    }
}
