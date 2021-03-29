// ImGui SDL2 binding with OpenGL
// In this binding, ImTextureID is used to store an OpenGL 'GLuint' texture identifier. Read the FAQ about ImTextureID in imgui.cpp.

// You can copy and use unmodified imgui_impl_* files in your project. See main.cpp for an example of using this.
// If you use this binding you'll need to call 4 functions: ImGui_ImplXXXX_Init(), ImGui_ImplXXXX_NewFrame(), ImGui::Render() and ImGui_ImplXXXX_Shutdown().
// If you are new to ImGui, see examples/README.txt and documentation at the top of imgui.cpp.
// https://github.com/ocornut/imgui

#define CIMGUI_DEFINE_ENUMS_AND_STRUCTS
#include <cimgui.h>

struct SDL_Window;
struct SDL_Surface;
typedef union SDL_Event SDL_Event;

#ifndef IMGUI_API
#define IMGUI_API __declspec(dllexport)
#endif

extern "C" {
	IMGUI_API bool _cdecl ImGui_ImplSdl_Init(ImGuiIO* io, SDL_Window* window, SDL_Surface* screen);
	IMGUI_API void _cdecl ImGui_ImplSdl_Shutdown(ImGuiIO* io);
	IMGUI_API void _cdecl ImGui_ImplSdl_PreNewFrame(ImGuiIO* io, SDL_Window* window);
	IMGUI_API bool _cdecl ImGui_ImplSdl_ProcessEvent(ImGuiIO* io, SDL_Event* event);
	IMGUI_API void _cdecl ImGui_ImplSdl_RenderDrawLists(ImGuiIO* io, ImDrawData* draw_data);

	// Use if you want to reset your rendering device without losing ImGui state.
	IMGUI_API void _cdecl ImGui_ImplSdl_InvalidateDeviceObjects(ImGuiIO* io);
	IMGUI_API bool _cdecl ImGui_ImplSdl_CreateDeviceObjects(ImGuiIO* io);
};