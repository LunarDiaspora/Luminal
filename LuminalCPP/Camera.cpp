#include "Camera.h"
#include "Scene.h"
#include "Viewport.h"

namespace Luminal
{
    void Camera::OnCreate()
    {
        Scene::Camera = this;
    }

    glm::mat4 Camera::View()
    {
        glm::mat4 m = glm::mat4(1.0f);
        m = glm::translate(m, parent.Position);
        glm::mat4 qm = glm::toMat4(parent.Quaternion);
        m *= qm;
        return m;
    }

    glm::mat4 Camera::Projection()
    {
        glm::mat4 m = glm::perspective(glm::radians(FieldOfView), Viewport::Width / Viewport::Height, Near, Far);
        return m;
    }
}