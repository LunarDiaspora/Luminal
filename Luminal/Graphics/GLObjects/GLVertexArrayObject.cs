using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Graphics.GLObjects
{
    public class GLVertexArrayObject
    {
        public int Reference;

        public GLVertexArrayObject()
        {
            Reference = GL.GenVertexArray();
        }

        public void Bind()
        {
            GL.BindVertexArray(Reference);
        }
    }
}
