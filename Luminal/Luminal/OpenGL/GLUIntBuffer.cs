using OpenTK.Graphics.OpenGL;

namespace Luminal.OpenGL
{
    public class GLUIntBuffer
    {
        public int GLObject;
        public BufferTarget CurrentTarget;

        public GLUIntBuffer()
        {
            GLObject = GL.GenBuffer();
        }

        public GLUIntBuffer(string name)
        {
            GLObject = GL.GenBuffer();
            GLHelper.LabelObj(ObjectLabelIdentifier.Buffer, GLObject, $"UIntBuffer: {name}");
        }

        public void Bind(BufferTarget t)
        {
            GL.BindBuffer(t, GLObject);
            CurrentTarget = t;
        }

        public void Data(uint[] data, BufferUsageHint h = BufferUsageHint.DynamicDraw)
        {
            GL.BufferData(CurrentTarget, data.Length * sizeof(uint), data, h);
        }
    }
}