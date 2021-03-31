using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.OpenGL.ImGuiTheme
{
    public interface IImGuiTheme
    {
        public void InitTheme(ImGuiNET.ImGuiStylePtr style);
    }
}