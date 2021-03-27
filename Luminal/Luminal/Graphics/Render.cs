using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using Luminal.Core;

/*
 * Render.cs - helper API for drawing things
 * (c) Lunar Diaspora 2021
 */

namespace Luminal.Graphics
{
    public enum RenderMode
    {
        STROKE,
        FILL
    }

    public static class Render
    {
        public static void Rectangle(int x, int y, int w, int h, RenderMode mode = RenderMode.STROKE)
        {
            if (!Engine.WindowOpen) return; // Let's just not, ok

            SDL.SDL_Rect r = new()
            {
                x = x,
                y = y,
                w = w,
                h = h
            };

            int res = 0;
            switch(mode)
            {
                case RenderMode.FILL:
                    res = SDL.SDL_RenderFillRect(Engine.Renderer, ref r);
                    break;
                case RenderMode.STROKE:
                    res = SDL.SDL_RenderDrawRect(Engine.Renderer, ref r);
                    break;
            }

            if (res != 0)
            {
                // We fucked up
                var error = SDL.SDL_GetError();
                throw new LuminalException($"Render.Rectangle: {error}");
            }
        }

        public static void Line(int x, int y, int ex, int ey)
        {
            // This is to prevent weird things happening because it WILL crash if it tries to draw to a null pointer
            if (!Engine.WindowOpen) return;

            var res = SDL.SDL_RenderDrawLine(Engine.Renderer, x, y, ex, ey);
            if (res != 0)
            {
                // ...oops
                var error = SDL.SDL_GetError();
                throw new LuminalException($"Render.Line: {error}");
            }
        }

        public static void Point(int x, int y)
        {
            if (!Engine.WindowOpen) return;

            var res = SDL.SDL_RenderDrawPoint(Engine.Renderer, x, y);

            if (res != 0)
            {
                var error = SDL.SDL_GetError();
                throw new LuminalException($"Render.Point: {error}");
            }
        }
    }
}
