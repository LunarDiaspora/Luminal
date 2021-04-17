using Assimp;
using Luminal.Logging;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Luminal.OpenGL.Models
{
    public class Model
    {
        public List<Mesh> Meshes = new();
        public string Path;

        public Model(string path)
        {
            Path = path;
            // do things

            ImportModel(path);
        }

        private void ImportModel(string path)
        {
            var importer = new AssimpContext();
            var scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

            if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
            {
                Log.Error($"Failed to import model on {path}!");
                throw new Exception("Model import failure!");
            }

            DoNode(scene.RootNode, scene);

            // okay we're done, we can throw this away now
            importer.Dispose();
        }

        private void DoNode(Node node, Scene sc)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                var mi = node.MeshIndices[i];
                var m = sc.Meshes[mi];

                Meshes.Add(DoMesh(m, sc));
            }

            for (int j = 0; j < node.ChildCount; j++)
            {
                var child = node.Children[j];
                DoNode(child, sc);
            }
        }

        private Vector3 AssimpV3ToOTK(Vector3D vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        private Vector2 ThreeToTwo(Vector3 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        private Mesh DoMesh(Assimp.Mesh inp, Scene sc)
        {
            List<Vertex> verts = new();
            List<uint> inds = new();
            List<GLTexture> texes = new();

            for (int i = 0; i < inp.VertexCount; i++)
            {
                Vertex v = new();
                // vertex^2

                var position = inp.Vertices[i];
                v.Position = AssimpV3ToOTK(position);
                var normal = inp.Normals[i];
                v.Normal = AssimpV3ToOTK(normal);

                if (inp.HasTextureCoords(0))
                {
                    var chan = inp.TextureCoordinateChannels[0];
                    var uvc = chan.Count;
                    var vc = inp.VertexCount;
                    if (uvc != vc)
                    {
                        Log.Wtf($"Luminal 3D: (Model.cs) Irregular UV channel length!?\nVertex {i}, {uvc} UVs, {vc} vertices.\n" +
                                "Refusing to load this vertex's texture data!");
                    }
                    else
                    {
                        var auv = chan[i];
                        var uv = ThreeToTwo(AssimpV3ToOTK(auv));
                        v.UV = uv;
                    }
                }
                else
                {
                    v.UV = new Vector2(0.0f, 0.0f); // No UVs, make sure nothing breaks
                }

                v.Colour = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);

                verts.Add(v);
            }

            for (int j = 0; j < inp.FaceCount; j++)
            {
                var face = inp.Faces[j];
                if (!face.HasIndices)
                {
                    Log.Wtf($"Luminal 3D: (Model.cs) Face.HasIndices returned false???");
                    throw new Exception("Face.HasIndices returned false. What.");
                }

                for (int k = 0; k < face.IndexCount; k++)
                {
                    var index = face.Indices[k];
                    inds.Add((uint)index);
                }
            }

            if (inp.MaterialIndex >= 0)
            {
                var mat = sc.Materials[inp.MaterialIndex];
                DoTextures(mat, inp, sc);
            }

            var outputMesh = new Mesh(verts, inds, texes);
            return outputMesh;
        }

        private void DoTextures(Material mat, Assimp.Mesh mesh, Scene sc)
        {
            var diffuse = new List<GLTexture>();
            var texes = mat.GetAllMaterialTextures();
            Log.Debug($"{texes.Length}");

            for (int i = 0; i < mat.GetMaterialTextureCount(TextureType.Diffuse); i++)
            {
                mat.GetMaterialTexture(TextureType.Diffuse, i, out TextureSlot tex);
                Log.Debug(tex.FilePath);
            }
        }

        public void Draw()
        {
            foreach (var v in Meshes) v.Draw();
        }
    }
}