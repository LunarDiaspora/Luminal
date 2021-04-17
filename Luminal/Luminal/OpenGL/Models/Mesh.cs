using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Luminal.OpenGL.Models
{
    public class Mesh
    {
        public List<Vertex> Vertices;
        public List<uint> Indices;
        public List<GLTexture> Textures;

        private GLVertexArrayObject VAO;
        private GLFloatBuffer VBO;
        private GLUIntBuffer EBO;

        public Mesh(List<Vertex> v, List<uint> i, List<GLTexture> t)
        {
            Vertices = v;
            Indices = i;
            Textures = t;

            SetUp();
        }

        private unsafe void SetUp()
        {
            VAO = new("Mesh VAO");
            VBO = new("Mesh VBO");
            EBO = new("Mesh EBO");

            VAO.Bind();
            VBO.Bind(BufferTarget.ArrayBuffer);

            // Buffer the vertex data into the VBO.
            var verarr = CollectionsMarshal.AsSpan(Vertices);
            GL.BufferData(BufferTarget.ArrayBuffer, verarr.Length * sizeof(Vertex), ref verarr[0], BufferUsageHint.StaticDraw);

            EBO.Bind(BufferTarget.ElementArrayBuffer);

            // Buffer the element array into the EBO.
            var indarr = CollectionsMarshal.AsSpan(Indices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indarr.Length * sizeof(uint), ref indarr[0], BufferUsageHint.StaticDraw);

            SetupPointers();

            //GL.BindVertexArray(0);
        }

        private unsafe void SetupPointers()
        {
            // Defining our attributes for memory-mapping.
            //
            // float float float float float float float float float float float float
            // ----------------- ----------------- ----------- -----------------------
            //     position            normal          UV               colour
            // ======================================================================>
            // stride (should always equal total size of struct)

            var fs = sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), 0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), fs * 3);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), fs * 6);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, sizeof(Vertex), fs * 8);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
        }

        public void Draw()
        {
            VAO.Bind();

            VBO.Bind(BufferTarget.ArrayBuffer);
            EBO.Bind(BufferTarget.ElementArrayBuffer);

            GL.Enable(EnableCap.DepthTest);

            GL.DrawElements(BeginMode.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
        }
    }
}