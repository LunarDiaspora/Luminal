using Luminal.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Luminal.Packages;
using Luminal.Logging;

namespace Luminal.Assets
{
    public class Asset
    {
        public static FileStream OpenFile(string path)
        {
            var pathinfo = AssetPath.GetPath(path);
            if (!PackageGlobals.LoadedPackages.ContainsKey(pathinfo.Package))
            {
                Log.Error($"Unknown package {pathinfo.Package} in path {path}");
                return null;
            }
            var pkg = PackageGlobals.LoadedPackages[pathinfo.Package];
            return pkg.OpenFile(pathinfo.Asset);
        }

        public static T Load<T>(string path)
        {
            var pathinfo = AssetPath.GetPath(path);
            if (!PackageGlobals.LoadedPackages.ContainsKey(pathinfo.Package))
            {
                Log.Error($"Unknown package {pathinfo.Package} in path {path}");
                return default(T);
            }
            var pkg = PackageGlobals.LoadedPackages[pathinfo.Package];
            return pkg.LoadAsset<T>(pathinfo.Asset);
        }
    }
}
