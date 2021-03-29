using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using ImGuiNET;
using System.Runtime.InteropServices;
using Luminal.Core;
using OpenGL;

namespace Luminal.IMGUI
{
    /**
     * 
     * This class handles interfacing with Dear IMGUI.
     * 
     */

    public class IMGUIManager
    {
        static bool Initialised = false;

        public static unsafe void Initialise()
        {
            //IntPtr Context = ImGui.CreateContext();

            //ImGui.SetCurrentContext(Context);
            //ImGui.GetIO().Fonts.AddFontDefault();

            //ImGui.GetIO().DisplaySize.X = Engine.Width;
            //ImGui.GetIO().DisplaySize.Y = Engine.Height;

            Initialised = true;
        }

        public static unsafe void BeforeFrame()
        {
            if (!Initialised) throw new Exception("Tried to call BeforeFrame before initialising.");

            //ImGui.NewFrame();
        }

        public static unsafe void Draw()
        {
            //ImGui.Render();

            //Gl.Viewport(0, 0, Engine.Width, Engine.Height);
        }
    }
}
