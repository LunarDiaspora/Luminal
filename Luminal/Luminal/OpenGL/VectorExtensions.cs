namespace Luminal.OpenGL
{
    public static class VectorExtensions
    {
        public static System.Numerics.Vector3 ToSystemNumerics(this OpenTK.Mathematics.Vector3 vec)
            => new(vec.X, vec.Y, vec.Z);

        public static OpenTK.Mathematics.Vector3 ToOpenTK(this System.Numerics.Vector3 vec)
            => new(vec.X, vec.Y, vec.Z);
    }
}
