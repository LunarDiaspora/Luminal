#pragma once

namespace Luminal
{
    // forward declaring it
    class Object
    {};

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