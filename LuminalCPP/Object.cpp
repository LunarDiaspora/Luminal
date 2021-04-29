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
}