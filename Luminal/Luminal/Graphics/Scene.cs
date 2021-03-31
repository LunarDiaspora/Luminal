using Luminal.Core;
using SDL2;

namespace Luminal.Graphics
{
    public class Scene
    {
        public virtual void Update(Engine main, float deltaTime)
        {
            // It's an update function.
        }

        public virtual void Draw(Engine main)
        {
            // Do things here. This is going to get overridden.
        }

        public virtual void OnEnter()
        {
            // Called when scene entered.
        }

        public virtual void OnExit()
        {
            // Called when scene exited.
        }

        public virtual void OnKeyDown(Engine main, SDL.SDL_Scancode sc)
        {
            // Called when key is pressed.
        }

        public virtual void OnKeyUp(Engine main, SDL.SDL_Scancode sc)
        {
            // Called when key is released.
        }
    }
}