using Luminal.Core;

namespace Luminal.LGUI
{
    public abstract class GUIObject
    {
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;

        public abstract void OnRender();

        public Rectangle DraggableArea = new()
        {
            x = 0,
            y = 0,
            w = -1,
            h = -1
        };

        public void Move(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void MoveRelative(int x, int y)
        {
            X += x;
            Y += y;
        }
    }
}