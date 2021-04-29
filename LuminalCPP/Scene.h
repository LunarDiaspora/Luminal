#pragma once

#include "Camera.h"
#include "GLShaderProgram.h"

namespace Luminal
{
    class Object;

    class Scene
    {
    public:
        static Camera* Camera;

        static std::vector<Object> objects;

        static void AddObject(Object o);

        static void DrawAll();
        static void GUIAll();
        static void UpdateAll(float dt);

        static void OnStart();

        static GLShaderProgram Program;
        static GLShader VertexShader;
        static GLShader FragmentShader;

    private:
        static void BeforeFrame();
    };
}