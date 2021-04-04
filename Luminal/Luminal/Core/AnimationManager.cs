using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Core
{
    public static class AnimationManager
    {
        public static float CurrentTime = 0.0f;

        public static void Update(float Delta)
        {
            CurrentTime += Delta;
        }
    }
}
