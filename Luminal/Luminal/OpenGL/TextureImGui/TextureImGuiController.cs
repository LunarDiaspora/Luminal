using ImGuiNET;
using ImGuizmoNET;
using Luminal.Core;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SC = SDL2.SDL.SDL_Scancode;

namespace Luminal.OpenGL.TextureImGui
{
    public static unsafe class TextureImGuiController
    {
        public static GLRenderTexture Texture;

        public static System.Numerics.Vector2 Size = new(500, 500);
        public static System.Numerics.Vector2 ImGuiScale = System.Numerics.Vector2.One;

        public static IntPtr Context;

        private static int VtxBufSize = 2048;
        private static int IdxBufSize = 256;

        private static GLShader VS;
        private static GLShader FS;

        private static string VertexSource;
        private static string FragSource;

        private static GLShaderProgram ImGuiProgram;
        private static GLVertexArrayObject IG_VAO;
        private static int VertexBuffer;
        private static int IndexBuffer;
        private static GLTexture ImguiFontTexture;
        private static bool NewFrame = false;

        public delegate void GUICall();
        public static event GUICall OnGUI;

        public static void Init()
        {
            Texture = new GLRenderTexture();
            Texture.Size = Size;
            Texture.Use();

            Context = ImGui.CreateContext();

            ImGui.SetCurrentContext(Context);

            ImGuizmo.SetImGuiContext(Context);

            var io = ImGui.GetIO();

            io.DisplaySize.X = Size.X;
            io.DisplaySize.Y = Size.Y;

            io.KeyMap[(int)ImGuiKey.Tab] = (int)SC.SDL_SCANCODE_TAB;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)SC.SDL_SCANCODE_LEFT;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)SC.SDL_SCANCODE_RIGHT;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)SC.SDL_SCANCODE_UP;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)SC.SDL_SCANCODE_DOWN;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)SC.SDL_SCANCODE_PAGEUP;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)SC.SDL_SCANCODE_PAGEDOWN;
            io.KeyMap[(int)ImGuiKey.Home] = (int)SC.SDL_SCANCODE_HOME;
            io.KeyMap[(int)ImGuiKey.End] = (int)SC.SDL_SCANCODE_END;
            io.KeyMap[(int)ImGuiKey.Insert] = (int)SC.SDL_SCANCODE_INSERT;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)SC.SDL_SCANCODE_DELETE;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)SC.SDL_SCANCODE_BACKSPACE;
            io.KeyMap[(int)ImGuiKey.Space] = (int)SC.SDL_SCANCODE_SPACE;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)SC.SDL_SCANCODE_RETURN;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)SC.SDL_SCANCODE_ESCAPE;
            io.KeyMap[(int)ImGuiKey.KeyPadEnter] = (int)SC.SDL_SCANCODE_KP_ENTER;
            io.KeyMap[(int)ImGuiKey.A] = (int)SC.SDL_SCANCODE_A;
            io.KeyMap[(int)ImGuiKey.C] = (int)SC.SDL_SCANCODE_C;
            io.KeyMap[(int)ImGuiKey.V] = (int)SC.SDL_SCANCODE_V;
            io.KeyMap[(int)ImGuiKey.X] = (int)SC.SDL_SCANCODE_X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)SC.SDL_SCANCODE_Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)SC.SDL_SCANCODE_Z;

            ImGui.StyleColorsDark();

            VtxBufSize = 10000;
            IdxBufSize = 2000;

            IG_VAO = new("ImGui");

            IG_VAO.Bind();

            VertexSource = File.ReadAllText("EngineResources/Shaders/2D/2D.vert");
            FragSource = File.ReadAllText("EngineResources/Shaders/2D/2D.frag");

            VS = new GLShader(VertexSource, GLShaderType.Vertex);
            FS = new GLShader(FragSource, GLShaderType.Fragment);

            VS.Compile();
            FS.Compile();

            ImGuiProgram = new GLShaderProgram().Attach(VS).Attach(FS).Link();

            GLHelper.VertexBuffer("ImGui VBO", out VertexBuffer);
            GLHelper.ElementBuffer("ImGui EBO", out IndexBuffer);

            GL.NamedBufferData(VertexBuffer, VtxBufSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.NamedBufferData(IndexBuffer, IdxBufSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

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

            ImGuiCreateFontAtlas();
        }

        internal static void ImGuiCreateFontAtlas()
        {
            SetContext();
            ImGui.GetIO().Fonts.GetTexDataAsRGBA32(out IntPtr p, out int w, out int h, out int bpp);

            var fontTex = new GLTexture("ImGui Font Texture", w, h, p);
            fontTex.SetMagFilter(TextureMagFilter.Nearest);
            fontTex.SetMinFilter(TextureMinFilter.Nearest);

            ImGui.GetIO().Fonts.SetTexID((IntPtr)fontTex.GLObject);

            ImguiFontTexture = fontTex;

            ImGui.GetIO().Fonts.ClearTexData();
        }

        public static void SetContext()
        {
            ImGui.SetCurrentContext(Context);
            ImGuizmo.SetImGuiContext(Context);
        }

        private static unsafe void BeforeFrame()
        {
            SetContext();

            ImGui.NewFrame();
            ImGuizmo.BeginFrame();
            NewFrame = true;
        }

        private unsafe static void IGUpdate(float dt)
        {
            SetContext();
            var ioptr = ImGuiNative.igGetIO();

            Matrix4.CreateTranslation(0, 0, 0).

            ioptr->DeltaTime = dt;

            ioptr->DisplaySize = new System.Numerics.Vector2(
                    Size.X / ImGuiScale.X,
                    Size.Y / ImGuiScale.Y
                );

            ioptr->DisplayFramebufferScale = ImGuiScale;
        }

        public static void DoFrame(float dt)
        {
            SetContext();
            BeforeFrame();
            Texture.Size = Size;
            Texture.Use();

            IGUpdate(dt);

            OnGUI?.Invoke();

            IGRender();

            ImGui.SetCurrentContext(OpenGLManager.Context);
            ImGuizmo.SetImGuiContext(OpenGLManager.Context);
        }

        private static void IGRender()
        {
            if (NewFrame)
            {
                SetContext();
                NewFrame = false;
                ImGui.Render();
                var dd = ImGui.GetDrawData();
                IGRender2(dd);
            }
        }

        private static void IGRender2(ImDrawDataPtr dd)
        {
            SetContext();

            Texture.Size = Size;
            Texture.Use();
            GL.Viewport(0, 0, (int)Size.X, (int)Size.Y);

            if (dd.CmdListsCount == 0)
            {
                return;
            }

            GL.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            for (int i = 0; i < dd.CmdListsCount; i++)
            {
                ImDrawListPtr cmd_list = dd.CmdListsRange[i];

                int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if (vertexSize > VtxBufSize)
                {
                    int newSize = (int)Math.Max(VtxBufSize * 1.5f, vertexSize);
                    GL.NamedBufferData(VertexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    VtxBufSize = newSize;
                }

                int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
                if (indexSize > IdxBufSize)
                {
                    int newSize = (int)Math.Max(IdxBufSize * 1.5f, indexSize);
                    GL.NamedBufferData(IndexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    IdxBufSize = newSize;
                }
            }

            var io = ImGui.GetIO();
            var MVP = Matrix4.CreateOrthographicOffCenter(0.0f, Size.X, Size.Y, 0.0f, -1.0f, 1.0f);

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

            for (int n = 0; n < dd.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = dd.CmdListsRange[n];

                GL.NamedBufferSubData(VertexBuffer, IntPtr.Zero, cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmdList.VtxBuffer.Data);
                GL.NamedBufferSubData(IndexBuffer, IntPtr.Zero, cmdList.IdxBuffer.Size * sizeof(ushort), cmdList.IdxBuffer.Data);

                int vOffset = 0;
                int iOffset = 0;

                for (int i = 0; i < cmdList.CmdBuffer.Size; i++)
                {
                    ImDrawCmdPtr pcmd = cmdList.CmdBuffer[i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GLTexture.Active(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);

                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, (int)Size.Y - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

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

            Texture.AfterFrame();
            GLRenderTexture.Reset();
        }
    }
}
