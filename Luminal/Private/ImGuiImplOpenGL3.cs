using System;
using System.Runtime.InteropServices;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

/**
 *  ImGuiImplOpenGL3.cs
 *  An OpenGL 3.0 renderer for Dear ImGui via ImGui.NET.
 *  
 *  Written by Rin, 2021
 *  
 *  github.com/ry00001
 *  twitter.com/lostkagamine
 *  
 *  Original C++ source:
 *  https://github.com/ocornut/imgui/blob/v1.74/examples/imgui_impl_opengl3.cpp
 *  
 *  Usage:
 *  Call ImGuiImplOpenGL3.Init() after creating your ImGui context.
 *  
 *  After ImGui.Render(), call ImGuiImplOpenGL3.Render(), making sure
 *  that ImGui and OpenGL are configured correctly.
 *  
 *  (!) Please make sure to set ImGuiIO.DisplaySize correctly!
 *  (!) Leaving it at default WILL cause an error.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 *  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 *  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 *  IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 *  CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 *  TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
 *  THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *  
 *  (!) PLEASE DO NOT REMOVE THIS HEADER.
 *  If you are making a modified version of this code, add your name above!
 */
namespace Luminal.Private
{
    public class ImGuiImplOpenGL3
    {
        private static int Program;
        private static int VAO;

        private static int VBO;
        private static int EBO;

        private static int FontTexture;

        private const string VertexSource = @"#version 130

in vec2 position;
in vec2 uv;
in vec4 colour;

uniform mat4 Projection;
out vec2 frag_uv;
out vec4 frag_colour;

void main()
{
    frag_uv = uv;
    frag_colour = colour;
    gl_Position = Projection * vec4(position.xy, 0, 1);
}";

        private const string FragmentSource = @"#version 130

in vec2 frag_uv;
in vec4 frag_colour;

uniform sampler2D Texture;

out vec4 outColour;

void main()
{
    outColour = frag_colour * texture(Texture, frag_uv.xy);
}";

        private static void CheckShader(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int ok);
            if (ok != 1)
            {
                var infolog = GL.GetShaderInfoLog(shader);
                Console.WriteLine("ImGuiImplOpenGL3: --- SHADER COMPILE ERROR ---");
                Console.WriteLine(infolog.Trim());
                throw new Exception("ImGuiImplOpenGL3: SHADER FAILED TO COMPILE. This is a bug! " +
                                    "Details have been printed to wherever Console.WriteLine goes. " +
                                    "Please report this crash to the maintainer!");
            }
        }

        public static unsafe void Init()
        {
            var vert = GL.CreateShader(ShaderType.VertexShader);
            var frag = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vert, VertexSource);
            GL.CompileShader(vert);
            CheckShader(vert);

            GL.ShaderSource(frag, FragmentSource);
            GL.CompileShader(frag);
            CheckShader(frag);

            Program = GL.CreateProgram();
            GL.AttachShader(Program, vert);
            GL.AttachShader(Program, frag);
            GL.LinkProgram(Program);

            VAO = GL.GenVertexArray();

            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            var io = ImGui.GetIO();
            var glVerMaj = GL.GetInteger(GetPName.MajorVersion);
            var glVerMin = GL.GetInteger(GetPName.MinorVersion);
            var verInt = (glVerMaj * 100) + (glVerMin * 10);
            var vendor = GL.GetString(StringName.Vendor);
            var csName = $"CSHARP-opengl3 GL/{verInt} GPU/{vendor}";
            var backendRendererName = Marshal.StringToHGlobalAnsi(csName);
            (io.NativePtr)->BackendRendererName = (byte*)backendRendererName;

#if !_IMGUI_IMPL_OPENGL3_I_KNOW_WHAT_IM_DOING
            if (io.DisplaySize.X <= 0 ||
                io.DisplaySize.Y <= 0)
            {
                var w = io.DisplaySize.X;
                var h = io.DisplaySize.Y;

                /**********************************************************************\
                |   Hi! If you're here, you're probably making a mistake.              |
                |   Commenting out this error _will_ break things!                     |
                |   Instead, please set ImGuiIO display size like this:                |
                |                                                                      |
                |   var io = ImGui.GetIO();                                            |
                |   io.DisplaySize.X = YourWindowWidth;                                |
                |   io.DisplaySize.Y = YourWindowHeight;                               |
                |                                                                      |
                |   If you have a very, very good reason for                           |
                |   needing a zero-dimension display, please add:                      |
                |                                                                      |
                |   #define _IMGUI_IMPL_OPENGL3_I_KNOW_WHAT_IM_DOING                   |
                |                                                                      |
                |   to the _very top_ of this file. (Before `using System;`.)          |
                |   The C# preprocessor will complain if you put it anywhere else.     |
                |                                                                      |
                |   Please don't file issues about this specific error.                |
                \**********************************************************************/

                throw new Exception(
                     "ImGuiImplOpenGL3: Display width or height is less than or equal to zero. " +
                    $"(W: {w}, H: {h}) " +
                     "A common reason for this error is forgetting to set DisplaySize " +
                     "in your ImGuiIO to the size of the viewport to render in. " +
                     "If you have a valid reason for a zero-area display, please " +
                     "see the source code, in ImGuiImplOpenGL3.cs Init().");
            }
#endif

