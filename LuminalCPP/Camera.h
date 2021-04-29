#pragma once

#include "Component.h"

#include <glm/glm.hpp>

namespace Luminal
{
    class Camera : public EngineComponent
    {
    public:
        void OnCreate() override;

        glm::mat4 View();
        glm::mat4 Projection();

        float FieldOfView = 70.0f;
        float Near = 0.1f;
        float Far = 1000.0f;

    	virtual EngineComponent* clone() override {
			return new Camera(*this); // Rare instance where new is an okay idea, assuming this gets wrapped in a std::unique_ptr immediately
    	}
    };
}