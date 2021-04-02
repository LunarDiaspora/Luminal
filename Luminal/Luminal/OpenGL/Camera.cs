using Luminal.Core;
using OpenTK.Mathematics;

namespace Luminal.OpenGL
{
    public class Camera : SceneObject
    {
        public float FieldOfView = 90.0f;
        public float Near = 0.1f;
        public float Far = 1000f;

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
            return Matrix4.CreatePerspectiveFieldOfView(GLHelper.DegRad(FieldOfView), (float)Engine.Width / Engine.Height, Near, Far);
        }
    }
}