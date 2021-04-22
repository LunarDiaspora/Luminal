using Luminal.Core;
using Luminal.OpenGL.ImGuiTheme;
using System;

namespace Luminal.Player.Core
{
    public class EnginePlayer
    {
        public Engine Engine;
        public bool Started = false;

        public void ConstructEngine()
        {
            Engine = new();
        }
        
        public void Start(int width, int height, string wintitle, LuminalFlags f = 0, IImGuiTheme theme = null)
        {
            if (Engine == null) ConstructEngine();

            Started = true;

            theme ??= new LuminalTheme();

            Engine.StartRenderer(width, height, wintitle, typeof(EnginePlayer), f, theme);
        }

        public static readonly EnginePlayer Instance = new();
    }
}
