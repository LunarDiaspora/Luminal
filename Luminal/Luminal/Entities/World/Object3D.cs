using Luminal.OpenGL;
using OpenTK.Mathematics;
using Vector3 = System.Numerics.Vector3;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace Luminal.Entities.World
{
    public class Object3D : BaseObject
    {
        public Vector3 Position;

        private Vector3 _Euler;
        private Quaternion _Quat = Quaternion.Identity;

        public Vector3 Euler
        {
            set
            {
                _Euler = GLHelper.V3DegRad(value.ToOpenTK()).ToSystemNumerics();
                var q = Quaternion.Identity;
                q *= Quaternion.FromAxisAngle(Vector3.UnitX.ToOpenTK(), _Euler.X);
                q *= Quaternion.FromAxisAngle(Vector3.UnitY.ToOpenTK(), _Euler.Y);
                q *= Quaternion.FromAxisAngle(Vector3.UnitZ.ToOpenTK(), _Euler.Z);
                _Quat = q;
            }
            get
            {
                return GLHelper.V3RadDeg(_Euler.ToOpenTK()).ToSystemNumerics();
            }
        }

        public Quaternion Quat
        {
            set
            {
                _Quat = value;
                Quaternion.ToEulerAngles(in value, out OpenTK.Mathematics.Vector3 t);
                _Euler = t.ToSystemNumerics();
            }
            get
            {
                return _Quat;
            }
        }

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