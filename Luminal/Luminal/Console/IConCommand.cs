using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Console
{
    public interface IConCommand
    {
        void Run(Arguments a);
    }
}
