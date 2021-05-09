using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Luminal.Reflection;

namespace Luminal.Entities.Components
{
    public class SpotLight3D : Component3D
    {
        [Colour]
        public Vector3 Colour = new(1.0f, 1.0f, 1.0f);

        public float Intensity = 1.0f;

        public float Linear = 0.09f;
        public float Quadratic = 0.032f;

        public float Radius = 30.0f;

        public float Contour = 3.0f;

        public float OuterRadius
        {
            get => Radius - Contour;
        }

        public override void Create()
        {
            ECSScene.CurrentScene.SpotLights.Add(this);
        }

        public override void Destroy()
        {
            ECSScene.CurrentScene.SpotLights.Remove(this);
        }
    }
}