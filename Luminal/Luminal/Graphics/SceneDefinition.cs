using System;
using System.Collections.Generic;
using System.Text;

namespace Luminal.Graphics
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneDefinition : Attribute
    {
        public string name;

        public SceneDefinition(string n)
        {
            name = n;
        }
    }
}
