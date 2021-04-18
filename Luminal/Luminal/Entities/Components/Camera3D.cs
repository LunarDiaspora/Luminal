using Luminal.Core;
using Luminal.OpenGL;
using OpenTK.Mathematics;

namespace Luminal.Entities.Components
{
    public class Camera3D : Component3D
    {
        public float FieldOfView = 90.0f;
        public float Near = 0.1f;
        public float Far = 1000f;

        public override void Create()
        {
            // TODO: make this better.
            ECSScene.Camera = this;
        }

        public Matrix4 View()
        {
            Matrix4.CreateFromQuaternion(in Parent.Quat, out Matrix4 rotate);
            return Matrix4.CreateTranslation(Parent.Position.ToOpenTK()) * rotate;
        }

        public Matrix4 Projection()
        {
            return Matrix4.CreatePerspectiveFieldOfView(GLHelper.DegRad(FieldOfView), (float)Engine.Width / Engine.Height, Near, Far);
        }
    }
}