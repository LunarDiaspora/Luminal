using Luminal.Graphics;
using SDL2;
using System;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Luminal.Core
{
    public class Image
    {
        // The SDL2 texture of the object
        public IntPtr Texture;

        public float Width;
        public float Height;

        public static bool LoadFrom(string Path, out Image img)
        {
            var p = Path;
            // SDL_Surface* IMG_Load(const char* path);
            var sur = SDL_image.IMG_Load(p);
            var raw_sur = Marshal.PtrToStructure<SDL_Surface>(sur);

            var tex = SDL_GPU.GPU_CopyImageFromSurface(sur);
            // I'll free the surface right here and see if weirdness happens
            SDL_FreeSurface(sur);

            img = new Image()
            {
                Texture = tex,
                Width = raw_sur.w,
                Height = raw_sur.h
            };
            return true;
        }

        public void Draw(float x = 0, float y = 0)
        {
            if (!Engine.WindowOpen) return;

            var sourceRect = new GPU_Rect()
            {
                x = 0,
                y = 0,
                w = Width,
                h = Height
            };

            //SDL_QueryTexture(Texture, out _, out _, out int tw, out int th);

            SDL_GPU.GPU_Blit(Texture, ref sourceRect, Engine.Screen, x, y);
        }

        public void Destroy()
        {
            SDL_GPU.GPU_FreeImage(Texture);
        }
    }
}