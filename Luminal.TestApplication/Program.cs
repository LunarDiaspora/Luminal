using System;
using Luminal.Core;
using Luminal.Graphics;
using ImGuiNET;

namespace Luminal.TestApplication
{
    class Main
    {
        public Main()
        {
            var e = new Engine();

            e.OnDraw += Draw;
            e.OnGUI += GUI;

            e.StartRenderer(1280, 720, "Luminal Engine Test Application", typeof(Main), LuminalFlags.ENABLE_DEAR_IMGUI);
        }

        public void Draw(Engine _)
        {
            Context.SetColour(255, 0, 0, 255);
            Render.Rectangle(100, 100, 100, 100, RenderMode.FILL);
        }

        public void GUI(Engine _)
        {
            ImGui.Begin("poggers");

            ImGui.Text("yay");

            ImGui.End();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new Main();
        }
    }
}
