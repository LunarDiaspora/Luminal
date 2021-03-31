using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Luminal.OpenGL
{
    public class SceneObject
    {
        public Vector3 Position;

        public Vector3 Euler
        {
            set
            {
                Quat = Quaternion.FromEulerAngles(GLHelper.V3DegRad(value));
            }
            get
            {
                return GLHelper.V3RadDeg(Quat.ToEulerAngles());
            }
        }

        public Quaternion Quat;

        public Vector3 Right
        {
            get
            {
                var rot = Matrix4.CreateFromQuaternion(Quat.Inverted());
                return Vector3.TransformPosition(GLHelper.Right, rot);
            }
        }

        public Vector3 Up
        {
            get
            {
                var rot = Matrix4.CreateFromQuaternion(Quat.Inverted());
                return Vector3.TransformPosition(GLHelper.Up, rot);
            }
        }

        public void RotateAbout(Vector3 axis, float angle)
        {
            var q = Quaternion.FromAxisAngle(axis, angle);
            Quat *= q;
        }

        public void Translate(Vector3 delta)
        {
            Position += delta;
        }
    }
}
