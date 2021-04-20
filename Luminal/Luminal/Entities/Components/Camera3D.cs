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
            var q = Parent.Quat;
            Matrix4.CreateFromQuaternion(in q, out Matrix4 rotate);
            return Matrix4.CreateTranslation(Parent.Position.ToOpenTK()) * rotate;
        }

        public Matrix4 Projection()
        {
            float aspect = (float)Viewport.Width / Viewport.Height;
            if (float.IsInfinity(aspect) || aspect < 0)
            {
                aspect = 1f;
            }
            return Matrix4.CreatePerspectiveFieldOfView(GLHelper.DegRad(FieldOfView), aspect, Near, Far);
        }
    }
}