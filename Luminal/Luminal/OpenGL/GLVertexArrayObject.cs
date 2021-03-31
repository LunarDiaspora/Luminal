using OpenTK.Graphics.OpenGL;

namespace Luminal.OpenGL
{
    public class GLVertexArrayObject
    {
        public int GLObject;

        public GLVertexArrayObject()
        {
            GLObject = GL.GenVertexArray();
        }

        public GLVertexArrayObject(string name)
        {
            GLObject = GL.GenVertexArray();
            GLHelper.LabelObj(ObjectLabelIdentifier.VertexArray, GLObject, $"VAO: {name}");
        }

        public void Bind()
        {
            GL.BindVertexArray(GLObject);
        }
    }
}