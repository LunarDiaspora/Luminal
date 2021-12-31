using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Luminal.Core
{
    internal static class VertexExtensions
    {
        public static Vector3 ToSysNum(this Assimp.Vector3D vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }
    }
}
