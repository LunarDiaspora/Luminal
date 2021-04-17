using Luminal.Entities.Screen;
using Luminal.Entities.World;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Luminal.Entities
{
    public class BaseObject
    {
        internal readonly List<Component> components = new();

        public BaseObject()
        {
            ECSScene.objects.Add(this);
        }

        public T? GetComponent<T>() where T : Component
        {
            foreach (var c in components)
            {
                if (c.GetType() == typeof(T))
                {
                    return (T)c;
                }
            }

            return null;
        }

        public T CreateComponent<T>() where T : Component, new()
        {
            if (typeof(T).IsAssignableTo(typeof(Component3D)) &&
                this is not Object3D)
            {
                throw new Exception($"Component type mismatch! (Tried to add a {typeof(T)} (Component3D) to an object that is not 3D.)");
            }

            if (typeof(T).IsAssignableTo(typeof(Component2D)) &&
                this is not Object2D)
            {
                throw new Exception($"Component type mismatch! (Tried to add a {typeof(T)} (Component2D) to an object that is not 2D.)");
            }

            var theresAlreadyOne = components.Any(j => j is T);
            if (theresAlreadyOne)
                throw new Exception($"There is already a component of type {typeof(T)}, but you are trying to add another. Don't do that!");

            var c = new T();

            if (c == null)
                throw new Exception($"CreateComponent<{typeof(T)}>(): The object constructed was null.");

            c._parent = this;

            components.Add(c);

            c.Create();

            return c;
        }
    }
}

#nullable restore