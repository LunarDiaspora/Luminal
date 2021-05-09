using Luminal.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Core
{
    public static class RenderingVariables
    {
        [ConVar("r_blinn", "Switches between the Blinn-Phong lighting model and the regular Phong model.")]
        public static bool UseBlinnPhong = true;

        [ConVar("r_blinn_multiplier", "Multiplier for Blinn-Phong specular shading.")]
        public static float BlinnPhongMult = 4.0f;
    }
}
