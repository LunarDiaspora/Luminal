#pragma once

#include "Object.h"

namespace Luminal
{
    class EngineComponent
    {
    public:
        Object parent;

        virtual void OnGUI() {};
        virtual void OnRender() {};
        virtual void OnUpdate(float dt) {};
        virtual void OnCreate() {};
        virtual void OnDestroy() {};
    };
}