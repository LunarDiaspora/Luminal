using Luminal.Core;
using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luminal.Packages
{
    public static class PackageGlobals
    {
        public static Dictionary<string, Package> LoadedPackages = new();

        internal static void AddAssets()
        {
            var pack = new Package();
            pack.Root = Engine.AbsoluteAssetPath;
            pack.Name = "Assets";
            pack.ID = "Assets";
            pack.Author = "n/a";
            LoadedPackages["Assets"] = pack;
        }

        public static void LoadPackage(string packageRoot)
        {
            var pkgFull = Path.GetFullPath(packageRoot);
            var pkgManifest = Path.Combine(pkgFull, "manifest.luminalpackage");
            var pack = new Package();
            pack.SetUp(pkgManifest);
            LoadedPackages[pack.ID] = pack;
            Log.Debug($"Loaded {pack.ID} (\"{pack.Name}\" by \"{pack.Author}\")");
        }
    }
}
