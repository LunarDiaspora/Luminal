#pragma once

#include <gl/glew.h>

namespace Luminal
{
    class GLTexture
    {
    public:
        void Create();

        void Load(const char* path);

        void Bind();

        unsigned int GLObject;
        int Width;
        int Height;
        int Channels;

        static void Active(unsigned int which);
    };
}