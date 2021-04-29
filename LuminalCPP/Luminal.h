#pragma once
#include <SDL2/SDL.h>
#include <string>

#define var auto

namespace Luminal
{
    class Engine
    {
    public:
        void Start(int windowWidth, int windowHeight, std::string windowTitle);
        static void Quit(int code);

        inline static SDL_Window* Window;
        inline static bool WindowOpen;
        inline static SDL_GLContext GLContext;

        void (*OnUpdate)(float delta);
        void (*OnDraw)();
        void (*OnGUI)();
        void (*OnLoaded)();

        Engine();

    private:
        static void DummyOnUpdate(float _) {};
        static void DummyProc() {};
    };
}
