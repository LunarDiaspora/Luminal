using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Core;
using OpenTK.Mathematics;

namespace Luminal.OpenGL
{
    public class Camera : SceneObject
    {
        public float FieldOfView = 90.0f;
        
        public Camera(Vector3 position, Vector3 angle)
        {
            Position = position;
            Quat = Quaternion.FromEulerAngles(angle);
        }

        public Matrix4 View()
        {
            Matrix4.CreateFromQuaternion(in Quat, out Matrix4 rotate);
            return Matrix4.CreateTranslation(Position) * rotate;
        }

        public Matrix4 Projection()
        {
            return Matrix4.CreatePerspectiveFieldOfView(GLHelper.DegRad(FieldOfView), Engine.Width / Engine.Height, 0.1f, 1000f);
        }
    }
}
