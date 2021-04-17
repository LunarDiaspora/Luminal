using System.Numerics;

namespace Luminal.Entities.Components
{
    public class PointLight3D : Component3D
    {
        public Vector3 Colour = new(1.0f, 1.0f, 1.0f);
        public float Intensity = 1.0f;
        public float Shininess = 64.0f;

        public override void Create()
        {
            // We only have one light for now. This is okay.
            ECSScene.Light = this;
        }
    }
}