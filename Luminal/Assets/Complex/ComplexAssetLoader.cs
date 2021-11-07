using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luminal.Assets.Complex
{
    internal class ComplexAssetLoader
    {
        // A complex asset is, at its core, a directory

        internal struct AssetMetaStructure
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }

        internal static ComplexAsset Load(string root)
        {
            var fullRoot = Path.GetFullPath(root);
            var assetMeta = File.ReadAllText(Path.Combine(fullRoot, "asset.assetmeta"));
            var assetMetaStruct = JsonSerializer.Deserialize<AssetMetaStructure>(assetMeta);
            var type = assetMetaStruct.Type;

            if (!ComplexTypeManager.AssetTypes.ContainsKey(type))
            {
                Log.Error($"There is no complex asset for type {type}.");
                return null;
            }

            var csType = ComplexTypeManager.AssetTypes[type];
            var instance = (ComplexAsset)Activator.CreateInstance(csType);
            var metaFile = File.ReadAllText(Path.Combine(fullRoot, $"meta.{type}"));
            instance.OnConstruct(fullRoot, metaFile);

            return instance;
        }
    }
}
