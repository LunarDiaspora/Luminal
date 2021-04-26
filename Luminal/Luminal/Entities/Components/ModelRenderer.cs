using Luminal.OpenGL.Models;
using OpenTK.Mathematics;
using Luminal.OpenGL;

namespace Luminal.Entities.Components
{
    public class ModelRenderer : Component3D
    {
        public Model Model;
        public Material Material = new("ModelMaterial");

        public override void Render3D()
        {
            ECSScene.Program.Use();

            var model = Matrix4.Identity;
            model *= Matrix4.CreateFromQuaternion(Parent.Quat);
            model *= Matrix4.CreateTranslation(Parent.Position.ToOpenTK());
            ECSScene.Program.UniformMatrix4("Model", ref model);

            Material.SetShaderVariables();

            Model.Draw();
        }
    }
}