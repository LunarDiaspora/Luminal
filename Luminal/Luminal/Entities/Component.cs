using Luminal.Entities.Screen;
using Luminal.Entities.World;

namespace Luminal.Entities
{
    public class Component
    {
        public Component()
        {
        }

        protected internal BaseObject _parent;

        public bool Enabled = true;

        public virtual void Create()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Render2D()
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
        public Object3D Parent => (Object3D)_parent;
    }

    // Like a Component except it can only go on Object2Ds.
    public class Component2D : Component
    {
        public Object2D Parent => (Object2D)_parent;
    }
}