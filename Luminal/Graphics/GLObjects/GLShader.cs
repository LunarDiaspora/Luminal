using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Logging;
using OpenTK.Graphics.OpenGL;

namespace Luminal.Graphics.GLObjects
{
    public enum GLShaderType
    {
        Vertex,
        Fragment,
        Geometry
    }

    public class GLShader
    {
        public int Reference;
        public GLShaderType Type;

        public GLShader(GLShaderType type)
        {
            Type = type;

            var tkType = type switch
            {
                GLShaderType.Vertex => ShaderType.VertexShader,
                GLShaderType.Fragment => ShaderType.FragmentShader,
                GLShaderType.Geometry => ShaderType.GeometryShader,
                _ => throw new Exception($"Unknown shader type {type}")
            };
            Reference = GL.CreateShader(tkType);
        }

        public void Compile(string source)
        {
            GL.ShaderSource(Reference, source);

            GL.CompileShader(Reference);

            GL.GetShader(Reference, ShaderParameter.CompileStatus, out int ok);
            if (ok != 1)
            {
                var log = GL.GetShaderInfoLog(Reference);

                Log.Critical($"{Type} shader compile error!");
                Log.Critical(log.Trim());

                throw new Exception("Shader compile error.");
            }
        }
    }
}
