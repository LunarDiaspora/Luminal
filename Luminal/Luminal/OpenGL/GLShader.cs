using Luminal.Logging;
using OpenTK.Graphics.OpenGL;
using System;

namespace Luminal.OpenGL
{
    public enum GLShaderType
    {
        FRAGMENT,
        VERTEX
    }

    public class GLShader
    {
        public string SourceCode;
        public GLShaderType Type;
        private ShaderType TKType;

        public int GLObject;

        public bool Compiled = false;

        public GLShader(string code, GLShaderType type = GLShaderType.FRAGMENT, bool compileOnCreate = true)
        {
            SourceCode = code;
            Type = type;

            switch (type)
            {
                case GLShaderType.FRAGMENT:
                    TKType = ShaderType.FragmentShader;
                    break;

                case GLShaderType.VERTEX:
                    TKType = ShaderType.VertexShader;
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
                Log.Fatal($"Error while compiling {(Type == GLShaderType.VERTEX ? "vertex" : "fragment")} shader!");
                Log.Fatal(outputLog.Trim());
                throw new Exception("Shader compilation failure.");
            }
        }
    }
}