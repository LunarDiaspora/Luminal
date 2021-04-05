using ImGuiNET;
using Luminal.Core;
using Luminal.Logging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SDL2;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SC = SDL2.SDL.SDL_Scancode;

namespace Luminal.OpenGL
{
    /**
     *
     * This class handles interfacing with OpenGL and rendering 3D things.
     *
     */

    public struct GLVersion
    {
        public int Major;
        public int Minor;
        public string VersionString;
    }

    public unsafe class OpenGLManager
    {
        private static bool Initialised = false;

        public static IntPtr Context;

        private static GLShader VS;
        private static GLShader FS;

        private static string VertexSource;
        private static string FragSource;

        private static GLShaderProgram ImGuiProgram;

        public delegate void GLCallback();

        public static event GLCallback OnOpenGL;

        public static event GLCallback OnInitGL;

        public static event GLCallback OnEarlyOpenGL; // Called before SDL_gpu blits.

        //static GLVertexArrayObject VAO = new();
        //static GLFloatBuffer VBO = new();
        //static GLUIntBuffer EBO = new();

        //static GLTexture Texture;

        private static GLTexture ImguiFontTexture;

        private static bool NewFrame = false;

        public static System.Numerics.Vector2 ImGuiScale = System.Numerics.Vector2.One;

        private static GLVertexArrayObject IG_VAO;

        private static int VtxBufSize;
        private static int IdxBufSize;

        private static int VertexBuffer; // Not using my abstractions here
        private static int IndexBuffer;

        public static bool DontPassKeyPresses = false;

        public static GLVersion Version;

        private unsafe delegate void SetClipboardDelegate(void* a, char* b);

        private static unsafe void SetClipboard(void* _, char* text)
        {
            var str = Marshal.PtrToStringAnsi(new IntPtr(text));
            SDL.SDL_SetClipboardText(str);
        }

        private unsafe delegate char* GetClipboardDelegate(void* a);

        private static unsafe char* GetClipboard(void* _)
        {
            var ct = SDL.SDL_GetClipboardText();
            var ptr = Marshal.StringToHGlobalAnsi(ct);
            return (char*)ptr.ToPointer();
        }

        // Define these delegates to make sure the GC doesn't destroy them, which will crash the program
        private static GetClipboardDelegate _GetClipboard;
        private static SetClipboardDelegate _SetClipboard;

        public static unsafe void Initialise()
        {
            GL.LoadBindings(new SDLBindingsContext());

            Context = ImGui.CreateContext();

            ImGui.SetCurrentContext(Context);

            var io = ImGui.GetIO();

            io.DisplaySize.X = Engine.Width;
            io.DisplaySize.Y = Engine.Height;

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

            var maj = GL.GetInteger(GetPName.MajorVersion);
            var min = GL.GetInteger(GetPName.MinorVersion);

            Log.Info($"OpenGLManager: This appears to be OpenGL {maj}.{min}.");

            var t = new GLVersion()
            {
                Major = maj,
                Minor = min,
                VersionString = $"{maj}.{min}"
            };

            Version = t;

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

            _SetClipboard = SetClipboard;
            io.SetClipboardTextFn = Marshal.GetFunctionPointerForDelegate(_SetClipboard);

            _GetClipboard = GetClipboard;
            io.GetClipboardTextFn = Marshal.GetFunctionPointerForDelegate(_GetClipboard);


            Initialised = true;

            OnInitGL?.Invoke();
        }

        public static unsafe void BeforeFrame()
        {
            if (!Initialised) throw new Exception("Tried to call BeforeFrame before initialising.");

            ImGui.NewFrame();
            NewFrame = true;
        }

        internal static void ImGuiCreateFontAtlas()
        {
            ImGui.GetIO().Fonts.GetTexDataAsRGBA32(out IntPtr p, out int w, out int h, out int bpp);

            var fontTex = new GLTexture("ImGui Font Texture", w, h, p);
            fontTex.SetMagFilter(TextureMagFilter.Nearest);
            fontTex.SetMinFilter(TextureMinFilter.Nearest);

            ImGui.GetIO().Fonts.SetTexID((IntPtr)fontTex.GLObject);

            ImguiFontTexture = fontTex;

            ImGui.GetIO().Fonts.ClearTexData();
        }

        private static void IGRender()
        {
            if (NewFrame)
            {
                NewFrame = false;
                var dd = ImGui.GetDrawData();
                IGRender2(dd);
            }
        }

        private static void IGRender2(ImDrawDataPtr dd)
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

        private static void IGUpdate(float dt)
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

        public static void EarlyDraw()
        {
            OnEarlyOpenGL?.Invoke();
        }

        private static bool LMB_Down = false;
        private static bool MMB_Down = false;
        private static bool RMB_Down = false;

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
                    var t = Marshal.PtrToStringUTF8((IntPtr)e.text.text);
                    io.AddInputCharactersUTF8(t);
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

        private static bool PreviousCapState = false;

        public static void ImGuiUpdateMouse()
        {
            var io = ImGui.GetIO();

            uint mousestate = SDL.SDL_GetMouseState(out int x, out int y);
            io.MousePos = new(x, y);

            io.MouseDown[0] = LMB_Down || (mousestate & SDL.SDL_BUTTON(SDL.SDL_BUTTON_LEFT)) != 0;
            io.MouseDown[2] = MMB_Down || (mousestate & SDL.SDL_BUTTON(SDL.SDL_BUTTON_MIDDLE)) != 0;
            io.MouseDown[1] = RMB_Down || (mousestate & SDL.SDL_BUTTON(SDL.SDL_BUTTON_RIGHT)) != 0;

            LMB_Down = false;
            MMB_Down = false;
            RMB_Down = false;

            DontPassKeyPresses = io.WantCaptureKeyboard;

            if (PreviousCapState != io.WantTextInput)
            {
                if (io.WantTextInput)
                    SDL.SDL_StartTextInput();
                else
                    SDL.SDL_StopTextInput();
            }

            PreviousCapState = io.WantTextInput;
        }

        public static unsafe void Draw()
        {
            OnOpenGL?.Invoke();

            IGRender();
        }

        public static void AfterGUI()
        {
            ImGui.Render();
        }
    }
}