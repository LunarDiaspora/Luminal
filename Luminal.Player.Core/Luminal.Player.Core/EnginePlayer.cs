using Luminal.Core;
using Luminal.Entities;
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

        private static bool _hasPlayed = false;

        public static void Play()
        {
            Engine.Playing = true;
            _hasPlayed = true;

            if (ECSScene.CurrentScene != null)
            {
                foreach (var o in ECSScene.CurrentScene.Objects)
                {
                    o._Store();

                    foreach (var c in o.components)
                    {
                        c.Create();

                        c.BeginPlay();
                    }
                }
            }
        }

        public static void StopPlaying()
        {
            Engine.Playing = false;

            if (ECSScene.CurrentScene != null && _hasPlayed)
            {
                foreach (var o in ECSScene.CurrentScene.Objects)
                {
                    o._Load();

                    foreach (var c in o.components)
                    {
                        c.EndPlay();
                    }
                }
            }
        }

        public static readonly EnginePlayer Instance = new();
    }
}
