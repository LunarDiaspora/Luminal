#pragma once
#include <string>

namespace Luminal
{
    class GLShader
    {
    public:
        void Load(const char* filename, unsigned int type);
        std::string SourceCode;

        unsigned int GLObject;
    };
}