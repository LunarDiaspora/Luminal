using Luminal.Graphics;
using Luminal.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Luminal.Configuration;
using SDL2;
using SFML.System;
using Luminal.Logging;
using Luminal.LGUI;
using Luminal.OpenGL;

namespace Luminal.Core
{
    public enum LuminalFlags
    {
        ENABLE_DEAR_IMGUI = 1<<0
    }

    public class Engine
    {
        public static IntPtr Renderer; // SDL_Renderer*
        public static IntPtr Window; // SDL_Window*
        public static IntPtr Screen;

        public SceneManager sceneManager;

        public static bool WindowOpen;

        public Clock sfClock;

        public delegate void FinishedLoadCallback(Engine main);
        public event FinishedLoadCallback OnFinishedLoad;

        public delegate void LoadingCallback(Engine main);
        public event LoadingCallback OnLoading;

        public delegate void UpdateCallback(Engine main, float Delta);
        public event UpdateCallback OnUpdate;

        public delegate void DrawCallback(Engine main);
        public event DrawCallback OnDraw;

        public delegate void GUICallback(Engine main);
        public event GUICallback OnGUI;

        public delegate void KeyDownCallback(Engine main, SDL.SDL_Scancode Keycode);
        public delegate void KeyUpCallback(Engine main, SDL.SDL_Scancode Keycode);

        public event KeyDownCallback KeyDown;
        public event KeyUpCallback KeyUp;

        public static int Width;
        public static int Height;

        public static IntPtr GlContext;

        public Engine(int logLevel = 0)
        {
            var logger = new ConsoleLogger();
            Log.SetLogger(logger);
            Log.SetLogLevel(logLevel);
        }

        public void StartRenderer(int WindowWidth, int WindowHeight, string WindowTitle, Type executingType,
                                  LuminalFlags Flags = 0)
        {
            Log.Info($"--- Luminal Engine ---\nStarting at {WindowWidth} x {WindowHeight} (\"{WindowTitle}\")\nExecuting application: {executingType.Name}\n");

            Width = WindowWidth;
            Height = WindowHeight;

            var config = LuminalConfigLoader.LoadConfig("Luminal.json");

            AudioEngineManager.LoadEngine(config.AudioPlugin);

            sceneManager = new SceneManager(executingType);
            //sceneManager.SwitchScene("Dummy");

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 2);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 1);

            Window = SDL.SDL_CreateWindow(WindowTitle, 200, 200, WindowWidth, WindowHeight,
                                          SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
            //Renderer = SDL.SDL_CreateRenderer(Window, 0, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            var winid = SDL.SDL_GetWindowID(Window);

            SDL_GPU.GPU_SetInitWindow(winid);

            Context.SetColour(255, 255, 255, 255);

            Log.Info("Loading SDL2_ttf");
            SDL_ttf.TTF_Init();

            Log.Info("Loading SDL2_image");
            SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG |
                               SDL_image.IMG_InitFlags.IMG_INIT_TIF | SDL_image.IMG_InitFlags.IMG_INIT_WEBP);

            SDL_GPU.GPU_SetPreInitFlags(GPU_InitFlagEnum.GPU_INIT_DISABLE_VSYNC);

            //Screen = SDL_GPU.GPU_Init((uint)Width, (uint)Height, (uint)(SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL));

            var winflags = SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;

            Screen = SDL_GPU.GPU_InitRenderer(GPU_RendererEnum.GPU_RENDERER_OPENGL_3, (uint)Width, (uint)Height, (uint)winflags);
            SDL_GPU.GPU_SetDefaultAnchor(0, 0);

            //GlContext = SDL.SDL_GL_CreateContext(Window);

            if ((Flags | LuminalFlags.ENABLE_DEAR_IMGUI) > 0)
            {
                Log.Info("Enabling Dear IMGUI.");
                IMGUIManager.Initialise();
            }

            if (OnLoading != null) OnLoading(this);


