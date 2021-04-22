using Luminal.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoLoud;

namespace Luminal.Audio
{
    public class AudioEngine : IDisposable
    {
        Soloud audio;

        public void Initialise()
        {
            Log.Info("Initialising audio.");
            audio = new();
            audio.init(Soloud.CLIP_ROUNDOFF | Soloud.ENABLE_VISUALIZATION, Soloud.SDL2);
            audio.setGlobalVolume(4);
        }

        public void PlaySound(AudioFile f)
        {
            audio.play(f.file);
        }

        public void Dispose()
        {
            Log.Info("Disposing audio.");
        }

        public static readonly AudioEngine Instance = new();
    }
}
