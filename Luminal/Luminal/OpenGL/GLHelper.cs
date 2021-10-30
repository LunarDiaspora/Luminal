using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Luminal.OpenGL
{
    public static class GLHelper
    {
        public static Vector3 Up = new(0.0f, 1.0f, 0.0f);
        public static Vector3 Right = new(1.0f, 0.0f, 0.0f);
        public static Vector3 Forward = new(0.0f, 0.0f, 1.0f);
        public static Vector3 Left = -Right;
        public static Vector3 Down = -Up;
        public static Vector3 Back = -Forward;

        public static void LabelObj(ObjectLabelIdentifier ident, int obj, string name)
        {
            //GL.ObjectLabel(ident, obj, name.Length, name);
        }

        public static void Texture(TextureTarget target, string Name, out int Tex)
        {
            GL.CreateTextures(target, 1, out Tex);
            LabelObj(ObjectLabelIdentifier.Texture, Tex, $"Luminal GLTexture: {Name}");
        }

        public static void Buffer(string Name, out int Buf)
        {
            GL.CreateBuffers(1, out Buf);
            LabelObj(ObjectLabelIdentifier.Buffer, Buf, $"Buffer: {Name}");
        }

        public static void VertexBuffer(string Name, out int Buf) => Buffer($"VBO: {Name}", out Buf);

        public static void ElementBuffer(string Name, out int Buf) => Buffer($"EBO: {Name}", out Buf);

        public static float DegRad(float deg)
        {
            return deg * (float)(Math.PI / 180);
        }

        public static float RadDeg(float rad)
        {
            return rad * (float)(180 / Math.PI);
        }

        public static Vector3 V3DegRad(Vector3 v)
        {
            return new Vector3(DegRad(v.X), DegRad(v.Y), DegRad(v.Z));
        }

        public static Vector3 V3RadDeg(Vector3 v)
        {
            return new Vector3(RadDeg(v.X), RadDeg(v.Y), RadDeg(v.Z));
        }
    }
}