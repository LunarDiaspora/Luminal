using Luminal.Logging;
using OpenTK.Graphics.OpenGL;

namespace Luminal.OpenGL
{
    public class GLFloatBuffer
    {
        public int GLObject;
        public BufferTarget CurrentTarget;
        public string Name;

        public GLFloatBuffer()
        {
            GLObject = GL.GenBuffer();
        }

        public GLFloatBuffer(string name)
        {
            Name = name;
            GLObject = GL.GenBuffer();
            GLHelper.LabelObj(ObjectLabelIdentifier.Buffer, GLObject, $"FloatBuffer: {name}");
        }

        public void Bind(BufferTarget t)
        {
            GL.BindBuffer(t, GLObject);
            CurrentTarget = t;
        }

        public void Data(float[] data, BufferUsageHint h = BufferUsageHint.DynamicDraw)
        {
            GL.BufferData(CurrentTarget, data.Length * sizeof(float), data, h);
        }
    }
}