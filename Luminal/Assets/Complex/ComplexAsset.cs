using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Luminal.Assets.Complex
{
    public class ComplexAsset : AssetData
    {
        public virtual void OnConstruct(string rootPath, string metaFileContents) { }
    }
}
