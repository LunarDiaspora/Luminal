using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SDL2;

namespace Luminal.Graphics
{
    public class SDLBindingContext : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
            => SDL.SDL_GL_GetProcAddress(procName);
    }
}
