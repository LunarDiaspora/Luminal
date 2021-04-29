#pragma once

#include <gl/glew.h>

namespace Luminal
{
    class GLFloatBuffer
    {
    public:
        void Create();

        void BufferData(float* data, size_t size, unsigned int mode, unsigned int where);

        void Bind(unsigned int target);

        unsigned int GLObject;
    };
}