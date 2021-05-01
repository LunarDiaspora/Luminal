#pragma once

#include "Component.h"

namespace Luminal
{
    class PointLight : public EngineComponent
    {
        virtual EngineComponent* clone() override
        {
            return new PointLight(*this);
        }

        void OnCreate() override;

        void OnDestroy() override;

        float Linear = 0.09f;
        float Quadratic = 0.012f;

        glm::vec3 Colour;
    };
}