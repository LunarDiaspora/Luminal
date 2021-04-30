#include "PointLight.h"
#include "Scene.h"

namespace Luminal
{
    void PointLight::OnCreate()
    {
        auto p = std::unique_ptr<Luminal::PointLight>(this);
        Scene::PointLights.push_back(p);
    }

    void PointLight::OnDestroy()
    {
        auto j = std::find_if(Scene::PointLights.begin(), Scene::PointLights.end(),
            [this](const std::unique_ptr<PointLight>& x) { return this == x.get(); });
        Scene::PointLights.erase(j);
    }
}
