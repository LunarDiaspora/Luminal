using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Luminal.AssetTypes;
using Luminal.Graphics.GLObjects;
using OpenTK.Graphics.OpenGL;
using Luminal.Core;
using Scene = Assimp.Scene;

namespace Luminal.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct InternalVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
    }

    public class InternalMesh
    {
        public List<InternalVertex> Vertices = new();
        public List<uint> Indices = new();

        public GLVertexArrayObject VAO;
        public GLBuffer<InternalVertex> VBO;
        public GLBuffer<uint> EBO;

        public Shader Shader;

        internal unsafe void Setup()
        {
            VAO = new();
            VBO = new();
            EBO = new();

            VAO.Bind();
            VBO.Buffer(BufferTarget.ArrayBuffer,
                Vertices.ToArray(),
                BufferUsageHint.StreamDraw);
            EBO.Buffer(BufferTarget.ElementArrayBuffer,
                Indices.ToArray(),
                BufferUsageHint.StreamDraw);

            var loc_Position = Shader.Program.GetAttribLocation("l2_Position");
            var loc_Normal = Shader.Program.GetAttribLocation("l2_Normal");
            var loc_UV = Shader.Program.GetAttribLocation("l2_UV");
            GL.EnableVertexAttribArray(loc_Position);
            GL.EnableVertexAttribArray(loc_Normal);
            GL.EnableVertexAttribArray(loc_UV);

            GL.VertexAttribPointer(loc_Position, 3,
                VertexAttribPointerType.Float, false,
                sizeof(InternalVertex), 0);
            GL.VertexAttribPointer(loc_Normal, 3,
                VertexAttribPointerType.Float, false,
                sizeof(InternalVertex), sizeof(float) * 3);
            GL.VertexAttribPointer(loc_UV, 2,
                VertexAttribPointerType.Float, false,
                sizeof(InternalVertex), sizeof(float) * 6);

            GL.BindVertexArray(0);
        }

        internal unsafe void Draw()
        {
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            VAO.Bind();
            Shader.Program.Use();

            GL.DrawElements(BeginMode.Triangles, Indices.Count, DrawElementsType.UnsignedShort, 0);
        }
    }

    public class InternalModel
    {
        public List<InternalMesh> Meshes = new();

        public void SetUp(Scene aiScene)
        {
            var root = aiScene.RootNode;
            DoNode(root, aiScene);
        }

        void DoNode(Node n, Scene sc)
        {
            for (int i = 0; i < n.MeshCount; i++)
            {
                var midx = n.MeshIndices[i];
                var mesh = sc.Meshes[midx];
                DoMesh(mesh);
            }
        }

        void DoMesh(Mesh m)
        {
            var mesh = new InternalMesh();

            for (int i=0; i<m.VertexCount; i++)
            {
                // here is where we gaming
                var vtx = new InternalVertex();
                var pos = m.Vertices[i];

                vtx.Position = pos.ToSysNum();
                if (m.HasNormals)
                {
                    vtx.Normal = m.Normals[i].ToSysNum();
                }

                if (m.HasTextureCoords(0))
                {
                    // we only care about this one UV channel
                    // (who needs more than one UV channel?)
                    var fuck = m.TextureCoordinateChannels[0][i]; // i THINK?
                    vtx.UV = new Vector2(fuck.X, fuck.Y); // why is this a vec3 tbh
                } else
                {
                    vtx.UV = new Vector2(0f, 0f);
                }

                mesh.Vertices.Add(vtx);
            }

            for (int i=0; i<m.FaceCount; i++)
            {
                var f = m.Faces[i];
                if (!f.HasIndices)
                {
                    throw new Exception("What");
                }

                for (int j=0; j<f.IndexCount; j++)
                {
                    mesh.Indices.Add((uint)f.Indices[j]);
                }
            }
        }
    }
}
