using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using ImGuiNET;
using System.Runtime.InteropServices;
using Luminal.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.GL;
using Luminal.Logging;
using System.Drawing;

namespace Luminal.OpenGL
{
    /**
     * 
     * This class handles interfacing with Dear IMGUI.
     * 
     */

    public class IMGUIManager
    {
        static bool Initialised = false;

        public static IntPtr Context;

        static GLShader VS;
        static GLShader FS;

        static string VertexSource = "#version 330 core\n" +
            "layout (location=0) in vec3 aPos;\n"+
            "void main() {\n"+
            "gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);\n"+
            "}";
        static string FragSource = "#version 330 core\n" +
            "out vec4 FragColor;\n" +
            "void main() {\n" +
            "FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);\n" +
            "}";

        static GLShaderProgram Program;

        public static unsafe void Initialise()
        {
            GL.LoadBindings(new SDLBindingsContext());

            var maj = GL.GetInteger(GetPName.MajorVersion);
            var min = GL.GetInteger(GetPName.MinorVersion);

            Log.Info($"IMGUIManager: This appears to be OpenGL {maj}.{min}.");

            VS = new GLShader(VertexSource, GLShaderType.VERTEX);
            FS = new GLShader(FragSource, GLShaderType.FRAGMENT);

            VS.Compile();
            FS.Compile();

            Program = new GLShaderProgram().Attach(VS).Attach(FS).Link();

            Context = ImGui.CreateContext();

            ImGui.SetCurrentContext(Context);
            ImGui.GetIO().Fonts.AddFontDefault();

            ImGui.GetIO().DisplaySize.X = Engine.Width;
            ImGui.GetIO().DisplaySize.Y = Engine.Height;

            ImGui.GetIO().Fonts.GetTexDataAsRGBA32(out IntPtr p, out int w, out int h, out int bpp);

            var fontTex = new GLTexture("ImGui Font Texture", w, h, p);
            fontTex.SetMagFilter(TextureMagFilter.Nearest);
            fontTex.SetMinFilter(TextureMinFilter.Nearest);

            ImGui.GetIO().Fonts.SetTexID((IntPtr)fontTex.GLObject);

            ImGui.GetIO().Fonts.ClearTexData();

            Initialised = true;
        }

        public static unsafe void BeforeFrame()
        {
            if (!Initialised) throw new Exception("Tried to call BeforeFrame before initialising.");


            ImGui.NewFrame();
        }

        public static unsafe void Draw()
        {
            ImGui.Render();


        }

        //public static unsafe void Draw()
        //{
        //    //GL.ClearColor(Color.White);
        //    //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        //    ImGui.Render();

        //    var ddp = ImGui.GetDrawData().NativePtr;

        //    var dd = *ddp;

        //    var x = dd.DisplayPos.X;
        //    var y = dd.DisplayPos.Y;
        //    var r = x + dd.DisplaySize.X;
        //    var b = y + dd.DisplaySize.Y;

        //    var Framebuffer = new Core.Rectangle()
        //    {
        //        x = 0,
        //        y = 0,
        //        w = (dd.DisplaySize.X * dd.FramebufferScale.X),
        //        h = (dd.DisplaySize.Y * dd.FramebufferScale.Y)
        //    };

        //    if (Framebuffer.w == 0 || Framebuffer.h == 0)
        //    {
        //        return;
        //    }

        //    GL.Enable(EnableCap.Blend);
        //    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        //    GL.Disable(EnableCap.CullFace);
        //    GL.Disable(EnableCap.DepthTest);
        //    GL.Disable(EnableCap.StencilTest);
        //    GL.Disable(EnableCap.Lighting);
        //    GL.Disable(EnableCap.ColorMaterial);
        //    GL.Enable(EnableCap.Texture2D);

        //    GL.Viewport(0, 0, Engine.Width, Engine.Height);

        //    GL.MatrixMode(MatrixMode.Projection);
        //    GL.PushMatrix();

        //    GL.LoadIdentity();

        //    GL.Ortho(x, r, b, y, -1.0, 1.0);

        //    GL.MatrixMode(MatrixMode.Modelview);
        //    GL.PushMatrix();

        //    GL.LoadIdentity();

        //    var CLs = (ImDrawList**)dd.CmdLists;
        //    for (int i=0; i<dd.CmdListsCount; i++)
        //    {
        //        var cl = *CLs[i];

        //        var VertexBuffer = cl.VtxBuffer.Data;
        //        var IndexBuffer = cl.IdxBuffer.Data;

        //        uint io = 0;

        //        var vtxOffset = Marshal.OffsetOf(typeof(ImDrawVert), "pos");
        //        GL.VertexPointer(2, VertexPointerType.Float, sizeof(ImDrawVert), vtxOffset);

        //        var uvOffset = Marshal.OffsetOf(typeof(ImDrawVert), "uv");
        //        GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(ImDrawVert), uvOffset);

        //        var colorOffset = Marshal.OffsetOf(typeof(ImDrawVert), "col");
        //        GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(ImDrawVert), colorOffset);

        //        for (int j=0; j<cl.CmdBuffer.Size; j++)
        //        {
        //            var pcmd = ((ImDrawCmd*)cl.CmdBuffer.Data)[j];

        //            var clipoff = dd.DisplayPos;
        //            var clipscale = dd.FramebufferScale;

        //            var dr = new Vec4()
        //            {
        //                x = (pcmd.ClipRect.X - clipoff.X) * clipscale.X,
        //                y = (pcmd.ClipRect.Y - clipoff.Y) * clipscale.Y,
        //                z = (pcmd.ClipRect.Z - clipoff.X) * clipscale.X,
        //                w = (pcmd.ClipRect.W - clipoff.Y) * clipscale.Y,
        //            };

        //            if (dr.x < Framebuffer.w && dr.y < Framebuffer.h &&
        //                dr.z >= 0.0f && dr.w >= 0.0f)
        //            {
        //                GL.Scissor((int)dr.x, (int)(Framebuffer.h - dr.w), (int)(dr.z - dr.x), (int)(dr.w - dr.y));

        //                GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);

        //                // Fucking do it!
        //                var Pointer = IntPtr.Add(IndexBuffer, (int)io);
        //                GL.DrawElements(BeginMode.Triangles,
        //                                (int)pcmd.ElemCount,
        //                                DrawElementsType.UnsignedInt,
        //                                Pointer);
        //            }

        //            io += pcmd.ElemCount;
        //        }

        //    }

        //    // Destroy

        //    GL.MatrixMode(MatrixMode.Modelview);
        //    GL.PopMatrix();

        //    GL.MatrixMode(MatrixMode.Projection);
        //    GL.PopMatrix();

        //}
    }
}
