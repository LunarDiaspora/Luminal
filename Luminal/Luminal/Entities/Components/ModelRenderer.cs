using Luminal.OpenGL.Models;
using OpenTK.Mathematics;
using Luminal.OpenGL;

namespace Luminal.Entities.Components
{
    public class ModelRenderer : Component3D
    {
        public Model Model;
        public Material Material = new();

        public override void Render3D()
        {
            var model = Matrix4.CreateTranslation(Parent.Position.ToOpenTK());
            model *= Matrix4.CreateFromQuaternion(Parent.Quat);
            ECSScene.Program.UniformMatrix4("Model", ref model);

            Material.SetShaderVariables();

            Model.Draw();
        }
    }
}