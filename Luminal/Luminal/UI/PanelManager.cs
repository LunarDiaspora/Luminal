using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.UI
{
    public class PanelManager
    {
        public static List<BasePanel> ActivePanels = new();

        public static List<BasePanel> DeferredAdd = new();

        public static void RenderAllActivePanels()
        {
            foreach (var p in ActivePanels.Where(e => e.Open))
                p.Render();

            foreach (var t in DeferredAdd)
            {
                ActivePanels.Add(t);
            }

            ActivePanels = ActivePanels.Where(e => e.Open).ToList();

            DeferredAdd.Clear(); // PLEASE remember to clear this.
        }
    }
}
