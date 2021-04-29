#pragma once

#include "GLShader.h"

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
    };
}