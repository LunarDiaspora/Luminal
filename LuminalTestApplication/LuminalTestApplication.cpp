#include<iostream>
#include<Luminal.h>
#include<gl/glew.h>
#include<GLShader.h>
#include<GLShaderProgram.h>
#include "imgui.h"
#include<GLFloatBuffer.h>
#include<GLVertexArrayObject.h>
#include<GLUIntBuffer.h>
#include<GLTexture.h>
#include <Model.h>
#include<Scene.h>
#include<Camera.h>
#include<Object.h>
#include <ModelRenderer.h>

Luminal::GLShader vert;
Luminal::GLShader frag;
Luminal::GLShaderProgram prog;
Luminal::GLFloatBuffer buf;
Luminal::GLVertexArrayObject VAO;
Luminal::GLUIntBuffer EBO;
Luminal::GLTexture boris;
Luminal::Model pot;

std::unique_ptr<Luminal::Object> camera = std::make_unique<Luminal::Object>();
std::unique_ptr<Luminal::Camera> cam = std::make_unique<Luminal::Camera>();
std::unique_ptr<Luminal::Object> model = std::make_unique<Luminal::Object>();
Luminal::ModelRenderer mr;

float vertices[] = {
     0.5f,  0.5f, 0.0f, 1.0f, 1.0f,  // top right
     0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
    -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
    -0.5f,  0.5f, 0.0f, 0.0f, 1.0f // top left 
};
unsigned int indices[] = {  // note that we start from 0!
    0, 1, 3,   // first triangle
    1, 2, 3    // second triangle
};

#undef main // Dirty hack thanks msvc
int main()
{
    Luminal::Engine eng;
    std::cout << "Starting engine\n";

    eng.OnLoaded = []()
    {
        camera->AddComponent(std::move(cam));
        camera->Position.z = -5.0f;

        Luminal::Scene::AddObject(std::move(camera));

        mr.LoadModel("Resources/test.obj");
        model->AddComponent(std::make_unique<Luminal::ModelRenderer>(mr));

        Luminal::Scene::AddObject(std::move(model));
    };

    eng.OnDraw = []()
    {
    };

    eng.OnGUI = []()
    {
        ImGui::Begin("Test Window");

        ImGui::Text("Hello from Luminal, C++ edition!");

        ImGui::End();

        ImGui::ShowDemoWindow();
    };

    eng.Start(1366, 768, "Test App");

    return 0;
}