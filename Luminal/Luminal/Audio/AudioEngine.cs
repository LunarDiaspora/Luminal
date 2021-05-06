using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoLoud;
using Luminal.Entities;
using OpenTK.Mathematics;
using Luminal.OpenGL;
using Luminal.Console;

namespace Luminal.Audio
{
    public class AudioEngine : IDisposable
    {
        public Soloud audio;

        [ConVar("aud_volume", "Global volume of all engine audio.")]
        public static float AudioVolume
        {
            get => AudioEngine.Instance.audio.getGlobalVolume();
            set => AudioEngine.Instance.audio.setGlobalVolume(value);
        }

        public void Initialise()
        {
            Log.Info("Initialising audio.");
            audio = new();
            audio.init(Soloud.CLIP_ROUNDOFF | 
                Soloud.ENABLE_VISUALIZATION | 
                Soloud.LEFT_HANDED_3D,
                Soloud.SDL2);
            audio.setGlobalVolume(1);
        }

        public void PlaySound(AudioFile f)
        {
            audio.play(f.file);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this); // Fuck you IntelliSense
            // Also it's spelled finalise.

            Log.Info("Disposing audio.");
            audio.deinit();
        }

        public void Update()
        {
            if (ECSScene.Camera != null)
            {
                // Audio listener will be based off of the camera.
                var obj = ECSScene.Camera.Parent;
                audio.set3dListenerPosition(obj.Position.X, obj.Position.Y, obj.Position.Z);

                var lookTowards = obj.Forward.ToOpenTK().Normalized().ToSystemNumerics(); // ew
                var lookVec = obj.Position + lookTowards;

                audio.set3dListenerAt(lookVec.X, lookVec.Y, lookVec.Z); // The... look vector?
                audio.set3dListenerUp(obj.Up.X, obj.Up.Y, obj.Up.Z);
            }

            audio.update3dAudio();
        }

        public static readonly AudioEngine Instance = new();
    }
}
