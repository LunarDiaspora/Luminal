using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Luminal.Configuration
{
    public class LuminalConfigLoader
    {
        public static LuminalConfig LoadConfig(string Path)
        {
            var f = File.ReadAllText(Path);
            var js = JsonSerializer.Deserialize<LuminalConfig>(f);
            return js;
        }
    }
}
