#pragma once

#include <gl/glew.h>

namespace Luminal
{
    class GLFloatBuffer
    {
    public:
        void Create();

        void BufferData(float data[], unsigned int mode);

        void Bind(unsigned int target);

        unsigned int GLObject;
    };
}