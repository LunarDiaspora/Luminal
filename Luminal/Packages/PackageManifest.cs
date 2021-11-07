using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Packages
{
    public struct PackageManifest
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string AssetsRoot { get; set; }
    }
}
