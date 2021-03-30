using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Graphics;
using SDL2;

namespace Luminal.Core
{
    public class Rectangle
    {
        public float x = 0;
        public float y = 0;
        public float w = 0;
        public float h = 0;

        public static explicit operator SDL.SDL_Rect(Rectangle or)
        {
            var r = new SDL.SDL_Rect()
            {
                x = (int)or.x,
                y = (int)or.y,
                w = (int)or.w,
                h = (int)or.h
            };
            return r;
        }

        public static explicit operator GPU_Rect(Rectangle or)
        {
            var r = new GPU_Rect()
            {
                x = or.x,
                y = or.y,
                w = or.w,
                h = or.h
            };
            return r;
        }
    }
}
