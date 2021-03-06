using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.UI
{
    public abstract class BasePanel
    {
        public abstract void Render();
        public bool Open = true;
        public Guid ID;

        public void Close()
        {
            Open = false;
        }

        public BasePanel()
        {
            ID = Guid.NewGuid();
            PanelManager.DeferredAdd.Add(this);
        }
    }
}
