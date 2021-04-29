#include "GLUIntBuffer.h"

namespace Luminal
{
    void GLUIntBuffer::Create()
    {
        glGenBuffers(1, &GLObject);
    }

    void GLUIntBuffer::BufferData(unsigned int* data, size_t size, unsigned int mode = GL_STATIC_DRAW, unsigned int where = GL_ELEMENT_ARRAY_BUFFER)
    {
        Bind(where);
        glBufferData(where, size, data, mode);
    }

    void GLUIntBuffer::Bind(unsigned int where = GL_ARRAY_BUFFER)
    {
        glBindBuffer(where, GLObject);
    }
}