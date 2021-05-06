using Luminal.Core;

/*
 * Render.cs - helper API for drawing things
 * (c) Lunar Diaspora 2021
 */

namespace Luminal.Graphics
{
    public enum RenderMode
    {
        Stroke,
        Fill
    }

    public static class Render
    {
        public static void Rectangle(float x, float y, float w, float h, RenderMode mode = RenderMode.Stroke)
        {
            if (!Engine.WindowOpen) return; // Let's just not, ok

            switch (mode)
            {
                case RenderMode.Fill:
                    SDL_GPU.GPU_RectangleFilled(Engine.Screen, x, y, x + w, y + h, Context.Colour);
                    break;

                case RenderMode.Stroke:
                    SDL_GPU.GPU_Rectangle(Engine.Screen, x, y, x + w, y + h, Context.Colour);
                    break;
            }
        }

        public static void Line(float x, float y, float ex, float ey)
        {
            // This is to prevent weird things happening because it WILL crash if it tries to draw to a null pointer
            if (!Engine.WindowOpen) return;

            SDL_GPU.GPU_Line(Engine.Screen, x, y, ex, ey, Context.Colour);
        }

        public static void Point(float x, float y)
        {
            if (!Engine.WindowOpen) return;

            SDL_GPU.GPU_Pixel(Engine.Screen, x, y, Context.Colour);
        }

        // Type overrides (some code still uses int)
        public static void Rectangle(int x, int y, int w, int h, RenderMode mode = RenderMode.Stroke)
        {
            Rectangle((float)x, (float)y, (float)w, (float)h, mode); // This overflows the stack if I don't manually tell it which one to use.
        }

        public static void Line(int x, int y, int ex, int ey)
        {
            Line((float)x, y, ex, ey);
        }

        public static void Point(int x, int y)
        {
            Point((float)x, y);
        }
    }
}