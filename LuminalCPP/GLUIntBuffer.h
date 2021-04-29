#pragma once

#include <gl/glew.h>

namespace Luminal
{
    class GLUIntBuffer
    {
    public:
        void Create();

        void BufferData(unsigned int* data, size_t size, unsigned int mode, unsigned int where);

        void Bind(unsigned int where);

        unsigned int GLObject;
    };
}