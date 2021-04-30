#pragma once

#include <glm/glm.hpp>
#include "GLTexture.h"

namespace Luminal
{
    struct Material
    {
        glm::vec3 Albedo;
        glm::vec3 Specular;

        Luminal::GLTexture* AlbedoMap = nullptr;
        Luminal::GLTexture* SpecularMap = nullptr;

        float Shininess = 64.0f;

        bool HasAlbedo()
        {
            return AlbedoMap != nullptr;
        }

        bool HasSpecular()
        {
            return SpecularMap != nullptr;
        }
    };
}