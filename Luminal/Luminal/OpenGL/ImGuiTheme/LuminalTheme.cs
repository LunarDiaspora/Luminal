﻿using ImGuiNET;
using System.Numerics;

namespace Luminal.OpenGL.ImGuiTheme
{
    // Adapted from doukutsu-rs: https://github.com/doukutsu-rs/doukutsu-rs/blob/master/src/framework/ui.rs#L31-L104
    public class LuminalTheme : IImGuiTheme
    {
        public void InitTheme(ImGuiStylePtr style, ImGuiIOPtr io)
        {
            style.Colors[(int)ImGuiCol.Text] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.05f, 0.05f, 0.05f, 1f);
            style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.05f, 0.05f, 0.05f, 1f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.9f);
            style.Colors[(int)ImGuiCol.Border] = new Vector4(0.40f, 0.40f, 0.40f, 1.00f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(1.00f, 1.00f, 1.00f, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.80f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.84f, 0.37f, 0.00f, 0.20f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.84f, 0.37f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.06f, 0.06f, 0.06f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.08f, 0.08f, 0.08f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.06f, 0.06f, 0.06f, 0.40f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.14f, 0.14f, 0.14f, 0.40f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.31f, 0.31f, 0.31f, 0.30f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(1.00f, 1.00f, 1.00f, 0.30f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(1.00f, 1.00f, 1.00f, 0.50f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(1.00f, 1.00f, 1.00f, 0.50f);
            style.Colors[(int)ImGuiCol.Button] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.84f, 0.37f, 0.00f, 0.20f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.84f, 0.37f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.Header] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.84f, 0.37f, 0.00f, 0.20f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.84f, 0.37f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.50f, 0.50f, 0.43f, 0.50f);
            style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.75f, 0.45f, 0.10f, 0.78f);
            style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.75f, 0.45f, 0.10f, 1.00f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.98f, 0.65f, 0.26f, 0.25f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.98f, 0.65f, 0.26f, 0.67f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.98f, 0.65f, 0.26f, 0.95f);
            style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.17f, 0.10f, 0.04f, 0.94f);
            style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.84f, 0.37f, 0.00f, 0.60f);
            style.Colors[(int)ImGuiCol.TabActive] = new Vector4(0.67f, 0.30f, 0.00f, 0.68f);
            style.Colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.06f, 0.05f, 0.05f, 0.69f);
            style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.36f, 0.17f, 0.03f, 0.64f);
            style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.39f, 0.39f, 0.39f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.35f, 0.92f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.00f, 0.20f, 0.90f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.00f, 0.40f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.98f, 0.65f, 0.26f, 0.35f);
            style.Colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.00f, 0.00f, 1.00f, 0.90f);
            style.Colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.98f, 0.65f, 0.26f, 1.00f);
            style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(0.00f, 0.00f, 0.00f, 0.70f);
            style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.20f, 0.20f, 0.20f, 0.20f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.20f, 0.20f, 0.20f, 0.35f);

            style.WindowPadding = new Vector2(4f, 6f);
            style.FramePadding = new Vector2(8f, 6f);
            style.ItemSpacing = new Vector2(8f, 6f);
            style.ItemInnerSpacing = new Vector2(8f, 6f);
            style.IndentSpacing = 20f;

            style.ScrollbarSize = 20f;
            style.GrabMinSize = 8f;
            style.WindowBorderSize = 0f;
            style.ChildBorderSize = 0f;
            style.PopupBorderSize = 0f;
            style.FrameBorderSize = 0f;
            style.TabBorderSize = 0f;

            style.WindowRounding = 0f;
            style.ChildRounding = 0f;
            style.FrameRounding = 0f;
            style.PopupRounding = 0f;
            style.ScrollbarRounding = 0f;
            style.GrabRounding = 0f;
            style.TabRounding = 0f;

            style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
            style.WindowRounding = 0f;
            style.WindowMenuButtonPosition = ImGuiDir.Right;

            io.Fonts.AddFontDefault();
            //io.Fonts.AddFontFromFileTTF("./EngineResources/SourceCodePro.ttf", 18);

            io.ConfigWindowsMoveFromTitleBarOnly = true;
        }
    }
}