            //var sdlResult = SDL.SDL_CreateWindowAndRenderer(WindowWidth, WindowHeight, 0, out Renderer, out Window);
            //Console.WriteLine($"{sdlResult}");
            //SDL.SDL_SetWindowTitle(Window, WindowTitle);

            //Window.SetFramerateLimit(500);

            if (OnFinishedLoad != null) OnFinishedLoad(this);
            WindowOpen = true;

            sfClock = new Clock();

            while (WindowOpen)
            {
                //SDL.SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 255);
                //SDL.SDL_RenderClear(Renderer);
                //SDL.SDL_SetRenderDrawColor(Renderer, 255, 255, 255, 255);


                SDL_GPU.GPU_ClearColor(Screen, Context.MakeColourFromRGBA(0, 0, 0, 255));

                GUIManager.Begin();

                if ((Flags | LuminalFlags.ENABLE_DEAR_IMGUI) > 0)
                {
                    IMGUIManager.BeforeFrame();
                }

                if (OnGUI != null) OnGUI(this);

                SDL.SDL_Event evt;
                while (SDL.SDL_PollEvent(out evt) == 1)
                {
                    switch (evt.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            WinClose();
                            break;
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            var e = evt.key.keysym.scancode;
                            WinKeyDown(e);
                            break;
                        case SDL.SDL_EventType.SDL_KEYUP:
                            var k = evt.key.keysym.scancode;
                            WinKeyUp(k);
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            var m = evt.button.button;
                            MouseButtonDown(m, evt.button.x, evt.button.y);
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                            var u = evt.button.button;
                            MouseButtonUp(u, evt.button.x, evt.button.y);
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEMOTION:
                            var d = evt.motion;
                            MouseDrag(d.x, d.y, d.xrel, d.yrel);
                            break;
                    }
                }

                var t = sfClock.Restart();

                AudioEngineManager.Engine.Update(t.AsSeconds());

                if (sceneManager.ActiveScene != null)
                    sceneManager.ActiveScene.Update(this, t.AsSeconds());

                if (OnUpdate != null)
                    OnUpdate(this, t.AsSeconds());

                if (OnDraw != null)
                    OnDraw(this);

                if (sceneManager.ActiveScene != null) 
                    sceneManager.ActiveScene.Draw(this);

                GUIManager.RenderAll();

                if ((Flags | LuminalFlags.ENABLE_DEAR_IMGUI) > 0)
                {
                    IMGUIManager.Draw();
                }

                SDL_GPU.GPU_ResetRendererState();

                //SDL.SDL_RenderPresent(Renderer);
                SDL_GPU.GPU_Flip(Screen);

                GUIManager.End();

                //SDL.SDL_Delay(1);
            }
        }

        public void Quit()
        {
            AudioEngineManager.Engine.Dispose(); // Clean up after ourselves

            WindowOpen = false;
            //SDL.SDL_DestroyWindow(Window);
            //SDL.SDL_DestroyRenderer(Renderer);

            SDL_GPU.GPU_Quit();

            Context.FreeAllImages();
        }

        private void WinKeyDown(SDL.SDL_Scancode scancode)
        {
            if (KeyDown != null)
                KeyDown(this, scancode);

            if (sceneManager.ActiveScene != null)
                sceneManager.ActiveScene.OnKeyDown(this, scancode);
        }

        private void WinKeyUp(SDL.SDL_Scancode scancode)
        {
            if (KeyUp != null)
                KeyUp(this, scancode);

            if (sceneManager.ActiveScene != null)
                sceneManager.ActiveScene.OnKeyUp(this, scancode);
        }

        private void MouseButtonDown(byte btn, int x, int y)
        {
            GUIManager.OnMouseDown(x, y, btn);
        }

        private void MouseButtonUp(byte btn, int x, int y)
        {
            GUIManager.OnMouseUp(x, y, btn);
        }

        private void MouseDrag(int x, int y, int xrel, int yrel)
        {
            GUIManager.OnMouseDrag(x, y, xrel, yrel);
        }


        private void WinClose()
        {
            Quit();
        }
    }
}
