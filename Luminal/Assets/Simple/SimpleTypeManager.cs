using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Assets.Simple
{
    public class SimpleTypeManager
    {
        public static Dictionary<string, Type> AssetTypes = new();

        public static void Init()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var type in asm.GetTypes())
            {
                if (type.IsAssignableTo(typeof(SimpleAsset)) &&
                    type != typeof(SimpleAsset))
                {
                    var v = type.GetCustomAttributes(typeof(SimpleTypeAttribute), false);
                    if (v.Length != 1)
                    {
                        throw new Exception($"Add a [SimpleType()] to {type.Name}.");
                    }

                    var attr = (SimpleTypeAttribute)(v[0]);
                    AssetTypes[attr.TypeName] = type;

                    Log.Debug($"Registered {type.Name} for {attr.TypeName}");
                }
            }
        }
    }
}
