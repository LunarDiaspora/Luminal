using Luminal.Logging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

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

        public int UniformLocation(string uni)
        {
            return GL.GetUniformLocation(GLObject, uni);
        }

        public void Uniform1(string uni, float x)
        {
            var loc = UniformLocation(uni);
            GL.Uniform1(loc, x);
        }

        public void Uniform2(string uni, float x, float y)
        {
            var loc = UniformLocation(uni);
            GL.Uniform2(loc, x, y);
        }

        public void Uniform3(string uni, float x, float y, float z)
        {
            var loc = UniformLocation(uni);
            GL.Uniform3(loc, x, y, z);
        }

        public void Uniform4(string uni, float x, float y, float z, float w)
        {
            var loc = UniformLocation(uni);
            GL.Uniform4(loc, x, y, z, w);
        }

        public void UniformMatrix4(string uni, ref Matrix4 mat)
        {
            var loc = UniformLocation(uni);
            GL.UniformMatrix4(loc, false, ref mat);
        }
    }
}