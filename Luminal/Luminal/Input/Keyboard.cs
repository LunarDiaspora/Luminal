using Luminal.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace Luminal.Input
{
    public class Keyboard
    {
        internal static Dictionary<SDL_Scancode, bool> PressedMap = new();
        internal static Dictionary<SDL_Scancode, bool> DownThisFrame = new();
        internal static Dictionary<SDL_Scancode, bool> UpThisFrame = new();

        public static bool Down(string key)
        {
            var t = Engine.StringToScancode(key);
            return Down(t);
        }

        public static bool Down(SDL_Scancode down)
        {
            return PressedMap[down];
        }

        public static bool Pressed(string key)
        {
            var t = Engine.StringToScancode(key);
            return Pressed(t);
        }

        public static bool Pressed(SDL_Scancode k)
        {
            return DownThisFrame[k];
        }

        public static bool Released(string key)
        {
            var t = Engine.StringToScancode(key);
            return Released(t);
        }

        public static bool Released(SDL_Scancode k)
        {
            return UpThisFrame[k];
        }

        public static bool Changed(string key)
        {
            var t = Engine.StringToScancode(key);
            return Changed(t);
        }

        public static bool Changed(SDL_Scancode k)
        {
            return DownThisFrame[k] || UpThisFrame[k];
        }

        internal static void Initialise()
        {
            foreach (SDL_Scancode t in Enum.GetValues(typeof(SDL_Scancode)))
            {
                PressedMap[t] = false;
                DownThisFrame[t] = false;
                UpThisFrame[t] = false;
            }
        }

        internal static void Update()
        {
            foreach (var (k, _) in DownThisFrame)
            {
                DownThisFrame[k] = false;
                UpThisFrame[k] = false;
            }
        }
    }
}
