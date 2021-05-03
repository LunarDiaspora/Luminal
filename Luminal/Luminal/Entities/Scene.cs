using Luminal.Entities.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Entities
{
    public class Scene
    {
        public List<BaseObject> Objects = new();
        public List<PointLight3D> PointLights = new();
        public Camera3D Camera;

        public void SetActive()
        {
            ECSScene.L3D_SceneEnding();

            ECSScene.CurrentScene = this;

            foreach (var t in Objects)
            {
                foreach (var c in t.components)
                {
                    c.Create();
                }
            }

            ECSScene.ProcessChangesToObjects();
        }

        public static void Deactivate()
        {
            ECSScene.L3D_SceneEnding();

            ECSScene.CurrentScene = null;

            ECSScene.ProcessChangesToObjects();
        }
    }
}
