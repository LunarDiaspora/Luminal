using Luminal.OpenGL.Models;
using OpenTK.Mathematics;

namespace Luminal.Entities.Components
{
    public class ModelRenderer : Component3D
    {
        public Model Model;

        public override void Render3D()
        {
            var model = Matrix4.CreateTranslation(Parent.Position);
            model *= Matrix4.CreateFromQuaternion(Parent.Quat);
            ECSScene.Program.UniformMatrix4("Model", ref model);

            Model.Draw();
        }
    }
}