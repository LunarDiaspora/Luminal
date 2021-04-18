using System.Numerics;

namespace Luminal.Entities
{
    public class Material
    {
        public Vector3 Albedo = new(0.7f, 0.7f, 0.7f);

        public Vector3 Specular = new(1.0f, 1.0f, 1.0f);

        public float Shininess = 64.0f;

        internal void SetShaderVariables()
        {
            ECSScene.Program.Uniform3("Mat.Albedo", Albedo);
            ECSScene.Program.Uniform3("Mat.Specular", Specular);
            ECSScene.Program.Uniform1("Mat.Shininess", Shininess);
        }
    }
}
