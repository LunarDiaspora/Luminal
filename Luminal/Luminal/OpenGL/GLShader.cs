using Luminal.Logging;
using OpenTK.Graphics.OpenGL;
using System;

namespace Luminal.OpenGL
{
    public enum GLShaderType
    {
        Fragment,
        Vertex,
        Geometry
    }

    public class GLShader
    {
        public string SourceCode;
        public GLShaderType Type;
        private ShaderType TKType;

        public int GLObject;

        public bool Compiled = false;

        public GLShader(string code, GLShaderType type = GLShaderType.Fragment, bool compileOnCreate = true)
        {
            SourceCode = code;
            Type = type;

            switch (type)
            {
                case GLShaderType.Fragment:
                    TKType = ShaderType.FragmentShader;
                    break;

                case GLShaderType.Vertex:
                    TKType = ShaderType.VertexShader;
                    break;

                case GLShaderType.Geometry:
                    TKType = ShaderType.GeometryShader;
                    break;
            }

            GLObject = GL.CreateShader(TKType);

            GL.ShaderSource(GLObject, code);

            if (compileOnCreate) Compile();
        }

        public void Compile()
        {
            if (Compiled) return;
            Compiled = true;

            GL.CompileShader(GLObject);

            GL.GetShader(GLObject, ShaderParameter.CompileStatus, out int ok);
            if (ok != 1)
            {
                // Something's gone wrong.
                var outputLog = GL.GetShaderInfoLog(GLObject);

                var shaderType = Type switch
                {
                    GLShaderType.Vertex => "vertex",
                    GLShaderType.Fragment => "fragment",
                    GLShaderType.Geometry => "geometry",
                    _ => "(what the fuck?)"
                };

                Log.Fatal($"Error while compiling {shaderType} shader!");
                Log.Fatal(outputLog.Trim());
                throw new Exception("Shader compilation failure.");
            }
        }
    }
}