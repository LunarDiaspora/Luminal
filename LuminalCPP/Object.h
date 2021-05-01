#pragma once

#include <glm/glm.hpp>
#include <glm/gtx/quaternion.hpp>
#include <vector>
#include <memory>

namespace Luminal
{
    class EngineComponent;

    class Object
    {
    public:
        Object();

        glm::vec3 Position;
        glm::dquat Quaternion;

        std::vector<std::unique_ptr<EngineComponent>> Components;

        glm::mat4 Model();

        void AddComponent(std::unique_ptr<EngineComponent> c);

    	Object(const Object& obj);
    	Object& operator=(const Object& obj);

        void Complete();
    };
}