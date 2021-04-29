#pragma once

#include "Camera.h"
#include "GLShaderProgram.h"

namespace Luminal
{
    class Object;

    class Scene
    {
    public:
        inline static Camera* Camera;

        inline static std::vector<Object> objects;

        static void AddObject(Object o);

        static void DrawAll();
        static void GUIAll();
        static void UpdateAll(float dt);

        static void OnStart();

        inline static GLShaderProgram Program;
        inline static GLShader VertexShader;
        inline static GLShader FragmentShader;

    private:
        static void BeforeFrame();
    };
}