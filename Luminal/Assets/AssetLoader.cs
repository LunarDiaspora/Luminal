using Luminal.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Luminal.Assets
{
    public class AssetLoader
    {
        public static T LoadDocument<T>(string path)
        {
            var fullpath = Path.Combine(Engine.AbsoluteAssetPath, path);
            var file = File.ReadAllText(fullpath);
            var document = JsonSerializer.Deserialize<T>(file);
            return document;
        }
    }
}
