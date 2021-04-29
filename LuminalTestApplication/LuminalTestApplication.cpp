#include<iostream>
#include<Luminal.h>
#include<gl/glew.h>
#include<GLShader.h>
#include<GLShaderProgram.h>
#include "imgui.h"
#include<GLFloatBuffer.h>

Luminal::GLShader vert;
Luminal::GLShader frag;
Luminal::GLShaderProgram prog;
Luminal::GLFloatBuffer buf;

float vertices[] = {
    -0.5f, -0.5f, 0.0f,
     0.5f, -0.5f, 0.0f,
     0.0f,  0.5f, 0.0f
};

#undef main // Dirty hack thanks msvc
int main()
{
    Luminal::Engine eng;
    std::cout << "Starting engine\n";

    eng.OnLoaded = []()
    {
        vert.Load("Resources/standard.vert", GL_VERTEX_SHADER);
        frag.Load("Resources/standard.frag", GL_FRAGMENT_SHADER);

        prog.Create();

        prog.Attach(vert).Attach(frag).Link();

        buf.Create();

        buf.BufferData(vertices, GL_STATIC_DRAW);
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

    eng.Start(800, 600, "Test App");

    return 0;
}