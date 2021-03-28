using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Logging;

namespace Luminal.GUI
{
    public static class GUIManager
    {
        public static List<GUIObject> ObjectsThisFrame = new();
        public static bool InFrame = false;

        // Called upon the start of a frame.
        // Sets up, and clears object buffer to avoid leaking memory.
        public static void Begin()
        {
            InFrame = true;
            ObjectsThisFrame.Clear();
        }

        // Called at the end of a frame.
        // Cleans up this frame.
        public static void End()
        {
            InFrame = false;
        }

        // Adds an object to the IMGUI render queue.
        public static void AppendObject(GUIObject obj)
        {
            if (InFrame)
            {
                ObjectsThisFrame.Add(obj);
            }
        }

        // Renders all objects. Typically called after user-defined rendering functions.
        public static void RenderAll()
        {
            if (InFrame)
            {
                foreach (GUIObject o in ObjectsThisFrame)
                {
                    o.OnRender();
                }
            }
        }

        public static int StartMouseX = 0;
        public static int StartMouseY = 0;
        public static bool MouseDown = false;

        public static GUIObject ObjectPressed;

        public static int RelativeX = 0;
        public static int RelativeY = 0;

        public static void OnMouseDown(int x, int y, byte btn)
        {
            if (btn == 1) // left mouse
            {
                MouseDown = true;
                StartMouseX = x;
                StartMouseY = y;

                ObjectPressed = ObjectsThisFrame.Find(t =>
                {
                    var rect = t.DraggableArea;
                    var dragw = rect.w == -1 ? t.Width : rect.w;
                    var dragh = rect.h == -1 ? t.Height : rect.h;
                    dragw += t.X + rect.x;
                    dragh += t.Y + rect.y;
                    return (x >= t.X + rect.x && y >= t.Y + rect.y) &&
                           (x <= dragw && y <= dragh);
                });

                if (ObjectPressed != null)
                {
                    RelativeX = x - ObjectPressed.X;
                    RelativeY = y - ObjectPressed.Y;
                }
            }
        }

        public static void OnMouseUp(int x, int y, byte btn)
        {
            if (btn == 1)
            {
                MouseDown = false;
                ObjectPressed = null;
            }
        }

        // Called when a mouse drag is performed.
        public static void OnMouseDrag(int x, int y, int rx, int ry)
        {
            int newx = (RelativeX + rx);
            int newy = (RelativeY + ry);
            if (ObjectPressed != null)
            {
                ObjectPressed.MoveRelative(rx, ry);
            }
        }
    }
}
