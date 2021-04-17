using Luminal.Entities.Components;
using Luminal.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Entities
{
    internal class ECSScene
    {
        public static List<BaseObject> objects = new();

        public static Camera3D Camera;

        // TEMPORARY
        // TODO: Make a better lighting system
        public static PointLight3D Light;

        public static void UpdateAll()
        {
            foreach (var o in objects)
            {
                foreach (var c in o.components)
                {
                    c.Update();
                }
            }
        }

        public static void Render2DAll()
        {
            foreach (var o in objects)
            {
                foreach (var c in o.components)
                {
                    c.Render2D();
                }
            }
        }

        public static void OnGUIAll()
        {
            foreach (var o in objects)
            {
                foreach (var c in o.components)
                {
                    c.OnGUI();
                }
            }
        }

        public static void Render3DAll()
        {
            L3D_BeforeFrame();

            foreach (var o in objects)
            {
                foreach (var c in o.components)
                {
                    c.Render3D();
                }
            }
        }

        static System.Numerics.Vector3 TKToSysNum3(OpenTK.Mathematics.Vector3 tk)
        {
            return new(tk.X, tk.Y, tk.Z);
        }

        public static GLShaderProgram Program;
        public static System.Numerics.Vector3 AmbientColour = new(0.7f, 0.7f, 0.7f);
        public static System.Numerics.Vector3 ObjectColour = new(1f, 1f, 1f);


        public static void L3D_SetUp()
        {
            var vsSource = File.ReadAllText("EngineResources/mesh.vert");

            // TODO: write a better light shader
            var fsSource = File.ReadAllText("EngineResources/one_light.frag");

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

            var proj = Camera.Projection();
            var view = Camera.View();

            Program.UniformMatrix4("View", ref view);
            Program.UniformMatrix4("Projection", ref proj);

            Program.Uniform3("AmbientColour", AmbientColour);
            Program.Uniform3("DiffuseColour", Light.Colour);
            Program.Uniform3("ObjectColour", ObjectColour);
            Program.Uniform3("LightPosition", TKToSysNum3(Light.Parent.Position));
            Program.Uniform3("ViewPosition", TKToSysNum3(Camera.Parent.Position));

            Program.Uniform1("Shininess", Light.Shininess);
        }
    }
}
