using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Reflection
{
    /*
     * Indicates that a field SHOULD NOT appear in the Inspector.
     */
    [AttributeUsage(AttributeTargets.Field)]
    public class SkipAttribute : Attribute
    {
    }

    /*
     * Indicates that a Vector3 or a Vector4 should be treated as a colour.
     */
    [AttributeUsage(AttributeTargets.Field)]
    public class ColourAttribute : Attribute
    {
    }
}
