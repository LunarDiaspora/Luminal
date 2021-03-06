using Luminal.Entities.Screen;
using Luminal.Entities.World;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

#nullable enable

namespace Luminal.Entities
{
    public class BaseObject
    {
        public readonly List<Component> Components = new();
        internal readonly List<Component> deferredAdds = new();

        public string Name = "Object";

        public bool Active = true;

        [JsonIgnore]
        public bool Internal = false;

        internal bool Destroying = false;

        public Guid ID = new();

        public BaseObject()
        {
            if (!ECSScene.DisableTracking)
                ECSScene.PushObject(this);

            ID = Guid.NewGuid();
        }

        public BaseObject(string n)
        {
            Name = n;
            if (!ECSScene.DisableTracking)
                ECSScene.PushObject(this);

            ID = Guid.NewGuid();
        }

        public void Destroy()
        {
            Destroying = true;
        }

        public BaseObject Copy()
        {
            return (BaseObject)MemberwiseClone();
        }

        public virtual void _Store() { }
        public virtual void _Load() { }

        public T? GetComponent<T>() where T : Component
        {
            foreach (var c in Components)
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

            var theresAlreadyOne = Components.Any(j => j is T);
            if (theresAlreadyOne)
                throw new Exception($"There is already a component of type {typeof(T)}, but you are trying to add another. Don't do that!");

            var c = new T();

            if (c == null)
                throw new Exception($"CreateComponent<{typeof(T)}>(): The object constructed was null.");

            c._parent = this;

            deferredAdds.Add(c);

            c.Create();

            return c;
        }

        public T GetOrCreateComponent<T>() where T : Component, new()
        {
            var comp = GetComponent<T>();
            if (comp != null)
                return comp;
            return CreateComponent<T>();
        }

        internal void TickDeferred()
        {
            Components.AddRange(deferredAdds);
            deferredAdds.Clear();
        }
    }
}

#nullable restore