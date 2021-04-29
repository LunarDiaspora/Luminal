#pragma once

#include <glm/glm.hpp>
#include <glm/gtx/quaternion.hpp>
#include <vector>
#include "Component.h"

namespace Luminal
{
    class Object
    {
    public:
        Object();

        glm::vec3 Position;
        glm::dquat Quaternion;

        std::vector<Component*> Components;
    };
}