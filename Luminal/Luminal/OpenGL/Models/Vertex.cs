using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.OpenGL.Models
{
    /*
     * Each VectorX contains X many floats.
     * 
     * 3 + 3 + 2 + 4 = 12, so there are 8 floats. Therefore the stride is (how big a float is) x 8.
     */

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = sizeof(float) * 12)]
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
        public Vector4 Colour;
    }
}
