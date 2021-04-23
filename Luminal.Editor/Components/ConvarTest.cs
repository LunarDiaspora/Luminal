using Luminal.Entities;
using ImGuiNET;
using Luminal.Console;

namespace Luminal.Editor.Components
{
    public class ConvarTest : Component3D
    {
        [ConVar("s_test", "A testing convar.")]
        public static string TestCV = "";

        public override void OnGUI()
        {
            ImGui.Begin("Do ConVars work yet?");

            ImGui.Text($"TestCV = {TestCV}");

            ImGui.End();
        }
    }
}
