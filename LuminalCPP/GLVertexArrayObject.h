#pragma once

namespace Luminal
{
    class GLVertexArrayObject
    {
    public:
        void Create();

        void Bind();

        unsigned int GLObject;
    };
}