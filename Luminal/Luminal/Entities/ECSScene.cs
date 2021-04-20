using Luminal.Core;
using Luminal.Entities.Components;
using Luminal.OpenGL;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Luminal.Entities
{
    public class ECSScene
    {
        internal static List<BaseObject> objects = new();

        public static IEnumerable<BaseObject> enabled = objects.Where(e => !e.Destroying && e.Active);

        // Defer adding new objects until the frame ends.
        // This prevents an exception from being thrown.
        public static List<BaseObject> deferred = new();

        public static Camera3D Camera;

        public static List<PointLight3D> PointLights = new();

        public static GLRenderTexture RenderTexture;

        public static void UpdateAll()
        {
            foreach (var o in enabled)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.Update();
                }
            }
        }

        public static void Render2DAll()
        {
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
            L3D_BeforeFrame();

            foreach (var o in enabled)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.Render3D();
                }
            }

            L3D_AfterFrame();
        }

        public static void ProcessChangesToObjects()
        {
            foreach (var o in deferred)
            {
                if (o.Destroying) continue;
                objects.Add(o);
            }

            enabled = objects.Where(e => !e.Destroying && e.Active);

            var dead = objects.Where(e => e.Destroying).ToList();

            foreach (var o in dead)
            {
                foreach (var c in o.components.Where(a => a.Enabled))
                {
                    c.Destroy();
                }
                objects.Remove(o);
            }

            deferred.Clear();
        }

        public static void PushObject(BaseObject o)
        {
            deferred.Add(o);
        }

        public static GLShaderProgram Program;
        public static System.Numerics.Vector3 AmbientColour = new(0.7f, 0.7f, 0.7f);
        public static System.Numerics.Vector3 ObjectColour = new(1f, 1f, 1f);

        public static void L3D_SetUp()
        {
            var vsSource = File.ReadAllText("EngineResources/mesh.vert");

            // TODO: write a better light shader
            var fsSource = File.ReadAllText("EngineResources/lit.frag");

            var VS = new GLShader(vsSource, GLShaderType.VERTEX);
            var FS = new GLShader(fsSource, GLShaderType.FRAGMENT);

            Program = new GLShaderProgram()
                .Label("Scene")
                .Attach(VS)
                .Attach(FS)
                .Link();
        }

        public static void L3D_BeforeFrame()
        {
            Program.Use();

            if (RenderTexture != null)
            {
                RenderTexture.Use();
                GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.Viewport(0, 0, Viewport.Width, Viewport.Height);
            }

            GL.Enable(EnableCap.DepthTest);

            var proj = Camera.Projection();
            var view = Camera.View();

            Program.UniformMatrix4("View", ref view);
            Program.UniformMatrix4("Projection", ref proj);

            Program.Uniform3("AmbientColour", AmbientColour);
            Program.Uniform3("ObjectColour", ObjectColour);
            Program.Uniform3("ViewPosition", Camera.Parent.Position);

            // We actually -do- need the index here
            for (int i=0; i<PointLights.Count; i++)
            {
                var light = PointLights[i];
                Program.Uniform3($"Points[{i}].Position", light.Parent.Position);
                Program.Uniform3($"Points[{i}].Colour", light.Colour);
                Program.Uniform1($"Points[{i}].Intensity", light.Intensity);
                Program.Uniform1($"Points[{i}].Linear", light.Linear);
                Program.Uniform1($"Points[{i}].Quadratic", light.Quadratic);
            }

            Program.Uniform1i("PointCount", PointLights.Count);
        }

        public static void L3D_AfterFrame()
        {
            GLRenderTexture.Reset();
        }

        public static void L3D_SceneEnding()
        {
            PointLights.Clear();

            objects.Clear();
        }
    }
}