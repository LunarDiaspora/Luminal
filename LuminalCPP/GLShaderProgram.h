#pragma once

#include "GLShader.h"
#include <glm/glm.hpp>
#include <glm/gtc/type_ptr.hpp>

namespace Luminal
{
    class GLShaderProgram
    {
    public:
        void Create();
        
        GLShaderProgram Attach(GLShader sh);
        void Link();
        void Use();

        int GLObject;

        void Uniform1f(std::string name, float val);
        void Uniform1i(std::string name, int val);
        void UniformMatrix4(std::string name, glm::mat4 mat);
    };
}