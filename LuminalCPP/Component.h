#pragma once
#include "Object.h"

namespace Luminal
{

    class EngineComponent
    {
    public:
        Object* parent = nullptr;

        // These should probably be pure, no? You don't wanna instantiate an abstract EngineComponent
        virtual void OnGUI() {};
        virtual void OnRender() {};
        virtual void OnUpdate(float dt) {};
        virtual void OnCreate() {};
        virtual void OnDestroy() {};

        virtual EngineComponent* clone() = 0;


        

        /*
        Example implementation

        class CompA : public EngineComponent {
            virtual EngineComponent* clone() override {
                return new CompA(*this); // Rare instance where new is an okay idea, assuming this gets wrapped in a std::unique_ptr immediately
            }
        };
        
    
         */
    };

    
}