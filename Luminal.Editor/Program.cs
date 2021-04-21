using System;
using Luminal.Core;
using Luminal.OpenGL.ImGuiTheme;
using Luminal.Player.Core;

namespace Luminal.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            EnginePlayer.Instance.Engine.OnFinishedLoad += _ => Editor.Init();

            EnginePlayer.Instance.Start(1920, 1080, "Luminal Editor",
                LuminalFlags.ENABLE_KEY_REPEAT, new LuminalTheme());
        }
    }
}
