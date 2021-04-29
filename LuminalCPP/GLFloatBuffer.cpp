#include "GLFloatBuffer.h"

namespace Luminal
{
    void GLFloatBuffer::Create()
    {
        glGenBuffers(1, &GLObject);
    }

    void GLFloatBuffer::BufferData(float* data, size_t size, unsigned int mode = GL_STATIC_DRAW, unsigned int where = GL_ARRAY_BUFFER)
    {
        Bind(where);
        glBufferData(where, size, data, mode);
    }

    void GLFloatBuffer::Bind(unsigned int target)
    {
        glBindBuffer(target, GLObject);
    }
}