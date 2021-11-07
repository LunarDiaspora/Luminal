using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Logging;
using OpenTK.Graphics.OpenGL;

namespace Luminal.Graphics.GLObjects
{
    public class GLShaderProgram
    {
        public int Reference;

        public GLShaderProgram()
        {
            Reference = GL.CreateProgram();
        }

        public GLShaderProgram Attach(GLShader sh)
        {
            GL.AttachShader(Reference, sh.Reference);
            return this;
        }

        public GLShaderProgram Link()
        {
            GL.LinkProgram(Reference);

            GL.GetProgram(Reference, GetProgramParameterName.LinkStatus, out int ok);
            if (ok != 1)
            {
                var log = GL.GetProgramInfoLog(Reference);

                Log.Critical("Shader program failed to link!");
                Log.Critical(log.Trim());

                throw new Exception("Shader program failed to link.");
            }

            return this;
        }
    }
}
