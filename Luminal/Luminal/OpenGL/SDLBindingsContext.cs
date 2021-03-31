using OpenTK;
using SDL2;
using System;

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