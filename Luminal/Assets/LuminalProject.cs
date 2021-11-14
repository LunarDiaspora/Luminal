using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Luminal.Assets
{
    public class LuminalProject
    {
        public const string FormatVersion = "0.1.0";
        public const string ProjectFileHeader = "Luminal Engine Project File ";

        public string Name;
        public string AssetsRoot;
        public string AbsoluteAssetsRoot;
        public string PlayerConfig;
        public List<string> Dependencies;

        struct ProjectJson
        {
            public string Name { get; set; }
            public string AssetsRoot { get; set; }
            public string PlayerConfig { get; set; }
            public List<string> PackageDependencies { get; set; }
        }

        public static LuminalProject LoadFromDisk(string path)
        {
            var t = File.ReadAllText(path);
            var splitRaw = t.Split('\n');
            var split = splitRaw.Select(e => e.Trim()).ToArray();
            var rejoin = string.Join('\n', split[1..]);

            if (split[0] != (ProjectFileHeader + FormatVersion))
            {
                throw new Exception("Malformed project file!");
            }

            var k = JsonSerializer.Deserialize<ProjectJson>(rejoin);
            var directory = Path.GetDirectoryName(path);

            var prj = new LuminalProject()
            {
                Name = k.Name,
                AssetsRoot = k.AssetsRoot,
                PlayerConfig = k.PlayerConfig,
                AbsoluteAssetsRoot = Path.Combine(directory, k.AssetsRoot),
                Dependencies = k.PackageDependencies
            };

            return prj;
        }
    }
}
