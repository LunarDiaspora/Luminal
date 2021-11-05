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

        struct ProjectJson
        {
            public string Name { get; set; }
            public string AssetsRoot { get; set; }
            public string PlayerConfig { get; set; }
        }

        public static LuminalProject LoadFromDisk(string path)
        {
            var t = File.ReadAllText(path);
            var splitRaw = t.Split('\n');
            var split = splitRaw.Select(e => e.Trim()).ToList();

            if (split[0] != (ProjectFileHeader + FormatVersion))
            {
                throw new Exception("Malformed project file!");
            }

            var k = JsonSerializer.Deserialize<ProjectJson>(split[1]);
            var directory = Path.GetDirectoryName(path);

            var prj = new LuminalProject()
            {
                Name = k.Name,
                AssetsRoot = k.AssetsRoot,
                PlayerConfig = k.PlayerConfig,
                AbsoluteAssetsRoot = Path.Combine(directory, k.AssetsRoot)
            };

            return prj;
        }
    }
}
