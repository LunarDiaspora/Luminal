using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.DebugUI
{
    public abstract class DebugWindow
    {
        public bool Open;
        public DebugWindow()
        {
            Open = true;
        }

        public abstract void Render();
    }
}
