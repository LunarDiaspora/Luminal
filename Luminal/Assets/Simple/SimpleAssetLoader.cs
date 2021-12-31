using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Assets.Simple
{
    public class SimpleAssetLoader
    {
        internal static SimpleAsset Load(string path)
        {
            var fullpath = Path.GetFullPath(path);
            var type = Path.GetExtension(path)[1..]; // Remove the '.'
            
            if (!SimpleTypeManager.AssetTypes.ContainsKey(type))
            {
                Log.Error($"There is no simple type for type {type}.");
                return null;
            }

            var csType = SimpleTypeManager.AssetTypes[type];
            var inst = (SimpleAsset)Activator.CreateInstance(csType);

            inst.OnConstruct(fullpath);

            return inst;
        }
    }
}
