using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Luminal.OpenGL
{
    public class GLFloatBuffer
    {
        public int GLObject;
        public BufferTarget CurrentTarget;

        public GLFloatBuffer()
        {
            GLObject = GL.GenBuffer();
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
