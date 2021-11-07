using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Luminal.Graphics.GLObjects
{
    public unsafe class GLBuffer<T> where T : struct
    {
        public int Reference;

        public GLBuffer()
        {
            Reference = GL.GenBuffer();
        }

        public void Bind(BufferTarget where)
        {
            GL.BindBuffer(where, Reference);
        }

        public void Buffer(BufferTarget where, T[] data,
            BufferUsageHint hint = BufferUsageHint.StreamDraw)
        {
            Bind(where);
            var totalBytes = Marshal.SizeOf<T>() * data.Length;
            GL.BufferData(where, totalBytes, data, hint);
        }
    }
}
