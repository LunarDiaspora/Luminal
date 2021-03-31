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
                return Quat * GLHelper.Right;
            }
        }

        public Vector3 Up
        {
            get
            {
                return Quat * GLHelper.Up;
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
