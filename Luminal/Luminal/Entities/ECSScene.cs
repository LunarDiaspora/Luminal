using Luminal.Console;
using Luminal.Core;
using Luminal.Entities.Components;
using Luminal.OpenGL;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Luminal.Entities
{
    public class ECSScene
    {
        public static Scene CurrentScene;

        public static List<BaseObject> enabled = new();

        // Defer adding new objects until the frame ends.
        // This prevents an exception from being thrown.
        public static List<BaseObject> deferred = new();

        public static Camera3D Camera;

        public static GLRenderTexture RenderTexture;

        [ConVar("r_userendertexture", "Dictates use of the render texture. DO NOT TOUCH THIS IF YOU DON'T KNOW WHAT THIS DOES.")]
        public static bool UseRenderTexture = false;

        public static bool DisableTracking = false;

        [ConVar("r_disable3d", "Completely disables 3D rendering. Don't touch this unless you know what you're doing!")]
        public static bool Disable3D = false;

        public static void UpdateAll()
        {
            if (CurrentScene == null) return;

            foreach (var o in enabled)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.UpdateAlways();
                    if (Engine.Playing) c.Update();
                }
            }
        }

        public static void Render2DAll()
        {
            if (CurrentScene == null) return;

            foreach (var o in enabled)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.Render2D();
                }
            }
        }

        public static void OnGUIAll()
        {
            if (CurrentScene == null) return;

            foreach (var o in enabled)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.OnGUI();
                }
            }
        }

        public static void Render3DAll()
        {
            if (Disable3D) return; // Don't bother
            if (CurrentScene == null) return;

            BeforeDrawCalls();

            foreach (var o in enabled)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.EarlyRender3D();
                }
            }

            foreach (var o in enabled)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.Render3D();
                }
            }

            foreach (var o in enabled)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.LateRender3D();
                }
            }

            L3D_AfterFrame();
        }

        internal static void ExplicitRender()
        {
            if (Disable3D) return; // Don't bother
            if (CurrentScene == null) return;

            foreach (var o in enabled)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.Render3D();
                }
            }
        }

        public static void ProcessChangesToObjects(bool dontKill = false)
        {
            foreach (var o in deferred)
            {
                if (o.Destroying) continue;
                if (!dontKill) CurrentScene?.Objects.Add(o);
            }

            enabled = CurrentScene?.Objects.Where(e => !e.Destroying && e.Active).ToList();

            var dead = CurrentScene?.Objects.Where(e => e.Destroying).ToList();

            if (dead != null)
            {
                foreach (var o in dead)
                {
                    foreach (var c in o.components.Where(a => a.Enabled))
                    {
                        c.Destroy();
                    }
                    if (!dontKill) CurrentScene.Objects?.Remove(o);
                }
            }

            if (!dontKill) deferred.Clear();
        }

        public static void PushObject(BaseObject o)
        {
            CurrentScene?.Objects.Add(o);
        }

        public static GLShaderProgram Program;
        public static System.Numerics.Vector3 AmbientColour = new(0.7f, 0.7f, 0.7f);
        public static System.Numerics.Vector3 ObjectColour = new(1f, 1f, 1f);

        public static void L3D_SetUp()
        {
            RenderTexture = new();

            var vsSource = File.ReadAllText("EngineResources/Shaders/Rendering/mesh.vert");
            var fsSource = File.ReadAllText("EngineResources/Shaders/Rendering/lit.frag");

            var VS = new GLShader(vsSource, GLShaderType.Vertex);
            var FS = new GLShader(fsSource, GLShaderType.Fragment);

            Program = new GLShaderProgram()
                .Label("Scene")
                .Attach(VS)
                .Attach(FS)
                .Link();
        }

        public static void BeforeDrawCalls()
        {
            Program.Use();

            if (UseRenderTexture)
            {
                RenderTexture.Use();
            }

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Viewport(0, 0, Viewport.Width, Viewport.Height);

            GL.Enable(EnableCap.DepthTest);

            var proj = Camera.Projection();
            var view = Camera.View();

            Program.UniformMatrix4("View", ref view);
            Program.UniformMatrix4("Projection", ref proj);

            Program.Uniform3("AmbientColour", AmbientColour);
            Program.Uniform3("ObjectColour", ObjectColour);
            Program.Uniform3("ViewPosition", Camera.Parent.Position);

            Program.Uniform1i("BlinnPhong", RenderingVariables.UseBlinnPhong ? 1 : 0);
            Program.Uniform1("BlinnPhongMultiplier", RenderingVariables.BlinnPhongMult);

            // We actually -do- need the index here
            for (int i=0; i<CurrentScene.PointLights.Count; i++)
            {
                var light = CurrentScene.PointLights[i];
                Program.Uniform3($"Points[{i}].Position", light.Parent.Position);
                Program.Uniform3($"Points[{i}].Colour", light.Colour);
                Program.Uniform1($"Points[{i}].Intensity", light.Intensity);
                Program.Uniform1($"Points[{i}].Linear", light.Linear);
                Program.Uniform1($"Points[{i}].Quadratic", light.Quadratic);
            }

            for (int i=0; i<CurrentScene.SpotLights.Count; i++)
            {
                var light = CurrentScene.SpotLights[i];

                Program.Uniform3($"Spots[{i}].Position", light.Parent.Position);
                Program.Uniform3($"Spots[{i}].Colour", light.Colour);
                Program.Uniform3($"Spots[{i}].Angle", light.Parent.Forward);
                Program.Uniform1($"Spots[{i}].CutOff", System.MathF.Cos(GLHelper.DegRad(light.Radius)));
                Program.Uniform1($"Spots[{i}].OuterCutOff", System.MathF.Cos(GLHelper.DegRad(light.OuterRadius)));
                Program.Uniform1($"Spots[{i}].Intensity", light.Intensity);
                Program.Uniform1($"Spots[{i}].Linear", light.Linear);
                Program.Uniform1($"Spots[{i}].Quadratic", light.Quadratic);
            }

            Program.Uniform1i("PointCount", CurrentScene.PointLights.Count);
            Program.Uniform1i("SpotCount", CurrentScene.SpotLights.Count);

            Program.Uniform1i("aFullBright", RenderingVariables.FullBright ? 1 : 0);
        }

        public static void L3D_AfterFrame()
        {
            if (UseRenderTexture)
                RenderTexture.AfterFrame();

            GLRenderTexture.Reset();
        }

        public static void L3D_SceneEnding()
        {
            if (CurrentScene != null)
            {
                foreach (var o in CurrentScene.Objects)
                {
                    foreach (var c in o.components)
                    {
                        c.Destroy();
                    }
                }
            }

            CurrentScene = null;
        }
    }
}