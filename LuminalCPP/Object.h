#pragma once

#include <glm/glm.hpp>
#include <glm/gtx/quaternion.hpp>
#include <vector>
#include "Component.h"
#include <memory>

namespace Luminal
{
    class Object
    {
    public:
        Object();

        glm::vec3 Position;
        glm::dquat Quaternion;

        std::vector<std::unique_ptr<Luminal::EngineComponent>> Components;

        glm::mat4 Model();

        void AddComponent(std::unique_ptr<Luminal::EngineComponent> c);
    };
}