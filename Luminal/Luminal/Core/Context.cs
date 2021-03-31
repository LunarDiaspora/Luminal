using SDL2;
using System.Collections.Generic;

namespace Luminal.Core
{
    public class Context
    {
        private static SDL.SDL_Color IntColour;
        public static SDL.SDL_Color Colour { get { return IntColour; } }

        public static SDL.SDL_Color Black = MakeColourFromRGBA(0, 0, 0, 255);
        public static SDL.SDL_Color TransparentBlack = MakeColourFromRGBA(0, 0, 0, 0);

        public static string CurrentFont = "monospace";

        public static Dictionary<string, Image> Images = new();

        public static SDL.SDL_Color MakeColourFromRGBA(byte r, byte g, byte b, byte a = 255)
        {
            return new SDL.SDL_Color
            {
                r = r,
                g = g,
                b = b,
                a = a
            };
        }

        public static bool LoadImage(string Name, string Path)
        {
            var ok = Image.LoadFrom(Path, out Image n);
            if (ok) Images[Name] = n;
            return ok;
        }

        public static void FreeAllImages()
        {
            foreach (var (_, value) in Images)
            {
                value.Destroy();
            }
        }

        public static void SetColour(byte r, byte g, byte b, byte a = 255)
        {
            IntColour = MakeColourFromRGBA(r, g, b, a);
            SDL.SDL_SetRenderDrawColor(Engine.Renderer, r, g, b, a);
        }
    }
}