using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Luminal.Logging;

namespace Luminal.OpenGL
{
    public class GLShaderProgram
    {
        public int GLObject;

        public GLShaderProgram()
        {
            GLObject = GL.CreateProgram();
        }

        public GLShaderProgram Attach(GLShader shader)
        {
            GL.AttachShader(GLObject, shader.GLObject);
            return this;
        }

        public GLShaderProgram Link()
        {
            GL.LinkProgram(GLObject); // ACTUALLY call your functions, guys.

            GL.GetProgram(GLObject, GetProgramParameterName.LinkStatus, out int ok);

            if (ok != 1)
            {
                Log.Fatal("Error during shader linking.");
                var err = GL.GetProgramInfoLog(GLObject);
                Log.Fatal(err.Trim());
                throw new Exception("Error during shader linking.");
            }

            return this;
        }

        public void Use()
        {
            GL.UseProgram(GLObject);
        }
    }
}
