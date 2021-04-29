#include "GLVertexArrayObject.h"
#include <gl/glew.h>

namespace Luminal
{
    void GLVertexArrayObject::Create()
    {
        glCreateVertexArrays(1, &GLObject);
    }

    void GLVertexArrayObject::Bind()
    {
        glBindVertexArray(GLObject);
    }
}