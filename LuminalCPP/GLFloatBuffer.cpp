#include "GLFloatBuffer.h"

namespace Luminal
{
    void GLFloatBuffer::Create()
    {
        glGenBuffers(1, &GLObject);
    }

    void GLFloatBuffer::BufferData(float data[], unsigned int mode = GL_STATIC_DRAW)
    {
        Bind(GL_ARRAY_BUFFER);
        glBufferData(GL_ARRAY_BUFFER, sizeof(data), data, mode);
    }

    void GLFloatBuffer::Bind(unsigned int target)
    {
        glBindBuffer(target, GLObject);
    }
}