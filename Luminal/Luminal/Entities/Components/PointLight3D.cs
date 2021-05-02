using Luminal.Reflection;
using System.Numerics;

namespace Luminal.Entities.Components
{
    public class PointLight3D : Component3D
    {
        [Colour]
        public Vector3 Colour = new(1.0f, 1.0f, 1.0f);

        public float Intensity = 1.0f;

        public float Linear = 0.09f;
        public float Quadratic = 0.032f;

        public override void Create()
        {
            ECSScene.PointLights.Add(this);
        }

        public override void Destroy()
        {
            ECSScene.PointLights.Remove(this);
        }
    }
}