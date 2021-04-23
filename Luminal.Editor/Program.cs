using System;
using Luminal.Core;
using Luminal.Entities;
using Luminal.OpenGL.ImGuiTheme;
using Luminal.Player.Core;
using System.Runtime.InteropServices;
using Luminal.Logging;
using Luminal.Editor.Console;

namespace Luminal.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            ECSScene.Disable3D = true;

            EnginePlayer.Instance.ConstructEngine();

            Log.SetLogger(new EditorLogger());

            ConsoleManager.FindConCommands();
            ConsoleManager.FindConVars();

            EnginePlayer.Instance.Engine.OnFinishedLoad += _ => Editor.Init();

            EnginePlayer.Instance.Start(1920, 1080, "Luminal Editor",
                LuminalFlags.ENABLE_KEY_REPEAT, new LuminalTheme());
        }
    }
}
