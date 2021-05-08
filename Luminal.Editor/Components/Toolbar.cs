using ImGuiNET;
using Luminal.Entities;
using Luminal.OpenGL;
using Luminal.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Editor.Components
{
    public enum MovementMode
    {
        Translate,
        Rotate
    }

    [Skip]
    public class Toolbar : Component3D
    {
        public static MovementMode ActiveMode = MovementMode.Translate;

        public static GLTexture TranslateIcon = new GLTexture("Editor: Translate Icon", "EngineResources/Images/translate.png");
        public static GLTexture RotateIcon = new GLTexture("Editor: Rotate Icon", "EngineResources/Images/rotate.png");

        public static float ButtonSize = 50.0f;

        public override void OnGUI()
        {
            ImGui.Begin("Toolbar", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar);

            if (ImGui.ImageButton(new IntPtr(TranslateIcon.GLObject), new(ButtonSize, ButtonSize)))
                ActiveMode = MovementMode.Translate;

            ImGui.SameLine();

            if (ImGui.ImageButton(new IntPtr(RotateIcon.GLObject), new(ButtonSize, ButtonSize)))
                ActiveMode = MovementMode.Rotate;

            ImGui.End();
        }
    }
}
