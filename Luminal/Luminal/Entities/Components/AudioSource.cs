using Luminal.Audio;
using SoLoud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Entities.Components
{
    public class AudioSource : Component3D
    {
        public static readonly uint HANDLE_MISSING = 2147483647u;

        public AudioFile Audio;

        uint handle = HANDLE_MISSING;

        public bool PlayOnEnteringPlayMode = false;

        public void MakeHandle()
        {
            handle = AudioEngine.Instance.audio.play3d(Audio.file,
                Parent.Position.X, Parent.Position.Y, Parent.Position.Z,
                0f, 0f, 0f, 1f, 1);
        }

        public void Play()
        {
            if (handle == HANDLE_MISSING)
            {
                MakeHandle();
            }

            AudioEngine.Instance.audio.setPause(handle, 0);
        }

        public void Stop()
        {
            AudioEngine.Instance.audio.setPause(handle, 1);
            AudioEngine.Instance.audio.seek(handle, 0.0);
        }

        public override void BeginPlay()
        {
            if (handle == HANDLE_MISSING)
            {
                MakeHandle();
            }

            if (PlayOnEnteringPlayMode) Play();
        }

        public override void EndPlay()
        {
            Stop();
        }

        public override void UpdateAlways()
        {
            // We don't need any special checks here, actually
            // because SoLoud does nothing if your handle is invalid.

            if (AudioEngine.Instance.audio.isValidVoiceHandle(handle) == 1) // I lied.
                AudioEngine.Instance.audio.set3dSourcePosition(handle,
                    Parent.Position.X, Parent.Position.Y, Parent.Position.Z);
        }
    }
}
