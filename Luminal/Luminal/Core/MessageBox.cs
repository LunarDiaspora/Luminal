using SDL2;

namespace Luminal.Core
{
    public static class MessageBox
    {
        public static void Info(string text, string title)
        {
            var parent = Engine.Window;
            SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION, title, text, parent);
        }

        public static void Warning(string text, string title)
        {
            var parent = Engine.Window;
            SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING, title, text, parent);
        }

        public static void Error(string text, string title)
        {
            var parent = Engine.Window;
            SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR, title, text, parent);
        }
    }
}