using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Luminal.Logging;
using Luminal.Assets.Complex;

namespace Luminal.Packages
{
    public class Package
    {
        public string ID;

        public string Name;
        public string Author;

        public string Root;

        internal void SetUp(string pathToPackageFile)
        {
            var fullPath = Path.GetFullPath(pathToPackageFile);
            var fullPkgRoot = Path.GetDirectoryName(fullPath);

            var packageFile = File.ReadAllText(fullPath);
            var packageDesc = JsonSerializer.Deserialize<PackageManifest>(packageFile);

            var fullAssets = Path.Combine(fullPkgRoot, packageDesc.AssetsRoot);

            Name = packageDesc.Name;
            Author = packageDesc.Author;
            Root = fullAssets;
            ID = packageDesc.ID;
        }

        internal T LoadDocumentFrom<T>(string path)
        {
            var text = File.ReadAllText(Path.Combine(Root, path));
            var doc = JsonSerializer.Deserialize<T>(text);
            return doc;
        }

        public FileStream OpenFile(string path)
        {
            var fullpath = Path.Combine(Root, path);
            if (!File.Exists(fullpath))
            {
                // This is a nothing (the file does not exist)
                Log.Error($"Meaningless asset path {ID}:{path}");
                return null;
            }
            return File.Open(fullpath, FileMode.Open);
        }

        public T LoadAsset<T>(string path)
        {
            var fullpath = Path.Combine(Root, path);
            if (Directory.Exists(fullpath))
            {
                if (File.Exists(Path.Combine(fullpath, "asset.assetmeta"))
                    && typeof(T).IsAssignableTo(typeof(ComplexAsset)))
                {
                    // This is a complex asset!
                    var complex = ComplexAssetLoader.Load(fullpath);
                    return (T)(object)complex;
                } else
                {
                    // This is a nothing
                    Log.Error($"Meaningless asset path {ID}:{path}");
                    return default(T);
                }
            }

            if (!File.Exists(fullpath))
            {
                // This is also a nothing (the file does not exist)
                Log.Error($"Meaningless asset path {ID}:{path}");
                return default(T);
            }

            if (typeof(T) == typeof(string))
            {
                var t = File.ReadAllText(fullpath);
                return (T)(object)t;
            }

            // This is a regular file
            var fullText = File.ReadAllText(fullpath);
            return JsonSerializer.Deserialize<T>(fullText);
        }
    }   
}
