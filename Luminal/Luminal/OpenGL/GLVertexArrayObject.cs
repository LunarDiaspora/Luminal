using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Luminal.OpenGL
{
    public class GLVertexArrayObject
    {
        public int GLObject;

        public GLVertexArrayObject()
        {
            GLObject = GL.GenVertexArray();
        }

        public void Bind()
        {
            GL.BindVertexArray(GLObject);
        }
    }
}