            CreateFontTexture();
        }

        private static unsafe void SetupRenderState(ImDrawDataPtr drawData, int fbWidth, int fbHeight)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);

            GL.Viewport(0, 0, fbWidth, fbHeight);

            GL.BindVertexArray(VAO);

            var io = ImGui.GetIO();

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            var left = drawData.DisplayPos.X;
            var right = drawData.DisplayPos.X + drawData.DisplaySize.X;
            var top = drawData.DisplayPos.Y;
            var bottom = drawData.DisplayPos.Y + drawData.DisplaySize.Y;
            var matrix = Matrix4.CreateOrthographicOffCenter(
                left,
                right,
                bottom,
                top,
                -1.0f,
                1.0f);

            GL.UseProgram(Program);

            var projLocation = GL.GetUniformLocation(Program, "Projection");
            GL.UniformMatrix4(projLocation, false, ref matrix);
            var textureLocation = GL.GetUniformLocation(Program, "Texture");
            GL.Uniform1(textureLocation, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            var attribLocationVtxPos = GL.GetAttribLocation(Program, "position");
            var attribLocationVtxUv = GL.GetAttribLocation(Program, "uv");
            var attribLocationVtxCol = GL.GetAttribLocation(Program, "colour");
            GL.EnableVertexAttribArray(attribLocationVtxPos);
            GL.EnableVertexAttribArray(attribLocationVtxUv);
            GL.EnableVertexAttribArray(attribLocationVtxCol);

            GL.VertexAttribPointer(attribLocationVtxPos, 2, VertexAttribPointerType.Float,
                false, sizeof(ImDrawVert), Marshal.OffsetOf<ImDrawVert>("pos"));
            GL.VertexAttribPointer(attribLocationVtxUv, 2, VertexAttribPointerType.Float,
                false, sizeof(ImDrawVert), Marshal.OffsetOf<ImDrawVert>("uv"));
            GL.VertexAttribPointer(attribLocationVtxCol, 4, VertexAttribPointerType.UnsignedByte,
                true, sizeof(ImDrawVert), Marshal.OffsetOf<ImDrawVert>("col"));
        }

        private static unsafe void CreateFontTexture()
        {
            var io = ImGui.GetIO();

            byte* pixels;
            int tw, th;

            io.Fonts.GetTexDataAsRGBA32(out pixels, out tw, out th);

            FontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, FontTexture);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMinFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba,
                tw, th, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, new IntPtr(pixels));

            io.Fonts.SetTexID(new IntPtr(FontTexture));
        }

        public static unsafe void Render()
        {
            // DRAW IMGUI

            var drawData = ImGui.GetDrawData();
            var fbWidth = (int)(drawData.DisplaySize.X * drawData.FramebufferScale.X);
            var fbHeight = (int)(drawData.DisplaySize.Y * drawData.FramebufferScale.Y);

            if (fbWidth <= 0 || fbHeight <= 0)
            {
                return;
            }

            SetupRenderState(drawData, fbWidth, fbHeight);

            var clipOff = drawData.DisplayPos;
            var clipScale = drawData.FramebufferScale;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                GL.BufferData(BufferTarget.ArrayBuffer,
                    cmdList.VtxBuffer.Size * sizeof(ImDrawVert),
                    cmdList.VtxBuffer.Data,
                    BufferUsageHint.StreamDraw);

                GL.BufferData(BufferTarget.ElementArrayBuffer,
                    cmdList.IdxBuffer.Size * sizeof(ushort),
                    cmdList.IdxBuffer.Data,
                    BufferUsageHint.StreamDraw);

                for (int i = 0; i < cmdList.CmdBuffer.Size; i++)
                {
                    var pcmd = cmdList.CmdBuffer[i];

                    var clipRect = new Vector4();
                    clipRect.X = (pcmd.ClipRect.X - clipOff.X) * clipScale.X;
                    clipRect.Y = (pcmd.ClipRect.Y - clipOff.Y) * clipScale.Y;
                    clipRect.Z = (pcmd.ClipRect.Z - clipOff.X) * clipScale.X;
                    clipRect.W = (pcmd.ClipRect.W - clipOff.Y) * clipScale.Y;

                    if (clipRect.X < fbWidth &&
                        clipRect.Y < fbHeight &&
                        clipRect.Z >= 0.0f &&
                        clipRect.W >= 0.0f)
                    {
                        var scissorRect = new Vector4();
                        scissorRect.X = clipRect.X;
                        scissorRect.Y = (fbHeight - clipRect.W);
                        scissorRect.Z = (clipRect.Z - clipRect.X);
                        scissorRect.W = (clipRect.W - clipRect.Y);

                        GL.Scissor((int)scissorRect.X,
                            (int)scissorRect.Y,
                            (int)scissorRect.Z,
                            (int)scissorRect.W);

                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D,
                            pcmd.TextureId.ToInt32());

                        var t = (int)(pcmd.IdxOffset * sizeof(ushort));

                        GL.DrawElementsBaseVertex(PrimitiveType.Triangles,
                            (int)pcmd.ElemCount,
                            DrawElementsType.UnsignedShort,
                            new IntPtr(t),
                            (int)pcmd.VtxOffset);
                    }
                }

            }

            GL.Disable(EnableCap.ScissorTest);
            GL.Disable(EnableCap.Blend);
        }
    }
}
