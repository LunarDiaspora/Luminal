using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SDL2;

namespace Luminal.OpenGL
{
    public class SDLBindingsContext : IBindingsContext
    {
        public IntPtr GetProcAddress(string h)
        {
            var bptr = SDL.SDL_GL_GetProcAddress(h);
            return bptr;
        }
    }
}
