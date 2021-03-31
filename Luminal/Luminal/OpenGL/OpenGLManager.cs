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
using OpenTK.Mathematics;
using Luminal.Logging;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace Luminal.OpenGL
{
    /**
     * 
     * This class handles interfacing with OpenGL and rendering 3D things.
     * 
     */

    public class OpenGLManager
    {
        static bool Initialised = false;

        public static IntPtr Context;

        static GLShader VS;
        static GLShader FS;

        static string VertexSource;
        static string FragSource;

        static GLShaderProgram ImGuiProgram;

        //static GLVertexArrayObject VAO = new();
        //static GLFloatBuffer VBO = new();
        //static GLUIntBuffer EBO = new();

        //static GLTexture Texture;

        static GLTexture ImguiFontTexture;

        static bool NewFrame = false;

        public static System.Numerics.Vector2 ImGuiScale = System.Numerics.Vector2.One;

        static GLVertexArrayObject IG_VAO;

        static int VtxBufSize;
        static int IdxBufSize;

        static int VertexBuffer; // Not using my abstractions here
        static int IndexBuffer;

        public static unsafe void Initialise()
        {
            GL.LoadBindings(new SDLBindingsContext());

            Context = ImGui.CreateContext();

            ImGui.SetCurrentContext(Context);

            var io = ImGui.GetIO();

            io.Fonts.AddFontDefault();

            io.DisplaySize.X = Engine.Width;
            io.DisplaySize.Y = Engine.Height;

            var maj = GL.GetInteger(GetPName.MajorVersion);
            var min = GL.GetInteger(GetPName.MinorVersion);

            Log.Info($"OpenGLManager: This appears to be OpenGL {maj}.{min}.");

            VtxBufSize = 10000;
            IdxBufSize = 2000;

            IG_VAO = new("ImGui");

            IG_VAO.Bind();

            VertexSource = File.ReadAllText("EngineResources/2D.vert");
            FragSource = File.ReadAllText("EngineResources/standard.frag");

            VS = new GLShader(VertexSource, GLShaderType.VERTEX);
            FS = new GLShader(FragSource, GLShaderType.FRAGMENT);

            VS.Compile();
            FS.Compile();

            ImGuiProgram = new GLShaderProgram().Attach(VS).Attach(FS).Link();

            GLHelper.VertexBuffer("ImGui VBO", out VertexBuffer);
            GLHelper.ElementBuffer("ImGui EBO", out IndexBuffer);

            GL.NamedBufferData(VertexBuffer, VtxBufSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.NamedBufferData(IndexBuffer, IdxBufSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            ImGuiCreateFontAtlas();

            var va = IG_VAO.GLObject;

            GL.VertexArrayVertexBuffer(va, 0, VertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
            GL.VertexArrayElementBuffer(va, IndexBuffer);

            GL.EnableVertexArrayAttrib(va, 0);
            GL.VertexArrayAttribBinding(va, 0, 0);
            GL.VertexArrayAttribFormat(va, 0, 2, VertexAttribType.Float, false, 0);

            GL.EnableVertexArrayAttrib(va, 1);
            GL.VertexArrayAttribBinding(va, 1, 0);
            GL.VertexArrayAttribFormat(va, 1, 2, VertexAttribType.Float, false, 8);

            GL.EnableVertexArrayAttrib(va, 2);
            GL.VertexArrayAttribBinding(va, 2, 0);
            GL.VertexArrayAttribFormat(va, 2, 4, VertexAttribType.UnsignedByte, true, 16);

            /*
            float[] vertices = // X Y Z R G B A U V
            {
                -0.5f, -0.5f, 0.5f,   1.0f, 1.0f, 1.0f, 1.0f,  0.0f, 1.0f, // BL
                0.5f, -0.5f, 0.5f,   1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f, // BR
                -0.5f, 0.5f, 0.5f,   1.0f, 1.0f, 1.0f, 1.0f,  0.0f, 0.0f, // TL
                0.5f, 0.5f, 0.5f,    1.0f, 1.0f, 1.0f, 1.0f,  1.0f, 0.0f  // TR
            };

            GL.Disable(EnableCap.CullFace);

            uint[] indices =
            {
                0, 1, 3,
                3, 2, 0
            };

            Texture = new GLTexture("boris", "boris.jpg");

            Texture.ActiveBind(TextureUnit.Texture0);

            VAO.Bind();

            EBO.Bind(BufferTarget.ElementArrayBuffer);
            EBO.Data(indices, BufferUsageHint.DynamicDraw);

            VBO.Bind(BufferTarget.ArrayBuffer);
            VBO.Data(vertices, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 7 * sizeof(float));

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            */

            Initialised = true;
        }

        public static unsafe void BeforeFrame()
        {
            if (!Initialised) throw new Exception("Tried to call BeforeFrame before initialising.");
            
            ImGui.NewFrame();
            NewFrame = true;
        }

        static void ImGuiCreateFontAtlas()
        {
            ImGui.GetIO().Fonts.GetTexDataAsRGBA32(out IntPtr p, out int w, out int h, out int bpp);

            var fontTex = new GLTexture("ImGui Font Texture", w, h, p);
            fontTex.SetMagFilter(TextureMagFilter.Nearest);
            fontTex.SetMinFilter(TextureMinFilter.Nearest);

            ImGui.GetIO().Fonts.SetTexID((IntPtr)fontTex.GLObject);

            ImguiFontTexture = fontTex;

            ImGui.GetIO().Fonts.ClearTexData();
        }

        static void IGRender()
        {
            if (NewFrame)
            {
                NewFrame = false;
                ImGui.Render();
                var dd = ImGui.GetDrawData();
                IGRender2(dd);
            }
        }

        static void IGRender2(ImDrawDataPtr dd)
        {
            if (dd.CmdListsCount == 0)
            {
                return;
            }

            for (int i = 0; i < dd.CmdListsCount; i++)
            {
                ImDrawListPtr cmd_list = dd.CmdListsRange[i];

                int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if (vertexSize > VtxBufSize)
                {
                    int newSize = (int)Math.Max(VtxBufSize * 1.5f, vertexSize);
                    GL.NamedBufferData(VertexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    VtxBufSize = newSize;

                    Log.Debug($"Resized dear imgui vertex buffer to new size {VtxBufSize}");
                }

                int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
                if (indexSize > IdxBufSize)
                {
                    int newSize = (int)Math.Max(IdxBufSize * 1.5f, indexSize);
                    GL.NamedBufferData(IndexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    IdxBufSize = newSize;

                    Log.Debug($"Resized dear imgui index buffer to new size {IdxBufSize}");
                }
            }

            var io = ImGui.GetIO();
            var MVP = Matrix4.CreateOrthographicOffCenter(0.0f, Engine.Width, Engine.Height, 0.0f, -1.0f, 1.0f);

            ImGuiProgram.Use();

            ImGuiProgram.UniformMatrix4("Projection", ref MVP);
            GL.Uniform1(ImGuiProgram.UniformLocation("Texture"), 0);

            IG_VAO.Bind();

            dd.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            for (int n=0; n<dd.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = dd.CmdListsRange[n];

                GL.NamedBufferSubData(VertexBuffer, IntPtr.Zero, cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmdList.VtxBuffer.Data);
                GL.NamedBufferSubData(IndexBuffer, IntPtr.Zero, cmdList.IdxBuffer.Size * sizeof(ushort), cmdList.IdxBuffer.Data);

                int vOffset = 0;
                int iOffset = 0;

                for (int i=0; i<cmdList.CmdBuffer.Size; i++)
                {
                    ImDrawCmdPtr pcmd = cmdList.CmdBuffer[i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    } else
                    {
                        GLTexture.Active(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);

                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, Engine.Height - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

                        if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                        {
                            GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(iOffset * sizeof(ushort)), vOffset);
                        }
                        else
                        {
                            GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                        }
                    }
                    iOffset += (int)pcmd.ElemCount;
                }

                vOffset += cmdList.VtxBuffer.Size;
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
        }

        static void IGUpdate(float dt)
        {
            var io = ImGui.GetIO();
            io.DeltaTime = dt;
            io.DisplaySize = new System.Numerics.Vector2(
                    Engine.Width / ImGuiScale.X,
                    Engine.Height / ImGuiScale.Y
                );
            io.DisplayFramebufferScale = ImGuiScale;
        }

        public static void Update(float dt)
        {
            ImGuiUpdateMouse();

            IGUpdate(dt);
        }

        static bool LMB_Down = false;
        static bool MMB_Down = false;
        static bool RMB_Down = false;

        public static unsafe void ImGuiHandleEvent(SDL.SDL_Event e)
        {
            // This function is mostly straight ported from IMGUI's SDL backend thing.

            var io = ImGui.GetIO();

            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    if (e.button.button == SDL.SDL_BUTTON_LEFT)
                        LMB_Down = true;
                    if (e.button.button == SDL.SDL_BUTTON_MIDDLE)
                        MMB_Down = true;
                    if (e.button.button == SDL.SDL_BUTTON_RIGHT)
                        RMB_Down = true;
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    if (e.button.button == SDL.SDL_BUTTON_LEFT)
                        LMB_Down = false;
                    if (e.button.button == SDL.SDL_BUTTON_MIDDLE)
                        MMB_Down = false;
                    if (e.button.button == SDL.SDL_BUTTON_RIGHT)
                        RMB_Down = false;
                    break;
                case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                    if (e.wheel.x > 0) io.MouseWheelH += 1;
                    if (e.wheel.x < 0) io.MouseWheelH -= 1;
                    if (e.wheel.y > 0) io.MouseWheel += 1;
                    if (e.wheel.y < 0) io.MouseWheel -= 1;
                    break;
                case SDL.SDL_EventType.SDL_TEXTINPUT:
                    io.AddInputCharactersUTF8(Marshal.PtrToStringUTF8((IntPtr)e.text.text));
                    break;
                case SDL.SDL_EventType.SDL_KEYDOWN:
                case SDL.SDL_EventType.SDL_KEYUP:
                    int key = (int)e.key.keysym.scancode;
                    io.KeysDown[key] = (e.type == SDL.SDL_EventType.SDL_KEYDOWN);
                    io.KeyShift = ((SDL.SDL_GetModState() & SDL.SDL_Keymod.KMOD_SHIFT) != 0);
                    io.KeyCtrl = ((SDL.SDL_GetModState() & SDL.SDL_Keymod.KMOD_CTRL) != 0);
                    io.KeyAlt = ((SDL.SDL_GetModState() & SDL.SDL_Keymod.KMOD_ALT) != 0);
                    break;
            }
        }

        public static void ImGuiUpdateMouse()
        {
            var io = ImGui.GetIO();

            uint mousestate = SDL.SDL_GetMouseState(out int x, out int y);
            io.MousePos = new(x, y);

            io.MouseDown[0] = LMB_Down || (mousestate & SDL.SDL_BUTTON(SDL.SDL_BUTTON_LEFT)) != 0;
            io.MouseDown[1] = MMB_Down || (mousestate & SDL.SDL_BUTTON(SDL.SDL_BUTTON_MIDDLE)) != 0;
            io.MouseDown[2] = RMB_Down || (mousestate & SDL.SDL_BUTTON(SDL.SDL_BUTTON_RIGHT)) != 0;

            LMB_Down = false;
            MMB_Down = false;
            RMB_Down = false;
        }

        public static void Gui()
        {
            ImGui.Begin("yay!");
            ImGui.Text("woo");
            ImGui.End();
        }
        
        public static unsafe void Draw()
        {
            IGRender();

            //ImGui.Render();

            //GL.ClearColor(Color.CadetBlue);
            //GL.Clear(ClearBufferMask.ColorBufferBit);

            //var p = new Point(0, 0);
            //var s = new Size(Engine.Width, Engine.Height);
            //GL.Viewport(p, s);

            /*
            Program.Use();

            Texture.ActiveBind(TextureUnit.Texture0);

            Program.Uniform2("ScreenSize", Engine.Width, Engine.Height);


            var Model = Matrix4.CreateRotationY(0.0f);
            var View = camera.View();
            var Projection = camera.Projection();

            //var mat = Matrix4.CreateRotationY((float)(4f / Math.PI));

            Program.UniformMatrix4("Model", ref Model);
            Program.UniformMatrix4("View", ref View);
            Program.UniformMatrix4("Projection", ref Projection);

            VAO.Bind();

            EBO.Bind(BufferTarget.ElementArrayBuffer);

            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 3);
            */
        }

        public static void KeyPress(SDL.SDL_Scancode s)
        {
            /*
            switch (s)
            {
                case SDL.SDL_Scancode.SDL_SCANCODE_S:
                    camera.Translate(camera.Forward * -0.1f);
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_W:
                    camera.Translate(camera.Forward * 0.1f);
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_A:
                    camera.Translate(camera.Right * 0.05f);
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_D:
                    camera.Translate(camera.Right * -0.05f);
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_LEFT:
                    camera.Rotate(new Vector3(0.0f, -2.0f, 0.0f));
                    break;
                case SDL.SDL_Scancode.SDL_SCANCODE_RIGHT:
                    camera.Rotate(new Vector3(0.0f, 2.0f, 0.0f));
                    break;
            }
            */
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
