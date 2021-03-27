using System;
using Luminal.Modular;
using Luminal.Audio;
using Luminal.Logging;
using FmodAudio;

namespace LuminalFMODCoreEngine
{
    [LuminalAudioModule()]
    public class FMODCoreAudioEngine : GenericAudioEngine
    {
        public FmodSystem system;
        bool closing = false;

        public FMODCoreAudioEngine(int sr = 44100) : base(sr)
        {
            system = Fmod.CreateSystem();
        }

        public override void Initialise()
        {
            Log.Warn("FmodCore audio engine initialising");
            system.Init(999);
        }

        public override void Dispose()
        {
            Log.Warn("FmodCore audio engine disposing");
            closing = true;
            system.Dispose();
        }

        public override void PlaySound(GenericAudioFile _f)
        {
            if (closing) return;
            var f = (FMODCoreFile)_f;
            system.PlaySound(f.snd);
        }

        public override GenericAudioFile LoadFileFromPath(string p)
        {
            if (closing) return null;
            var f = new FMODCoreFile(p, this);
            return f;
        }

        public override void Update(float dt)
        {
            if (closing) return;
            system.Update();
        }
    }
}
