#include <string>
#include <stdio.h>
#include <stdlib.h>
#include "GLShader.h"
#include <gl/glew.h>
#include <iostream>

std::string ReadFile(const char* file)
{
    std::FILE* fp = std::fopen(file, "rb");
    if (fp)
    {
        std::string contents;
        std::fseek(fp, 0, SEEK_END);
        contents.resize(std::ftell(fp));
        std::rewind(fp);
        std::fread(&contents[0], 1, contents.size(), fp);
        std::fclose(fp);
        return contents;
    }
    throw (errno);
}

namespace Luminal
{
    void GLShader::Load(const char* file, unsigned int type)
    {
        SourceCode = ReadFile(file);

        GLObject = glCreateShader(type);

        GLchar const* f[] = { SourceCode.c_str() };
        GLint length[] = { SourceCode.size() };

        glShaderSource(GLObject, 1, f, length);
        glCompileShader(GLObject);

        int ok;
        glGetShaderiv(GLObject, GL_COMPILE_STATUS, &ok);

        if (ok != 1)
        {
            char errorLog[2048];
            int len;
            glGetShaderInfoLog(GLObject, 2048, &len, errorLog);

            std::string errStr(errorLog);

            std::cout << "Failed to compile shader!\n";
            std::cout << errStr << std::endl;
            return;
        }
    }
}