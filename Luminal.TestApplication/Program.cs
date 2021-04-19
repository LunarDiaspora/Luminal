﻿using ImGuiNET;
using Luminal.Core;
using Luminal.Entities;
using Luminal.Entities.Components;
using Luminal.Entities.Screen;
using Luminal.Entities.World;
using Luminal.OpenGL.Models;
using System.Collections.Generic;
using SC = SDL2.SDL.SDL_Scancode;
using Vector3 = System.Numerics.Vector3;

namespace Luminal.TestApplication
{
    internal class DemoWindowComponent : Component3D
    {
        public override void OnGUI()
        {
            ImGui.ShowDemoWindow(ref Enabled);
        }
    }

    internal class DebugTool : Component3D
    {
        public override void OnGUI()
        {
            ImGui.Begin("Debug Tool");

            var material = Main.model.GetComponent<ModelRenderer>().Material;

            ImGui.ColorEdit3("Ambient colour", ref Main.AmbientColour);
            ImGui.ColorEdit3("Albedo colour", ref material.Albedo);
            ImGui.ColorEdit3("Specular colour", ref material.Specular);

            ImGui.SliderFloat("Field of view", ref Main.camera.GetComponent<Camera3D>().FieldOfView, 1f, 179.9f);
            ImGui.SliderFloat("Model angle", ref Main.modelAngle, 0f, 360f);

            ImGui.DragFloat3("Player position", ref Main.PlayerPos, 0.25f);

            ImGui.SliderFloat("Shininess", ref material.Shininess, 1f, 128f);

            if (ImGui.CollapsingHeader("Animation controls", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (ImGui.Button("Play"))
                {
                    AnimationManager.Play("test");
                }

                ImGui.SameLine();
                if (ImGui.Button("Pause"))
                {
                    AnimationManager.Pause("test");
                }
            }

            ImGui.Checkbox("Demo window visible", ref Main.camera.GetComponent<DemoWindowComponent>().Enabled);

            ImGui.End();
        }
    }

    internal class LightControlPanel : Component3D
    {
        public Vector3 newColour = new(1.0f, 1.0f, 1.0f);

        public override void OnGUI()
        {
            ImGui.Begin("Light Controls");

            for (int index=0; index<Main.PointLights.Count; index++)
            {
                var light = Main.PointLights[index];

                if (ImGui.TreeNode($"Light #{index} - point"))
                {
                    var coupler = light.GetComponent<CoupleToCameraController>();
                    var lightComponent = light.GetComponent<PointLight3D>();

                    ImGui.Checkbox("Couple to camera", ref coupler.Enabled);

                    ImGui.ColorEdit3("Colour", ref lightComponent.Colour);

                    ImGui.DragFloat("Intensity", ref lightComponent.Intensity);

                    ImGui.DragFloat3("Position", ref light.Position, 0.25f);

                    ImGui.TreePop();
                }
            }

            ImGui.Separator();

            ImGui.ColorEdit3("New light colour", ref newColour);
            if (ImGui.Button("Add new light"))
            {
                Main.MakeNewLight(Main.camera.Position, newColour);
            }

            ImGui.End();
        }
    }

    internal class CoupleToCameraController : Component3D
    {
        public override void Update()
        {
            Parent.Position = Main.camera.Position;
        }
    }

    internal class Main
    {
        public Main()
        {
            var e = new Engine();

            e.OnDraw += Draw;
            e.OnGUI += GUI;

            e.KeyDown += KeyDown;

            e.OnFinishedLoad += Init;
            e.OnUpdate += Update;

            AnimationManager.AddPaused("test", new()
            {
                Length = 2.0f,
                Loop = true,
                Min = 0.0f,
                Max = 500f,
                Ease = Easing.Exponential.InOut
            });

            e.StartRenderer(1920, 1080, "Luminal Engine 3D Demonstration", typeof(Main),
                LuminalFlags.ENABLE_KEY_REPEAT);
        }

        public void Draw(Engine _)
        {
            Context.SetColour(255, 0, 0, 255);
            //Render.Rectangle(100 + AnimationManager.Value("test", 0.0f), 100, 100, 100, RenderMode.FILL);
        }

