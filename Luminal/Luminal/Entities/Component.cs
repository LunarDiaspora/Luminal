using Luminal.Entities.Screen;
using Luminal.Entities.World;
using System;
using Newtonsoft.Json;
using Luminal.Reflection;

namespace Luminal.Entities
{
    public class Component
    {
        public Component()
        {
            Type = GetType().Name;

            ID = Guid.NewGuid();
        }

        public Component Copy()
        {
            return (Component)MemberwiseClone();
        }

        [JsonIgnore]
        protected internal BaseObject _parent;

        [Skip]
        public bool Enabled = true;

        [Skip]
        public string Type;

        [Skip]
        public Guid ID = new();

        public virtual void Create()
        {
        }

        public virtual void BeginPlay()
        {
        }

        public virtual void EndPlay()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Render2D()
        {
        }

        public virtual void EarlyRender3D()
        {
        }

        public virtual void LateRender3D()
        {
        }

        public virtual void Render3D()
        {
        }

        public virtual void OnGUI()
        {
        }

        public virtual void Destroy()
        {
        }
    }

    // Like a Component except it can only go on Object3Ds.
    public class Component3D : Component
    {
        [JsonIgnore]
        public Object3D Parent => (Object3D)_parent;
    }

    // Like a Component except it can only go on Object2Ds.
    public class Component2D : Component
    {
        [JsonIgnore]
        public Object2D Parent => (Object2D)_parent;
    }
}