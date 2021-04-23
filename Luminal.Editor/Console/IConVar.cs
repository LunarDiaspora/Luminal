using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Editor.Console
{
    public interface IConVar
    {
        void Set(string v);
        object Get();
    }
}
