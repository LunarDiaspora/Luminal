using System.Numerics;
using Luminal.OpenGL;
using static OpenTK.Graphics.OpenGL.TextureUnit;
using Newtonsoft.Json;
using System;
using Luminal.Core;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Luminal.Entities
{
    public class Material
    {
        public string Name;

        public Vector3 Albedo = new(0.7f, 0.7f, 0.7f);

        public Vector3 Specular = new(1.0f, 1.0f, 1.0f);

        public float Shininess = 64.0f;

        public TextureLike AlbedoMap;

        public TextureLike SpecularMap;

        public Material(string name)
        {
            Name = name;
        }

        public void SerializeInfoToFile(string path)
        {
            using (var tw = File.OpenWrite(path))
            {
                using (var sw = new StreamWriter(tw))
                {
                    var w = new JsonTextWriter(sw);
                    w.WriteStartObject();
                    w.WriteEndObject();
                }
            }
        }

        internal void SetShaderVariables()
        {
            ECSScene.Program.Uniform3("Mat.Albedo", Albedo);
            ECSScene.Program.Uniform3("Mat.Specular", Specular);
            ECSScene.Program.Uniform1("Mat.Shininess", Shininess);
            ECSScene.Program.Uniform1i("Mat.UseAlbedoMap", (AlbedoMap != null) ? 1 : 0);
            ECSScene.Program.Uniform1i("Mat.UseSpecularMap", (SpecularMap != null) ? 1 : 0);

            ECSScene.Program.Uniform1i("Mat.Albedo_Map", 0);
            ECSScene.Program.Uniform1i("Mat.Specular_Map", 1);

            if (AlbedoMap != null)
            {
                GL.ActiveTexture(Texture0);
                AlbedoMap.Bind();
            }

            if (SpecularMap != null)
            {
                GL.ActiveTexture(Texture1);
                SpecularMap.Bind();
            }
        }
    }
}
