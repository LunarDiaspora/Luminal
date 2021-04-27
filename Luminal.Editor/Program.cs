using System;
using Luminal.Core;
using Luminal.Entities;
using Luminal.OpenGL.ImGuiTheme;
using Luminal.Player.Core;
using System.Runtime.InteropServices;
using Luminal.Logging;
using Luminal.Console;

namespace Luminal.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            ECSScene.Disable3D = true;

            EnginePlayer.Instance.ConstructEngine();

            EnginePlayer.Instance.Engine.OnFinishedLoad += _ => Editor.Init();

            var f = LuminalFlags.ENABLE_KEY_REPEAT | LuminalFlags.RESIZABLE | LuminalFlags.RESPECT_CONFIG_RESOLUTION;

            EnginePlayer.Instance.Start(1920, 1080, "Luminal Editor",
                f, new LuminalTheme());
        }
    }
}
