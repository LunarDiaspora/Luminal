using Luminal.Assets.Complex;
using Luminal.Graphics.GLObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Luminal.AssetTypes
{
    [ComplexType("Shader")]
    public class Shader : ComplexAsset
    {
        struct MetaJson
        {
            public string Vertex { get; set; }
            public string Fragment { get; set; }
        }

        static string HashString(string a)
        {
            var sha = SHA256.Create();
            var enc = Encoding.UTF8.GetBytes(a);
            var hash = sha.ComputeHash(enc);
            sha.Dispose();
            return string.Concat(hash.Select(e => e.ToString("x2")));
        }

        static Dictionary<string, GLShader> ShaderCache = new();

        public GLShaderProgram Program;

        static GLShader CacheShader(string shader, GLShaderType type)
        {
            var hash = HashString(shader);
            if (ShaderCache.ContainsKey(hash)) return ShaderCache[hash];
            var shObj = new GLShader(type);
            shObj.Compile(shader);
            return shObj;
        }

        public override void OnConstruct(string rootPath, string metaFileContents)
        {
            var meta = JsonSerializer.Deserialize<MetaJson>(metaFileContents);
            var vertText = File.ReadAllText(Path.Combine(rootPath, meta.Vertex));
            var fragText = File.ReadAllText(Path.Combine(rootPath, meta.Fragment));
            var vert = CacheShader(vertText, GLShaderType.Vertex);
            var frag = CacheShader(fragText, GLShaderType.Fragment);
            Program = new();
            Program = Program.Attach(vert).Attach(frag).Link();
        }
    }
}
