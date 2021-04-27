using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using System.Windows.Forms;

namespace Luminal.Core
{
    public static class WinAPIUtilities
    {
        public static IntPtr GetHwnd()
        {
            SDL_version h;
            SDL_VERSION(out h);
            SDL_SysWMinfo t = new();
            t.version = h;
            SDL_GetWindowWMInfo(Engine.Window, ref t);
            return t.info.win.window;
        }

        public static Rectangle WorkArea()
        {
            var t = Screen.FromHandle(GetHwnd()).WorkingArea;
            var lr = new Rectangle()
            {
                x = t.X,
                y = t.Y,
                w = t.Width,
                h = t.Height
            };
            Logging.Log.Debug($"{lr.x} {lr.y} - {lr.w} {lr.h}");
            return lr;
        }
    }

}
