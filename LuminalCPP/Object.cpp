#include <glm/glm.hpp>
#include "Object.h"
#include "Component.h"

namespace Luminal
{
    Object::Object()
    {
        Components.clear();

        Position = glm::vec3(0, 0, 0);
        Quaternion = glm::dquat(0, 0, 0, 1);
    }

    void Object::AddComponent(std::unique_ptr<EngineComponent> c)
    {
		c->parent = this;
        c->OnCreate();
        Components.emplace_back(std::move(c));
    }

	Object::Object(const Object& obj)
	{
    	*this = obj;
	}

	Object& Object::operator=(const Object& obj)
	{
		Position = obj.Position;
    	Quaternion = obj.Quaternion;

    	for (const auto& comp: obj.Components) {
    		// Copy the component and update its parent to *this
    		Components.emplace_back(std::unique_ptr<EngineComponent>(comp->clone()))->parent = this;
    	}
    
		return *this;
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