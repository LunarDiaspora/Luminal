using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using OpenTK.Graphics.OpenGL;
using Luminal.Assets;
using Luminal.Models;
using Luminal.Graphics;
using System.Diagnostics;
using ImGuiNET;
using Luminal.Private;
using Luminal.DebugUI;
using Luminal.Logging;
using Luminal.Assets.Complex;
using Luminal.Packages;
using Luminal.AssetTypes;

namespace Luminal.Core
{
    public static class Engine
    {
        public static LuminalProject LoadedProject;
        public static string AbsoluteAssetPath;

        public static IntPtr Window;
        public static IntPtr GLContext; // No idea when we'll use this, but meh

        public static bool Running;

        public static bool Debug
#if DEBUG
            = true;
#else
            = false;
#endif

        public static void Start(string projectPath)
        {
            LoadedProject = LuminalProject.LoadFromDisk(projectPath);
            AbsoluteAssetPath = LoadedProject.AbsoluteAssetsRoot;

            Log.Info("-- Luminal2 Engine Startup --");
            Log.Info("A new beginning, for everyone.");
            Log.Info($"Project loaded: {LoadedProject.Name}");
            Log.Info($"Asset root: {AbsoluteAssetPath}");

            PackageGlobals.AddAssets();
            foreach (var pkg in LoadedProject.Dependencies)
            {
                PackageGlobals.LoadPackage(pkg);
            }
            ComplexTypeManager.Init();

            var playerConfig = Asset.Load<PlayerConfig>(LoadedProject.PlayerConfig);

            // Now we can proceed to create the window
            CreateWindow(playerConfig);

            Running = true;

            // Main loop - we shouldn't be exiting from here until the app ends.
            MainLoop();
        }

        public static void CreateWindow(PlayerConfig config)
        {
            var win = SDL.SDL_CreateWindow(config.Title,
                                        SDL.SDL_WINDOWPOS_UNDEFINED,
                                        SDL.SDL_WINDOWPOS_UNDEFINED,
                                        config.W,
                                        config.H,
                                        SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN |
                                        SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

            Viewport.WindowWidth = config.W;
            Viewport.WindowHeight = config.H;

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 4);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 0);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK,
                (int)SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
            var glContext = SDL.SDL_GL_CreateContext(win);

            // Load our OpenTK bindings
            // OpenTK does do windowing but we're not using it.
            GL.LoadBindings(new SDLBindingContext());

            Log.Debug("SDL2 init OK");

            Window = win;
            GLContext = glContext;
        }

        public static unsafe void Quit(int code=0)
        {
            SDL.SDL_GL_DeleteContext(GLContext);
            SDL.SDL_DestroyWindow(Window);

            Environment.Exit(code);
        }

        public static unsafe void MainLoop()
        {
            var stopwatch = new Stopwatch();

            var imCtx = ImGui.CreateContext();
            ImGui.SetCurrentContext(imCtx);

            var style = ImGui.GetStyle();
            ImGui.StyleColorsDark();

            style.WindowRounding = 8;

            var imIo = ImGui.GetIO();

            imIo.DisplaySize.X = Viewport.WindowWidth;
            imIo.DisplaySize.Y = Viewport.WindowHeight;

            ImGuiImplSDL2.Init(Window);
            ImGuiImplOpenGL3.Init();

            stopwatch.Start();

            var glVersion = GL.GetString(StringName.Version);
            var glRenderer = GL.GetString(StringName.Renderer);

            Log.Debug($"OpenGL {glVersion}");
            Log.Debug(@$"on ""{glRenderer}""");

            while (Running)
            {
                SDL.SDL_Event evt;
                while (SDL.SDL_PollEvent(out evt) != 0)
                {
                    ImGuiImplSDL2.ProcessEvent(evt);
                    switch (evt.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            Running = false;
                            break;
                    }
                }

                var timePassed = stopwatch.Elapsed;
                stopwatch.Restart();

                var deltaTime = (float)timePassed.TotalSeconds;

                ImGuiImplSDL2.NewFrame();
                ImGui.NewFrame();

                DebugWindowManager.RenderAll();

                if (Debug)
                {
                    if (ImGui.BeginMainMenuBar())
                    {
                        if (ImGui.BeginMenu("~ Luminal2 ~"))
                        {
                            if (ImGui.MenuItem("Exit"))
                                Quit();

                            ImGui.EndMenu();
                        }

                        if (ImGui.BeginMenu("Windows"))
                        {
                            if (ImGui.MenuItem("Console"))
                                DebugWindowManager.OpenWindow<ConsoleWindow>();

                            ImGui.EndMenu();
                        }

                        ImGui.EndMainMenuBar();
                    }
                }

                GL.ClearColor(0, 0, 0, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit |
                         ClearBufferMask.DepthBufferBit);

                ImGui.Render();
                ImGuiImplOpenGL3.Render();

                SDL.SDL_GL_SwapWindow(Window);
            }
        }
    }
}
