using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SDL2;
using static SDL2.SDL;
using static SDL2.SDL.SDL_Scancode;
using System.Numerics;

/**
 *  ImGuiImplSDL2.cs
 *  An SDL2 interaction handler for Dear ImGui via ImGui.NET.
 *  
 *  Written by Rin, 2021
 *  
 *  github.com/ry00001
 *  twitter.com/lostkagamine
 *  
 *  Original C++ source:
 *  https://github.com/ocornut/imgui/blob/master/backends/imgui_impl_sdl.cpp
 *  
 *  You are allowed to embed this file and include it in your application.
 *  
 *  Usage:
 *  Call ImGuiImplSDL2.Init(IntPtr window) after creating your ImGui context and SDL window.
 *  
 *  _Before_ ImGui.NewFrame(), call ImGuiImplSDL2.NewFrame(), making sure
 *  that ImGui is configured correctly.
 *  
 *  Upon receiving an SDL2 event (via SDL_PollEvent), call
 *  ImGuiImplSDL2.ProcessEvent(SDL_event evt).
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 *  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 *  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 *  IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 *  CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 *  TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
 *  THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *  
 *  (!) PLEASE DO NOT REMOVE THIS HEADER.
 *  If you are making a modified version of this code, add your name above!
 */
namespace Luminal.Private
{
    public static class ImGuiImplSDL2
    {

        private unsafe delegate void SetClipboardFuncDelegate(void* a, char* b);
        private unsafe delegate char* GetClipboardFuncDelegate(void* a);

        private static unsafe void SetClipboardFunc(void* _, char* text)
        {
            var csStr = Marshal.PtrToStringAuto(new IntPtr(text));
            SDL_SetClipboardText(csStr);
        }

        private static unsafe char* GetClipboardFunc(void* _)
        {
            var csClip = SDL_GetClipboardText();
            var natClip = Marshal.StringToHGlobalAuto(csClip);
            return (char*)natClip.ToPointer();
        }

        // Define these delegates as a lifetime static variable
        // because otherwise the GC will get to them and clobber the pointers
        // causing a call into garbage code and crashing the program.
        private static GetClipboardFuncDelegate _GetClipboardFunc;
        private static SetClipboardFuncDelegate _SetClipboardFunc;

        // We don't need to use ImGui backend userdata
        // because this is C# and we have the power of statics.
        // Plus SDL2 isn't happy doing multiple viewports so I don't care

        private static bool WasMousePressedLeft;
        private static bool WasMousePressedRight;
        private static bool WasMousePressedMiddle;

        private static IntPtr Window;

        private static System.Diagnostics.Stopwatch Stopwatch;

