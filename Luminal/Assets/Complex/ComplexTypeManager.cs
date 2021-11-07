using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Assets.Complex
{
    public class ComplexTypeManager
    {
        public static Dictionary<string, Type> AssetTypes = new();

        public static void Init()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var type in asm.GetTypes())
            {
                if (type.IsAssignableTo(typeof(ComplexAsset)) &&
                    type != typeof(ComplexAsset))
                {
                    var v = type.GetCustomAttributes(typeof(ComplexTypeAttribute), false);
                    if (v.Length != 1)
                    {
                        throw new Exception($"Add a [ComplexType()] to {type.Name}.");
                    }

                    var attr = (ComplexTypeAttribute)(v[0]);
                    AssetTypes[attr.TypeName] = type;

                    Log.Debug($"Registered {type.Name} for {attr.TypeName}");
                }
            }
        }
    }
}
