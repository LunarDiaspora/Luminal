namespace Luminal.OpenGL
{
    public class Vec4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vec4(float a = 0, float b = 0, float c = 0, float d = 0)
        {
            x = a;
            y = b;
            z = c;
            w = d;
        }

        public static readonly Vec4 Zero = new Vec4();
    }
}