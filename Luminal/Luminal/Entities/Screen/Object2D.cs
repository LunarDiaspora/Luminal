using System.Numerics;

namespace Luminal.Entities.Screen
{
    public class Object2D : BaseObject
    {
        public Vector2 Position = new(0, 0);
        public float Angle = 0.0f; // This is a 2D object. We only have to rotate along Z.
    }
}
