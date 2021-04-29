#pragma once

#include <glm/glm.hpp>
#include <string>
#include <vector>
#include "GLShaderProgram.h"
#include "GLUIntBuffer.h"
#include "GLFloatBuffer.h"
#include "GLVertexArrayObject.h"
#include <assimp/Importer.hpp>
#include <assimp/scene.h>
#include <assimp/postprocess.h>

namespace Luminal
{
    struct Vertex
    {
        glm::vec3 Position;
        glm::vec3 Normal;
        glm::vec2 UV;
    };

    struct Texture
    {
        unsigned int ID;
        std::string Type;
    };

    class Mesh
    {
    public:
        std::vector<Vertex> Vertices;
        std::vector<unsigned int> Indices;
        std::vector<Texture> Textures;

        Mesh(std::vector<Vertex> verts, std::vector<unsigned int> inds, std::vector<Texture> texes);
        void Draw(GLShaderProgram& shader);

    private:
        GLFloatBuffer VBO;
        GLUIntBuffer EBO;
        GLVertexArrayObject VAO;

        void setup();
    };

    class Model
    {
    public:
        void Load(const char* path);

        void Draw(GLShaderProgram& shader);

    private:
        std::vector<Mesh> Meshes;
        std::string Directory;

        void DoNode(aiNode* node, const aiScene* scene);
        Mesh DoMesh(aiMesh* mesh, const aiScene* scene);

        std::vector<Texture> doTextures(aiMaterial* mat, aiTextureType t, std::string name);
    };
}