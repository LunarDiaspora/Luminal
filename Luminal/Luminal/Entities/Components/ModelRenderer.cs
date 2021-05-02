using Luminal.OpenGL.Models;
using OpenTK.Mathematics;
using Luminal.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace Luminal.Entities.Components
{
    public class ModelRenderer : Component3D
    {
        public Model Model;
        public Material Material = new("ModelMaterial");

        public bool BackfaceCulling = true;

        public override void Render3D()
        {
            ECSScene.Program.Use();

            if (BackfaceCulling)
            {
                GL.Enable(EnableCap.CullFace);
            } else
            {
                GL.Disable(EnableCap.CullFace);
            }

            var model = Matrix4.Identity;
            model *= Matrix4.CreateFromQuaternion(Parent.Quat);
            model *= Matrix4.CreateTranslation(Parent.Position.ToOpenTK());
            ECSScene.Program.UniformMatrix4("Model", ref model);

            Material.SetShaderVariables();

            Model.Draw();
        }
    }
}