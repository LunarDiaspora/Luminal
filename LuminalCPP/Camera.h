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
    };
}