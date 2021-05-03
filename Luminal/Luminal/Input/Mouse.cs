using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2.SDL.SDL_bool;
using System.Numerics;
using Luminal.Core;

namespace Luminal.Input
{
    public enum MouseButton
    {
        Left,
        Middle,
        Right,
        Thumb1,
        Thumb2
    }

    public class Mouse
    {
        public static Vector2 Position
        {
            get
            {
                SDL_GetMouseState(out int x, out int y);
                return new(x, y);
            }

            set => SetMousePosition(value.X, value.Y);
        }

        public static Vector2 Delta => Engine._DeltaMouse;

        public static bool Locked
        {
            get => SDL_GetRelativeMouseMode() == SDL_TRUE;
            set => SDL_SetRelativeMouseMode(value ? SDL_TRUE : SDL_FALSE);
        }

        public static bool Captured
        {
            get => (SDL_GetWindowFlags(Engine.Window) & (uint)SDL_WindowFlags.SDL_WINDOW_MOUSE_CAPTURE) > 0;
            set => SDL_CaptureMouse(value ? SDL_TRUE : SDL_FALSE);
        }

        public static bool Grabbed
        {
            get => SDL_GetWindowGrab(Engine.Window) == SDL_TRUE;
            set => SDL_SetWindowGrab(Engine.Window, value ? SDL_TRUE : SDL_FALSE);
        }

        public static void SetMousePosition(float x, float y)
        {
            SDL_WarpMouseInWindow(Engine.Window, (int)x, (int)y);
        }

        public static void SetMousePositionGlobally(float x, float y)
        {
            SDL_WarpMouseGlobal((int)x, (int)y);
        }
    }
}