        public static unsafe void Init(IntPtr window)
        {
            var io = ImGui.GetIO();

            var csName = "CSHARP-sdl2";
            var nativeName = Marshal.StringToHGlobalAnsi(csName);
            (io.NativePtr)->BackendPlatformName = (byte*)nativeName;

            io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;

            Window = window;

            io.KeyMap[(int)ImGuiKey.Tab] = (int)SDL_SCANCODE_TAB;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)SDL_SCANCODE_LEFT;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)SDL_SCANCODE_RIGHT;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)SDL_SCANCODE_UP;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)SDL_SCANCODE_DOWN;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)SDL_SCANCODE_PAGEUP;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)SDL_SCANCODE_PAGEDOWN;
            io.KeyMap[(int)ImGuiKey.Home] = (int)SDL_SCANCODE_HOME;
            io.KeyMap[(int)ImGuiKey.End] = (int)SDL_SCANCODE_END;
            io.KeyMap[(int)ImGuiKey.Insert] = (int)SDL_SCANCODE_INSERT;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)SDL_SCANCODE_DELETE;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)SDL_SCANCODE_BACKSPACE;
            io.KeyMap[(int)ImGuiKey.Space] = (int)SDL_SCANCODE_SPACE;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)SDL_SCANCODE_RETURN;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)SDL_SCANCODE_ESCAPE;
            io.KeyMap[(int)ImGuiKey.KeyPadEnter] = (int)SDL_SCANCODE_KP_ENTER;
            io.KeyMap[(int)ImGuiKey.A] = (int)SDL_SCANCODE_A;
            io.KeyMap[(int)ImGuiKey.C] = (int)SDL_SCANCODE_C;
            io.KeyMap[(int)ImGuiKey.V] = (int)SDL_SCANCODE_V;
            io.KeyMap[(int)ImGuiKey.X] = (int)SDL_SCANCODE_X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)SDL_SCANCODE_Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)SDL_SCANCODE_Z;

            _SetClipboardFunc = SetClipboardFunc;
            _GetClipboardFunc = GetClipboardFunc;

            io.SetClipboardTextFn = Marshal.GetFunctionPointerForDelegate(_SetClipboardFunc);
            io.GetClipboardTextFn = Marshal.GetFunctionPointerForDelegate(_GetClipboardFunc);

            // This is where code for io.ImeWindowHandle would go
            // but like, that's Windows-only and we don't have the
            // luxury of #ifdef _WIN32 here

            SDL_SetHint(SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");

            Stopwatch = new System.Diagnostics.Stopwatch();
            Stopwatch.Start();
        }

        public static unsafe void ProcessEvent(SDL_Event evt)
        {
            var io = ImGui.GetIO();

            switch (evt.type)
            {
                case SDL_EventType.SDL_MOUSEWHEEL:
                    // This is dirty
                    io.MouseWheelH += Math.Sign(evt.wheel.x);
                    io.MouseWheel += Math.Sign(evt.wheel.y);
                    return;
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    switch (evt.button.which)
                    {
                        case SDL_BUTTON_LEFT:
                            WasMousePressedLeft = true;
                            break;
                        case SDL_BUTTON_RIGHT:
                            WasMousePressedRight = true;
                            break;
                        case SDL_BUTTON_MIDDLE:
                            WasMousePressedMiddle = true;
                            break;
                    }
                    return;
                case SDL_EventType.SDL_TEXTINPUT:
                    var csText = Marshal.PtrToStringUTF8(new IntPtr(evt.text.text));
                    io.AddInputCharactersUTF8(csText);
                    return;
                case SDL_EventType.SDL_KEYDOWN:
                case SDL_EventType.SDL_KEYUP:
                    var key = evt.key.keysym.scancode;
                    io.KeysDown[(int)key] = (evt.type == SDL_EventType.SDL_KEYDOWN);

                    var modstate = SDL_GetModState();
                    io.KeyShift = (modstate & SDL_Keymod.KMOD_SHIFT) > 0;
                    io.KeyCtrl = (modstate & SDL_Keymod.KMOD_CTRL) > 0;
                    io.KeyAlt = (modstate & SDL_Keymod.KMOD_ALT) > 0;
                    io.KeySuper = (modstate & SDL_Keymod.KMOD_GUI) > 0;
                    return;
                    // ImGuiNET does not support focus events
            }
        }

        private static void UpdateMousePosAndButtons()
        {
            var io = ImGui.GetIO();

            var prevMousePos = io.MousePos;
            io.MousePos = new Vector2(0, 0);

            var mouseButtons = SDL_GetMouseState(out int mx, out int my);
            io.MouseDown[0] = WasMousePressedLeft || (mouseButtons & SDL_BUTTON(SDL_BUTTON_LEFT)) > 0;
            io.MouseDown[1] = WasMousePressedRight || (mouseButtons & SDL_BUTTON(SDL_BUTTON_RIGHT)) > 0;
            io.MouseDown[2] = WasMousePressedMiddle || (mouseButtons & SDL_BUTTON(SDL_BUTTON_MIDDLE)) > 0;
            WasMousePressedLeft = false;
            WasMousePressedMiddle = false;
            WasMousePressedRight = false;

            IntPtr? mouseWindow =
                ((SDL_GetWindowFlags(Window) & (uint)SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) > 0)
                ? Window : null;

            if (mouseWindow == null)
                return;

            if (io.WantSetMousePos)
                SDL_WarpMouseInWindow(Window, (int)prevMousePos.X, (int)prevMousePos.Y);

            io.MousePos.X = mx;
            io.MousePos.Y = my;
        }

        public static void NewFrame()
        {
            var io = ImGui.GetIO();

            // Set window size every frame
            // Accommodates window resizes

            SDL_GetWindowSize(Window, out int w, out int h);
            SDL_GL_GetDrawableSize(Window, out int dw, out int dh);

            io.DisplaySize.X = w;
            io.DisplaySize.Y = h;

            io.DisplayFramebufferScale.X = ((float)dw / w);
            io.DisplayFramebufferScale.Y = ((float)dh / h);

            var time = Stopwatch.Elapsed;
            var deltaSecs = (float)time.TotalSeconds;
            Stopwatch.Restart();
            io.DeltaTime = deltaSecs;

            UpdateMousePosAndButtons();
        }
    }
}
