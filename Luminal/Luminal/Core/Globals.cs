using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Luminal.Core
{
    public class Globals
    {
        public static Dictionary<string, Font> Fonts = new();

        public static void LoadFont(string fontName, string fontPath, int fontSize = 12)
        {
            Font f = new Font(fontPath, fontSize);
            Fonts.Add(fontName, f);
        }
    }
}
