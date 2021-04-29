/*
* 
* The Luminal Engine
* (C) 2021- Rin.
* 
*/

#include <SDL2/SDL.h>
#include <SDL2/SDL_ttf.h>
#include <imgui.h>
#include <imgui_stdlib.h>
#include <string>
#include "Luminal.h"
#include <iostream>
#include <GL/glew.h>
#include "imgui_impl_opengl3.h"
#include "imgui_impl_sdl.h"
#include <SDL2/SDL_syswm.h>
#include "Viewport.h"
#include "Scene.h"

namespace Luminal
{
    Engine::Engine()
    {
        OnUpdate = DummyOnUpdate;
        OnDraw = DummyProc;
        OnGUI = DummyProc;
        OnLoaded = DummyProc;
    }

    void Engine::Start(int windowWidth, int windowHeight, std::string windowTitle)
    {
        if (SDL_Init(SDL_INIT_EVERYTHING) < 0)
        {
            std::cerr << "SDL_Init failure!\n";
            std::cerr << SDL_GetError() << std::endl;
            return;
        }

        SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 4);
        SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 5);
        SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);
        SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 24);
        SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, 8);

        Window = SDL_CreateWindow(windowTitle.c_str(), SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
            windowWidth, windowHeight, SDL_WINDOW_OPENGL | SDL_WINDOW_RESIZABLE);

        WindowOpen = true;

        GLContext = SDL_GL_CreateContext(Window);

        SDL_GL_MakeCurrent(Window, GLContext);

        SDL_GL_SetSwapInterval(1);

        var t = glewInit();
        assert(t == GLEW_OK);

        SDL_SysWMinfo wmInfo;
        SDL_VERSION(&wmInfo.version);
        SDL_GetWindowWMInfo(Window, &wmInfo);

        ImGui::CreateContext();

        ImGuiIO& io = ImGui::GetIO();

        ImGui::StyleColorsDark();

        ImGui_ImplSDL2_InitForOpenGL(Window, GLContext);
        ImGui_ImplOpenGL3_Init();

        Scene::OnStart();

        OnLoaded();

        while (1)
        {
            SDL_Event evt;
            while (SDL_PollEvent(&evt))
            {
                ImGui_ImplSDL2_ProcessEvent(&evt);

                switch (evt.type)
                {
                case SDL_QUIT:
                    // User requested quit.
                    Quit(0);
                }
            }

            ImGui_ImplOpenGL3_NewFrame();
            ImGui_ImplSDL2_NewFrame(Window);
            ImGui::NewFrame();

            OnGUI();
            Scene::GUIAll();

            ImGuiIO& io = ImGui::GetIO();
            Viewport::Width = io.DisplaySize.x;
            Viewport::Height = io.DisplaySize.y;

            glViewport(0, 0, (int)io.DisplaySize.x, (int)io.DisplaySize.y);

            glClearColor(0.1, 0.1, 0.1, 1);

            glClear(GL_COLOR_BUFFER_BIT);

            OnUpdate(0);
            Scene::UpdateAll(0);
            OnDraw();
            Scene::DrawAll();

            ImGui::Render();

            ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());

            SDL_GL_SwapWindow(Window);
        }
    }

    void Engine::Quit(int code)
    {
        WindowOpen = false;

        SDL_DestroyWindow(Window);

        exit(code);
    }
}