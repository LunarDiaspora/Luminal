using ImGuiNET;
using Luminal.Audio;
using Luminal.Configuration;
using Luminal.Entities;
using Luminal.Graphics;
using Luminal.LGUI;
using Luminal.Logging;
using Luminal.OpenGL;
using Luminal.OpenGL.ImGuiTheme;
using SDL2;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Reflection;
using Luminal.Console;

namespace Luminal.Core
{
    public enum LuminalFlags
    {
        ENABLE_KEY_REPEAT = 1 << 0
    }

    public static class LuminalFlagsExtension
    {
        public static string GetFlagString(this LuminalFlags t)
        {
            var features = new List<string>();

            foreach (var e in Enum.GetNames(typeof(LuminalFlags)))
            {
                var val = Enum.Parse(typeof(LuminalFlags), e);
                var present = t.Has((LuminalFlags)val);
                if (present)
                    features.Add(e);
            }

            return string.Join(" | ", features);
        }

        public static bool Has(this LuminalFlags a, LuminalFlags b)
        {
            return (a & b) > 0;
        }
    }

    public class EngineVersion
    {
        public float Major;
        public float Minor;
        public float Patch;

        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        public EngineVersion(float maj, float min, float pat)
        {
            Major = maj;
            Minor = min;
            Patch = pat;
        }

        public static readonly EngineVersion Current = new(0, 1, 0);
    }

    public enum VSyncMode
    {
        OFF,
        SYNC,
        ADAPTIVE
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

        public event DrawCallback OnLateDraw;

        public event DrawCallback OnEarlyDraw;

        public delegate void GUICallback(Engine main);

        public event GUICallback OnGUI;

        public delegate void KeyDownCallback(Engine main, SDL.SDL_Scancode Keycode);

        public delegate void KeyUpCallback(Engine main, SDL.SDL_Scancode Keycode);

        public event KeyDownCallback KeyDown;

        public event KeyUpCallback KeyUp;

        public static int Width;
        public static int Height;

        public static IntPtr GlContext;

        public static LuminalFlags EngineFlags;

        public static VSyncMode VSync;

        public static bool EnableConsole = true;
        public static bool ConsoleOpen = false;

        public Engine(int logLevel = 0)
        {
            var logger = new ConsoleLogger();
            Log.SetLogger(logger);
            Log.SetLogLevel(logLevel);
        }

        public static SDL.SDL_Scancode StringToScancode(string s)
        {
            var val = $"SDL_SCANCODE_{s.ToUpper()}";
            var e = (SDL.SDL_Scancode)Enum.Parse(typeof(SDL.SDL_Scancode), val);
            return e;
        }

        public void StartRenderer(int WindowWidth, int WindowHeight, string WindowTitle, Type executingType,
                                  LuminalFlags Flags = 0, IImGuiTheme theme = null)
        {
            var fs = Flags.GetFlagString();
            var themename = "none";
            if (theme != null)
                themename = theme.GetType().Name;

            Log.Info($"-== [[ Luminal Engine v{EngineVersion.Current} ]] ==-");
            Log.Info($"Starting at {WindowWidth}x{WindowHeight} (\"{WindowTitle}\")");
            Log.Info($"Host program (passed to StartRenderer): {executingType.Name}");
            Log.Info($"Engine flags present: {(fs == "" ? "none" : fs)}");
            Log.Info($"ImGui theme present: {themename}");
            Log.Info("");

            ConsoleManager.FindAllEverywhere();

            Width = WindowWidth;
            Height = WindowHeight;

            Viewport.Size = new(Width, Height);

            EngineFlags = Flags;

            var config = LuminalConfigLoader.LoadConfig("Luminal.json");

            AudioEngine.Instance.Initialise();

            sceneManager = new SceneManager(executingType);

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 2);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 1);

            Window = SDL.SDL_CreateWindow(WindowTitle, 200, 200, WindowWidth, WindowHeight,
                                          SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);

            var winid = SDL.SDL_GetWindowID(Window);

            SDL_GPU.GPU_SetInitWindow(winid);

            Context.SetColour(255, 255, 255, 255);

            Log.Info("Loading SDL2_ttf");
            SDL_ttf.TTF_Init();

