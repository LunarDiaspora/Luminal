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

    void GLShaderProgram::Uniform1f(std::string name, float val)
    {
        int i = glGetUniformLocation(GLObject, name.c_str());
        glUniform1f(i, val);
    }

    void GLShaderProgram::Uniform1i(std::string name, int val)
    {
        int i = glGetUniformLocation(GLObject, name.c_str());
        glUniform1i(i, val);
    }

    void GLShaderProgram::UniformMatrix4(std::string name, glm::mat4 mat)
    {
        int i = glGetUniformLocation(GLObject, name.c_str());
        glUniformMatrix4fv(i, 1, GL_FALSE, glm::value_ptr(mat));
    }
}