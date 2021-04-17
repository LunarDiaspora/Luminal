using Luminal.Entities;
using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Luminal.Graphics
{
    public class SceneManager
    {
        public Dictionary<string, Scene> Scenes;
        public Scene ActiveScene;

        public SceneManager(Type et)
        {
            Scenes = new Dictionary<string, Scene>();

            EnumerateAttributes(et);
        }

        private void EnumerateAttributes(Type et)
        {
            Assembly asm = Assembly.GetAssembly(et);

            foreach (var i in asm.DefinedTypes)
            {
                object[] attrs = i.GetCustomAttributes(typeof(SceneDefinition), false);
                // There should only be one scene tag to an object
                if (attrs.Length > 1) throw new ArgumentOutOfRangeException($"Class {i.Name} has {attrs.Length} SceneDefinitions and should only have 1!");
                if (attrs.Length > 0)
                {
                    SceneDefinition sd = (SceneDefinition)attrs[0];
                    Scene sc = (Scene)Activator.CreateInstance(i);
                    Log.Info($"Loading scene {sd.name} of type {sc.GetType().Name}");
                    Scenes.Add(sd.name, sc);
                }
            }
        }

        public void SwitchScene(string NewScene)
        {
            if (ActiveScene != null)
            {
                ActiveScene.OnExit();
            }

            if (!Scenes.ContainsKey(NewScene))
            {
                throw new ArgumentException($"Tried to switch to scene {NewScene}, but it does not exist.");
            }

            ECSScene.objects.Clear();

            Scene sc = Scenes[NewScene];
            ActiveScene = sc;
            sc.OnEnter();
        }
    }
}