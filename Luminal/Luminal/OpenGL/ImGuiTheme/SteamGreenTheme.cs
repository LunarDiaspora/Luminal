using ImGuiNET;
using System.Numerics;

namespace Luminal.OpenGL.ImGuiTheme
{
    // Based on a VGUI theme from the ImGUI issues and some modifications by Lewis.
    public class SteamGreenTheme : IImGuiTheme
    {
        public void InitTheme(ImGuiStylePtr style)
        {
            style.Colors[(int)ImGuiCol.Text] = new Vector4(0.82f, 0.81f, 0.80f, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.40f, 0.39f, 0.38f, 1.00f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.18f, 0.18f, 0.18f, 1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.22f, 0.21f, 0.21f, 1.00f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            style.Colors[(int)ImGuiCol.Border] = new Vector4(0.54f, 0.57f, 0.51f, 0.50f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.14f, 0.16f, 0.11f, 0.52f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.27f, 0.30f, 0.23f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.30f, 0.34f, 0.26f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.00f, 0.00f, 0.00f, 0.51f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.28f, 0.32f, 0.24f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.25f, 0.30f, 0.22f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.23f, 0.27f, 0.21f, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.54f, 0.57f, 0.51f, 0.50f);
            style.Colors[(int)ImGuiCol.Button] = new Vector4(0.29f, 0.34f, 0.26f, 0.40f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.54f, 0.57f, 0.51f, 0.50f);
            style.Colors[(int)ImGuiCol.Header] = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.35f, 0.42f, 0.31f, 0.6f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.54f, 0.57f, 0.51f, 0.50f);
            style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.14f, 0.16f, 0.11f, 1.00f);
            style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.54f, 0.57f, 0.51f, 1.00f);
            style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.19f, 0.23f, 0.18f, 0.00f); // grip invis
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.54f, 0.57f, 0.51f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.54f, 0.57f, 0.51f, 0.78f);
            style.Colors[(int)ImGuiCol.TabActive] = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            style.Colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            style.Colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(1.00f, 0.78f, 0.28f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            style.Colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.73f, 0.67f, 0.24f, 1.00f);
            style.Colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);

            style.FrameBorderSize = 0f;
            style.WindowRounding = 0f;
            style.ChildRounding = 0f;
            style.FrameRounding = 0f;
            style.PopupRounding = 0f;
            style.ScrollbarRounding = 0f;
            style.GrabRounding = 0f;
            style.TabRounding = 0f;

            style.WindowMenuButtonPosition = ImGuiDir.Right;
        }
    }
}