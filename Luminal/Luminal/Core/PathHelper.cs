using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Core
{
    public static class PathHelper
    {
        public static List<string> GetAllSteps(string dir)
        {
            var o = new List<string>();

            var di = new DirectoryInfo(dir);

            while (di.Parent != null)
            {
                o.Add(di.FullName);
                di = di.Parent;
            }

            return o;
        }
    }
}