            Log.Info("Loading SDL2_image");
            SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG |
                               SDL_image.IMG_InitFlags.IMG_INIT_TIF | SDL_image.IMG_InitFlags.IMG_INIT_WEBP);

            SDL_GPU.GPU_SetPreInitFlags(GPU_InitFlagEnum.GPU_INIT_DISABLE_VSYNC);

            var winflags = SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;

            Screen = SDL_GPU.GPU_InitRenderer(GPU_RendererEnum.GPU_RENDERER_OPENGL_3, (uint)Width, (uint)Height, (uint)winflags);
            SDL_GPU.GPU_SetDefaultAnchor(0, 0);

            OpenGLManager.Initialise();
            theme ??= new LuminalTheme();
            theme.InitTheme(ImGui.GetStyle(), ImGui.GetIO());
            OpenGLManager.ImGuiCreateFontAtlas();

            if (OnLoading != null) OnLoading(this);

            ECSScene.L3D_SetUp();

            if (OnFinishedLoad != null) OnFinishedLoad(this);
            WindowOpen = true;

            sfClock = new Clock();

            while (WindowOpen)
            {
                SDL_GPU.GPU_ClearColor(Screen, Context.MakeColourFromRGBA(0, 0, 0, 255));

                GUIManager.Begin();

                OpenGLManager.BeforeFrame();

                if (OnGUI != null) OnGUI(this);
                ECSScene.OnGUIAll();

                if (ConsoleOpen)
                    DebugConsole.OnGUI();

                OpenGLManager.AfterGUI();

                SDL.SDL_Event evt;
                while (SDL.SDL_PollEvent(out evt) == 1)
                {
                    OpenGLManager.ImGuiHandleEvent(evt);

                    var noRepeat = ((EngineFlags & LuminalFlags.ENABLE_KEY_REPEAT) <= 0);

                    switch (evt.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            WinClose();
                            break;

                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            if (noRepeat & evt.key.repeat > 0) break;
                            var e = evt.key.keysym.scancode;
                            WinKeyDown(e);
                            break;

                        case SDL.SDL_EventType.SDL_KEYUP:
                            if (noRepeat & evt.key.repeat > 0) break;
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

                SDL_GPU.GPU_ResetRendererState();

                SDL.SDL_GL_SetSwapInterval(VSync switch
                {
                    VSyncMode.OFF => 0,
                    VSyncMode.ADAPTIVE => -1,
                    VSyncMode.SYNC => 1,
                    _ => 0
                });

                var t = sfClock.Restart();
                var seconds = t.AsSeconds(); // Should probably calculate this less often.
                Timing.DeltaTime = seconds;

                Timing.TotalElapsedTime += seconds;

                AnimationManager.Update(seconds);

                if (sceneManager.ActiveScene != null)
                    sceneManager.ActiveScene.Update(this, seconds);

                ECSScene.UpdateAll();

                if (OnUpdate != null)
                    OnUpdate(this, seconds);

                OpenGLManager.Update(seconds);

                if (OnEarlyDraw != null)
                    OnEarlyDraw(this);

                OpenGLManager.EarlyDraw();

                SDL_GPU.GPU_ResetRendererState();

                ECSScene.Render3DAll();

                if (OnDraw != null)
                    OnDraw(this);

                ECSScene.Render2DAll();

                if (sceneManager.ActiveScene != null)
                    sceneManager.ActiveScene.Draw(this);

                if (OnLateDraw != null)
                    OnLateDraw(this);

                GUIManager.RenderAll();

                // This moves deferred objects into the main object list.
                // This also cleans up objects that have been destroyed.
                //
                // It's important we do this late in the frame, because
                // moving deferred objects in the middle of the frame can
                // cause inconsistent behaviour with some methods seeing objects
                // that other methods do not.
                ECSScene.ProcessChangesToObjects();

                SDL_GPU.GPU_ResetRendererState();

                // This next line draws IMGUI. It's important that this comes after everything else, else debug UI won't be drawn!
                OpenGLManager.Draw();

                SDL_GPU.GPU_Flip(Screen);

                GUIManager.End();

                Timing.FrameNumber++;
                Timing.frameCount++;

                Timing.fpsCounter += seconds;
                if (Timing.fpsCounter >= 1.0f)
                {
                    Timing.fpsCounter = 0;
                    Timing.FrameRate = (float)Timing.frameCount;
                    Timing.frameCount = 0;
                }
            }
        }

        public static void Quit(int exitCode = 0)
        {
            AudioEngine.Instance.Dispose();

            WindowOpen = false;

            SDL_GPU.GPU_Quit();

            SDL.SDL_DestroyWindow(Window);

            Context.FreeAllImages();

            Environment.Exit(exitCode);
        }

        private void WinKeyDown(SDL.SDL_Scancode scancode)
        {
            if (OpenGLManager.DontPassKeyPresses) return;

            ConsoleManager.RunBind(scancode);

            if (KeyDown != null)
                KeyDown(this, scancode);

            if (sceneManager.ActiveScene != null)
                sceneManager.ActiveScene.OnKeyDown(this, scancode);
        }

        private void WinKeyUp(SDL.SDL_Scancode scancode)
        {
            if (OpenGLManager.DontPassKeyPresses) return;

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