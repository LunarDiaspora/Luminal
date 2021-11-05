using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using OpenTK.Graphics.OpenGL;
using Luminal.Assets;

namespace Luminal.Core
{
    public static class Engine
    {
        public static LuminalProject LoadedProject;
        public static string AbsoluteAssetPath;

        public static void Start(string projectPath)
        {
            LoadedProject = LuminalProject.LoadFromDisk(projectPath);
            AbsoluteAssetPath = LoadedProject.AbsoluteAssetsRoot;
        }
    }
}
