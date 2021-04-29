#include "Scene.h"

#include "Component.h"

#include <gl/glew.h>

namespace Luminal
{
    void Scene::AddObject(Object o)
    {
        objects.push_back(o);
    }

    void Scene::DrawAll()
    {
        BeforeFrame();

        for (unsigned int i = 0; i < objects.size(); i++)
            for (unsigned int j = 0; j < objects[i].Components.size(); j++)
                objects[i].Components[j]->OnRender();
    }

    void Scene::UpdateAll(float dt)
    {
        for (unsigned int i = 0; i < objects.size(); i++)
            for (unsigned int j = 0; j < objects[i].Components.size(); j++)
                objects[i].Components[j]->OnUpdate(dt);
    }

    void Scene::GUIAll()
    {
        for (unsigned int i = 0; i < objects.size(); i++)
            for (unsigned int j = 0; j < objects[i].Components.size(); j++)
                objects[i].Components[j]->OnGUI();
    }

    void Scene::OnStart()
    {
        Program.Create();

        VertexShader.Load("Resources/standard.vert", GL_VERTEX_SHADER);
        FragmentShader.Load("Resources/standard.frag", GL_FRAGMENT_SHADER);

        Program.Attach(VertexShader).Attach(FragmentShader).Link();
    }

    void Scene::BeforeFrame()
    {
        assert(Camera);

        glm::mat4 view = Camera->View();
        glm::mat4 proj = Camera->Projection();

        Program.UniformMatrix4("View", view);
        Program.UniformMatrix4("Projection", proj);
    }
}