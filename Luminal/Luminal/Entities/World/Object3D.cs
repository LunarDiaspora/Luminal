using Luminal.OpenGL;
using OpenTK.Mathematics;
using Vector3 = System.Numerics.Vector3;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace Luminal.Entities.World
{
    public class Object3D : BaseObject
    {
        public Vector3 Position;

        public Vector3 Euler
        {
            set
            {
                Quat = Quaternion.FromEulerAngles(GLHelper.V3DegRad(value.ToOpenTK()));
            }
            get
            {
                return GLHelper.V3RadDeg(Quat.ToEulerAngles()).ToSystemNumerics();
            }
        }

        public Quaternion Quat = Quaternion.Identity;

        private Vector3 RotateVector(OpenTK.Mathematics.Vector3 axis)
        {
            var rot = Matrix4.CreateFromQuaternion(Quat.Inverted());
            var tk = -OpenTK.Mathematics.Vector3.TransformPosition(axis, rot);
            return tk.ToSystemNumerics();
        }

        public Vector3 Right
        {
            get
            {
                return RotateVector(GLHelper.Right);
            }
        }

        public Vector3 Up
        {
            get
            {
                return RotateVector(GLHelper.Up);
            }
        }

        public Vector3 Forward
        {
            get
            {
                return -RotateVector(GLHelper.Forward);
            }
        }

        public void RotateAbout(Vector3 axis, float angle)
        {
            var q = Quaternion.FromAxisAngle(axis.ToOpenTK(), angle);
            Quat *= q;
        }

        public void Rotate(Vector3 eulers)
        {
            var eul = GLHelper.V3DegRad(eulers.ToOpenTK());
            var qx = Quaternion.FromEulerAngles(eul.X, 0f, 0f);
            var qy = Quaternion.FromEulerAngles(0f, eul.Y, 0f);
            var qz = Quaternion.FromEulerAngles(0f, 0f, eul.Z);

            Quat *= qz;
            Quat *= qy;
            Quat *= qx;
        }

        public void Translate(Vector3 delta)
        {
            Position += delta;
        }
    }
}