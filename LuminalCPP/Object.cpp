#include <glm/glm.hpp>
#include "Object.h"

namespace Luminal
{
    Object::Object()
    {
        Components.clear();

        Position = glm::vec3(0, 0, 0);
        Quaternion = glm::dquat(0, 0, 0, 1);
    }

    void Object::AddComponent(std::unique_ptr<Luminal::EngineComponent> c)
    {
        c->OnCreate();

        Components.push_back(c);
    }

    glm::mat4 Object::Model()
    {
        glm::mat4 m = glm::mat4(1.0f);
        m = glm::translate(m, Position);
        glm::mat4 qm = glm::toMat4(Quaternion);
        m *= qm;
        return m;
    }
}