        internal static Vector3 AmbientColour = new(1, 1, 1);
        internal static Vector3 DiffuseColour = new(1, 1, 1);
        internal static Vector3 ObjectColour = new(0.7f, 0.7f, 0.7f);

        internal static Vector3 PlayerPos = new();
        internal static Vector3 LightPos = new();

        internal static float shininess = 32.0f;

        internal static List<Object3D> PointLights = new();

        public void GUI(Engine _)
        {
        }

        internal static Object3D camera = new();
        internal static Object3D model = new();

        internal static Object2D test2d = new();

        private static Model testModel;

        internal static PointLight3D MakeNewLight(Vector3 where, Vector3 colour)
        {
            var obj = new Object3D();

            var light = obj.CreateComponent<PointLight3D>();

            obj.Position = where;

            light.Colour = colour;

            obj.CreateComponent<CoupleToCameraController>();

            PointLights.Add(obj);

            return light;
        }

        private void Init(Engine _)
        {
            camera.Position = new Vector3(0f, 0f, -5.0f);
            camera.Euler = Vector3.Zero;

            camera.CreateComponent<Camera3D>();

            // These do not have to be on the camera! These can be anywhere.
            // I just put them here because I don't want to make another object for imgui.
            camera.CreateComponent<DebugTool>();
            camera.CreateComponent<DemoWindowComponent>();
            camera.CreateComponent<LightControlPanel>();

            MakeNewLight(new Vector3(0f, 0f, -10.0f), new Vector3(1.0f, 1.0f, 1.0f));

            var mr = model.CreateComponent<ModelRenderer>();

            testModel = new("teapot.obj");
            mr.Model = testModel;

            mr.Material.AlbedoMap = new("Boris", "file.jpg");

            //var ir = test2d.CreateComponent<ImageRenderer>();
            //ir.LoadImage("file.jpg");

            test2d.Position.X = 300f;
            test2d.Position.Y = 300f;
        }

        internal static float modelAngle = 0.0f;

        private void Update(Engine _, float __)
        {
            camera.Position = PlayerPos;

            model.Euler = new(0.0f, modelAngle, 0.0f);
        }

        private void KeyDown(Engine _, SC s)
        {
            const float speed = 0.1f;
            const float turnSpeed = 0.7f;

            switch (s)
            {
                case SC.SDL_SCANCODE_S:
                    PlayerPos += (camera.Forward * -speed);
                    break;

                case SC.SDL_SCANCODE_W:
                    PlayerPos += (camera.Forward * speed);
                    break;

                case SC.SDL_SCANCODE_A:
                    PlayerPos += (camera.Right * -speed);
                    break;

                case SC.SDL_SCANCODE_D:
                    PlayerPos += (camera.Right * speed);
                    break;

                case SC.SDL_SCANCODE_LEFT:
                    camera.Rotate(new Vector3(0.0f, -turnSpeed, 0.0f));
                    break;

                case SC.SDL_SCANCODE_RIGHT:
                    camera.Rotate(new Vector3(0.0f, turnSpeed, 0.0f));
                    break;

                case SC.SDL_SCANCODE_Q:
                    modelAngle -= turnSpeed;
                    break;

                case SC.SDL_SCANCODE_E:
                    modelAngle += turnSpeed;
                    break;

                case SC.SDL_SCANCODE_R:
                    PlayerPos += (camera.Up * speed);
                    break;

                case SC.SDL_SCANCODE_F:
                    PlayerPos += (-camera.Up * speed);
                    break;

                case SC.SDL_SCANCODE_UP:
                    camera.Rotate(new Vector3(turnSpeed, 0.0f, 0.0f));
                    break;

                case SC.SDL_SCANCODE_DOWN:
                    camera.Rotate(new Vector3(-turnSpeed, 0.0f, 0.0f));
                    break;
            }
        }
    }

    internal class Entrypoint
    {
        private static void Main(string[] args)
        {
            new Main();
        }
    }
}