#include "GLTexture.h"

#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"
#include <iostream>

namespace Luminal
{
    void GLTexture::Create()
    {
        glGenTextures(1, &GLObject);
    }

    void GLTexture::Load(const char* path)
    {
        Bind();
        Active(GL_TEXTURE0);

        stbi_set_flip_vertically_on_load(true);
        stbi_set_unpremultiply_on_load(true);

        unsigned char* data = stbi_load(path, &Width, &Height, &Channels, STBI_rgb);
        assert(data);

        GLuint format = GL_RGBA;

        switch (Channels)
        {
        case 3:
            format = GL_RGB;
            break;
        case 4:
            format = GL_RGBA;
            break;
        }

        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Width, Height, 0, format, GL_UNSIGNED_BYTE, data);
        glGenerateMipmap(GL_TEXTURE_2D);

        stbi_image_free(data);
    }

    void GLTexture::Bind()
    {
        glBindTexture(GL_TEXTURE_2D, GLObject);
    }

    void GLTexture::Active(unsigned int which)
    {
        glActiveTexture(which);
    }
}