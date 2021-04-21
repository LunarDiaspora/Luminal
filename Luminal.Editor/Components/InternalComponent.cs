using Luminal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Editor.Components
{
    class InternalComponent : Component3D
    {
        public override void Create()
        {
            Parent.Internal = true;
        }
    }
}
