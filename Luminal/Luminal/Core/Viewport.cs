using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Core
{
    public static class Viewport
    {
        public static Vector2 Size = new Vector2(Engine.Width, Engine.Height);
        public static int Width => (int)Size.X;
        public static int Height => (int)Size.Y;
    }
}
