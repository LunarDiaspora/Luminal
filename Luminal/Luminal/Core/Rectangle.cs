using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
