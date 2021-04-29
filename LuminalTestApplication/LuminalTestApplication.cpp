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

Luminal::GLShader vert;
Luminal::GLShader frag;
Luminal::GLShaderProgram prog;
Luminal::GLFloatBuffer buf;
Luminal::GLVertexArrayObject VAO;
Luminal::GLUIntBuffer EBO;
Luminal::GLTexture boris;
Luminal::Model pot;

Luminal::Object camera;
Luminal::Camera cam;

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
#if 0
        vert.Load("Resources/standard.vert", GL_VERTEX_SHADER);
        frag.Load("Resources/standard.frag", GL_FRAGMENT_SHADER);

        prog.Create();

        prog.Attach(vert).Attach(frag).Link();
        
        prog.Use();

        VAO.Create();

        VAO.Bind();

        buf.Create();

        buf.BufferData(vertices, sizeof(vertices), GL_STATIC_DRAW, GL_ARRAY_BUFFER);

        EBO.Create();

        EBO.BufferData(indices, sizeof(indices), GL_STATIC_DRAW, GL_ELEMENT_ARRAY_BUFFER);

        boris.Create();

        boris.Bind();
        boris.Load("Resources/boris.png");
#endif

        pot.Load("Resources/test.obj");

#if 0
        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)0);
        glEnableVertexAttribArray(0);

        glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)(3 * sizeof(float)));
        glEnableVertexAttribArray(1);

        glBindVertexArray(0);
#endif

        camera.AddComponent(std::make_unique<Luminal::Camera>(cam));
        camera.Position.z = -5.0f;
    };

    eng.OnDraw = []()
    {
        pot.Draw(prog);
    };

    eng.OnGUI = []()
    {
        ImGui::Begin("Test Window");

        ImGui::Text("Hello from Luminal, C++ edition!");

        ImGui::End();

        ImGui::ShowDemoWindow();
    };

    eng.Start(800, 600, "Test App");

    return 0;
}