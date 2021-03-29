﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.LGUI.Objects;

namespace Luminal.LGUI
{
    /*
     * An interface with Luminal's IMGUI framework.
     * 
     * (C) Lunar Diaspora 2021
     * 
     */
    
    public class IMGUI
    {
        public delegate void RenderDirective(GUIObject obj);

        public static void Window(string Title, RenderDirective rd)
        {
            // i'll fix this later
        }

        public static void Register(GUIObject obj)
        {
            GUIManager.AppendObject(obj);
        }
    }
}