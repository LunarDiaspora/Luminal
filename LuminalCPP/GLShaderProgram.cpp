#include "GLShaderProgram.h"
#include <gl/glew.h>
#include <iostream>

namespace Luminal
{
    void GLShaderProgram::Create()
    {
        GLObject = glCreateProgram();
    }

    GLShaderProgram GLShaderProgram::Attach(GLShader sh)
    {
        glAttachShader(GLObject, sh.GLObject);

        return *this;
    }

    void GLShaderProgram::Link()
    {
        glLinkProgram(GLObject);

        int ok;
        glGetProgramiv(GLObject, GL_LINK_STATUS, &ok);

        if (ok != 1)
        {
            char errorLog[2048];
            int len;
            glGetProgramInfoLog(GLObject, 2048, &len, errorLog);

            std::string errStr(errorLog);

            std::cout << "Failed to link program!\n";
            std::cout << errStr << std::endl;

            throw (errStr);
        }
    }

    void GLShaderProgram::Use()
    {
        glUseProgram(GLObject);
    }
}