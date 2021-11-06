using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.DebugUI
{
    public static class DebugWindowManager
    {
        public static List<DebugWindow> OpenWindows = new();

        public static T OpenWindow<T>() where T : DebugWindow, new()
        {
            var areThereAny = OpenWindows.Any(e => e.GetType() == typeof(T));
            if (areThereAny)
                return (T)OpenWindows.First(e => e.GetType() == typeof(T)); // Just grab it

            var win = new T();
            OpenWindows.Add(win);
            return win;
        }

        public static void RenderAll()
        {
            foreach (var m in OpenWindows)
                m.Render();

            var stillOpen = OpenWindows.Where(e => e.Open);
            OpenWindows = stillOpen.ToList();
        }
    }
}
