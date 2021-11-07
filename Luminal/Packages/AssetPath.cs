using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Packages
{
    public static class AssetPath
    {
        public struct AssetPathInfo
        {
            public string Package;
            public string Asset;
        }

        public static AssetPathInfo GetPath(string full)
        {
            var split = full.Split(':');
            var pkg = split[0];
            var restOfPath = string.Join(':', split[1..]);
            var info = new AssetPathInfo();
            info.Package = pkg;
            info.Asset = restOfPath; // temporary? TODO: improve
            return info;
        }
    }
}
