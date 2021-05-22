using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Core
{
    public static class MatrixUtils
    {
        public static Vector3 Unproject(Vector3 pos, Vector2 mousepos, Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            var finalMatrix = (viewMatrix * projectionMatrix).Inverted();

            var vec = new Vector4(pos, 1f);
            vec.X = (vec.X - mousepos.X) / Viewport.Width;
            vec.Y = (vec.Y - mousepos.Y) / Viewport.Height;
            vec = (vec * 2) - new Vector4(1);
            vec.Y = -vec.Y;

            var outVec = finalMatrix * vec;
            outVec.X /= outVec.W;
            outVec.Y /= outVec.W;
            outVec.Z /= outVec.W;

            return outVec.Xyz;
        }
    }
}
