using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2.SDL.SDL_bool;
using Luminal.Core;
using System.Numerics;

namespace Luminal.Input
{
    public enum FullscreenMode
    {
        WINDOWED,
        FULLSCREEN,
        BORDERLESS
    }

    public static class Window
    {
        public static float Opacity
        {
            get
            {
                SDL_GetWindowOpacity(Engine.Window, out float x);
                return x;
            }
            set
            {
                SDL_SetWindowOpacity(Engine.Window, value);
            }
        }

        public static string Title
        {
            get => SDL_GetWindowTitle(Engine.Window);
            set => SDL_SetWindowTitle(Engine.Window, value);
        }

        public static FullscreenMode FullScreen
        {
            get 
            {
                var f = SDL_GetWindowFlags(Engine.Window);

                if ((f & (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) > 0)
                    return FullscreenMode.FULLSCREEN;
                else if ((f & (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP) > 0)
                    return FullscreenMode.BORDERLESS;
                else
                    return FullscreenMode.WINDOWED;
            }

            set
            {
                SDL_SetWindowFullscreen(Engine.Window, value switch
                {
                    FullscreenMode.FULLSCREEN => 1,
                    FullscreenMode.BORDERLESS => 4097,
                    _ => 0
                });
            }
        }

        public static Vector2 Position
        {
            get
            {
                SDL_GetWindowPosition(Engine.Window, out int a, out int b);
                return new(a, b);
            }

            set => SDL_SetWindowPosition(Engine.Window, (int)value.X, (int)value.Y);
        }
    }
}
