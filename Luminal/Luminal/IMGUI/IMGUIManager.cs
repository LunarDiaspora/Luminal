using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using ImGuiNET;
using System.Runtime.InteropServices;
using Luminal.Core;

namespace Luminal.IMGUI
{
    /**
     * 
     * This class handles interfacing with Dear IMGUI.
     * 
     */

    public class IMGUIManager
    {
        [DllImport("ImguiGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int ImGui_ImplSdl_Init(IntPtr io, IntPtr window, IntPtr screen);

        [DllImport("ImguiGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int ImGui_ImplSdl_PreNewFrame(IntPtr io, IntPtr window);

        [DllImport("ImguiGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe int ImGui_ImplSdl_RenderDrawLists(ImGuiIO* io, ImDrawData* drawData);

        static bool Initialised = false;

        static IntPtr Texture;
        static SDL.SDL_Surface Surface;

        public static void Initialise()
        {
            IntPtr ImguiSurface = SDL.SDL_CreateRGBSurfaceWithFormat(0, Engine.Width, Engine.Height, 32, SDL.SDL_PIXELFORMAT_ARGB8888);
            Texture = SDL.SDL_CreateTexture(Engine.Renderer, SDL.SDL_PIXELFORMAT_ARGB8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, Engine.Width, Engine.Height);
            IntPtr Context = ImGui.CreateContext();
            Surface = Marshal.PtrToStructure<SDL.SDL_Surface>(ImguiSurface);

            ImGui.SetCurrentContext(Context);
            ImGui.GetIO().Fonts.AddFontDefault();

            ImGui.GetIO().DisplaySize.X = Engine.Width;
            ImGui.GetIO().DisplaySize.Y = Engine.Height;

            unsafe
            {
                ImGui_ImplSdl_Init((IntPtr)ImGui.GetIO().NativePtr, Engine.Window, ImguiSurface);
            }

            Initialised = true;
        }

        public static unsafe void BeforeFrame()
        {
            if (!Initialised) throw new Exception("Tried to call BeforeFrame before initialising.");
            ImGui_ImplSdl_PreNewFrame((IntPtr)ImGui.GetIO().NativePtr, Engine.Window);

            ImGui.NewFrame();
        }

        public static unsafe void Draw()
        {
            ImGui.Render();

            // Imgui native magic
            var dd = ImGui.GetDrawData();
            ImGui_ImplSdl_RenderDrawLists(ImGui.GetIO().NativePtr, dd.NativePtr);

            SDL.SDL_SetRenderDrawBlendMode(Engine.Renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            SDL.SDL_SetTextureBlendMode(Texture, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            // Draw it!
            SDL.SDL_UpdateTexture(Texture, IntPtr.Zero, Surface.pixels, Surface.pitch);
            SDL.SDL_RenderCopy(Engine.Renderer, Texture, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
