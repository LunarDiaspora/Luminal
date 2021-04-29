#include "model.h"
#include <iostream>

namespace Luminal
{
    Mesh::Mesh(std::vector<Vertex> verts, std::vector<unsigned int> inds, std::vector<Texture> texes)
    {
        Vertices = verts;
        Indices = inds;
        Textures = texes;

        setup();
    }

    void Mesh::setup()
    {
        VAO.Create();
        VBO.Create();
        EBO.Create();

        VAO.Bind();
        VBO.Bind(GL_ARRAY_BUFFER);

        VBO.BufferData((float*)&Vertices[0], Vertices.size() * sizeof(Vertex), GL_STATIC_DRAW, GL_ARRAY_BUFFER);

        EBO.Bind(GL_ELEMENT_ARRAY_BUFFER);
        EBO.BufferData((unsigned int*)&Indices[0], Indices.size() * sizeof(unsigned int), GL_STATIC_DRAW, GL_ELEMENT_ARRAY_BUFFER);

        // Pointers

        // Position
        glEnableVertexAttribArray(0);
        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)0);

        // Normal
        glEnableVertexAttribArray(1);
        glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Normal));

        // UV
        glEnableVertexAttribArray(2);
        glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, UV));

        glBindVertexArray(0);
    }

    void Mesh::Draw(GLShaderProgram& prog)
    {
        unsigned int albedoCount = 1;
        unsigned int specularCount = 1;

        for (unsigned int i = 0; i < Textures.size(); i++)
        {
            glActiveTexture(GL_TEXTURE0 + i);
            std::string number;
            std::string name = Textures[i].Type;

            if (name == "texture_diffuse")
                number = std::to_string(albedoCount++);
            else if (name == "texture_specular")
                number = std::to_string(specularCount++);

            prog.Uniform1i(("material." + name + number).c_str(), i);

            glBindTexture(GL_TEXTURE_2D, Textures[i].ID);
        }

        glActiveTexture(GL_TEXTURE0);

        VAO.Bind();
        prog.Use();

        glDrawElements(GL_TRIANGLES, Indices.size(), GL_UNSIGNED_INT, 0);

        glBindVertexArray(0);
    }

    void Model::Draw(GLShaderProgram& prog)
    {
        for (unsigned int i = 0; i < Meshes.size(); i++)
            Meshes[i].Draw(prog);
    }

    void Model::Load(const char* cpath)
    {
        std::string path = std::string(cpath);

        Assimp::Importer importer; // When the importer is sus! :flushed:

        const aiScene* scene = importer.ReadFile(path,
            aiProcess_Triangulate | aiProcess_FlipUVs | aiProcess_GenNormals);

        if (!scene || !scene->mRootNode || scene->mFlags & AI_SCENE_FLAGS_INCOMPLETE)
        {
            std::cout << "AssImp error at " << path << "!\n";
            std::cout << importer.GetErrorString() << std::endl;
            throw("AssImp error");
        }

        Directory = path.substr(0, path.find_last_of('/'));

        DoNode(scene->mRootNode, scene);
    }

    void Model::DoNode(aiNode* node, const aiScene* scene)
    {
        for (unsigned int i = 0; i < node->mNumMeshes; i++)
        {
            aiMesh* m = scene->mMeshes[node->mMeshes[i]];
            Meshes.push_back(DoMesh(m, scene));
        }

        for (unsigned int i = 0; i < node->mNumChildren; i++)
        {
            DoNode(node->mChildren[i], scene);
        }
    }

    Mesh Model::DoMesh(aiMesh* mesh, const aiScene* scene)
    {
        std::vector<Vertex> verts;
        std::vector<unsigned int> inds;
        std::vector<Texture> texes;

        for (unsigned int i = 0; i < mesh->mNumVertices; i++)
        {
            Vertex vert;

            aiVector3D v = mesh->mVertices[i];
            glm::vec3 pos = glm::vec3(v.x, v.y, v.z);

            aiVector3D n = mesh->mNormals[i];
            glm::vec3 norm = glm::vec3(n.x, n.y, n.z);

            vert.Position = pos;
            vert.Normal = norm;

            if (mesh->mTextureCoords[0])
            {
                // We have texture coordinates
                aiVector3D aiuv = mesh->mTextureCoords[0][i];

                glm::vec2 uvs = glm::vec2(aiuv.x, aiuv.y);

                vert.UV = uvs;
            }

            verts.push_back(vert);
        }

        for (unsigned int i = 0; i < mesh->mNumFaces; i++)
        {
            aiFace face = mesh->mFaces[i];

            for (unsigned int j = 0; j < face.mNumIndices; j++)
            {
                inds.push_back(face.mIndices[j]);
            }
        }

        Mesh m = Mesh(verts, inds, {});
        return m;
    }
}