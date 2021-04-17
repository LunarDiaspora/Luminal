using Luminal.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Entities.Components
{
    public class ImageRenderer : Component2D
    {
        private Image i;

        public void LoadImage(Image a)
        {
            i = a;
        }

        public void LoadImage(string path)
        {
            if (!Image.LoadFrom(path, out Image j))
            {
                throw new Exception($"ImageRenderer: Failed to load image. (path = {path})");
            }
            i = j;
        }

        public override void Render2D()
        {
            if (i != null)
                i.Draw(Parent.Position.X, Parent.Position.Y);
        }
    }
}
