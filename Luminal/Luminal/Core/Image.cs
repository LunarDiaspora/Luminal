using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Graphics;
using SDL2;

namespace Luminal.Core
{
    public class Image
    {
        // The SDL2 texture of the object
        public IntPtr Texture;

        public static bool LoadFrom(string Path, out Image img)
        {
            var p = Path;
            // SDL_Surface* IMG_Load(const char* path);
            var sur = SDL_image.IMG_Load(p);
            var tex = SDL_GPU.GPU_CopyImageFromSurface(sur);
            // I'll free the surface right here and see if weirdness happens
            SDL.SDL_FreeSurface(sur);

            img = new Image()
            {
                Texture = tex
            };
            return true;
        }
        
        public void Draw(float x = 0, float y = 0)
        {
            if (!Engine.WindowOpen) return;

            var sourceRect = new GPU_Rect()
            {
                x = 0,
                y = 0
            };

            SDL.SDL_QueryTexture(Texture, out _, out _, out int tw, out int th);

            sourceRect.w = tw;
            sourceRect.h = th;

            SDL_GPU.GPU_Blit(Texture, ref sourceRect, Engine.Screen, x, y);
        }

        public void Destroy()
        {
            SDL_GPU.GPU_FreeImage(Texture);
        }
    }
}
