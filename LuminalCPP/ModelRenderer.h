#pragma once

#include "Component.h"
#include "Model.h"
#include "Material.h"

namespace Luminal
{
    class ModelRenderer : public EngineComponent
    {
        virtual EngineComponent* clone() override
        {
            return new ModelRenderer(*this); // Use a std::unique_ptr! Managing memory manually is a bad idea.
        }

    public:
        std::shared_ptr<Luminal::Model> Model = nullptr;

        Luminal::Material Material;

        void SetShaderVariables();

        void OnRender() override;

        void LoadModel(const char* path);
    };
}