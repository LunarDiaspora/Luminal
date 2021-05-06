using Luminal.Core;
using Luminal.Graphics;

namespace Luminal.LGUI.Objects
{
    public class GUIWindow : GUIObject
    {
        public GUIWindow()
        {
            X = 0;
            Y = 0;
            Width = 100;
            Height = 100;
        }

        public override void OnRender()
        {
            Context.SetColour(30, 30, 30);
            Render.Rectangle(X, Y, Width, Height, RenderMode.Fill);

            Context.SetColour(77, 77, 77);
            Render.Rectangle(X, Y, Width, Height, RenderMode.Stroke);
        }
    }
}