using System;
using Luminal.Core;
using Luminal.OpenGL.ImGuiTheme;

namespace Luminal.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Engine();

            engine.OnFinishedLoad += _ => Editor.Init();

            engine.StartRenderer(1920, 1080, "Luminal Editor", typeof(Editor),
                LuminalFlags.ENABLE_KEY_REPEAT, new LuminalTheme());
        }
    }
}
