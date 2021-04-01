﻿using Luminal.Audio;
using Luminal.Logging;
using Luminal.Modular;

namespace LuminalNullAudioEngine
{
    [LuminalAudioModule]
    public class NullAudioEngine : GenericAudioEngine
    {
        public NullAudioEngine(int _) : base(44100)
        {
        }

        public override void Initialise()
        {
            Log.Debug("Initialising NullAudioEngine");
        }

        public override void Dispose()
        {
            Log.Debug("Disposing NullAudioEngine");
        }

        public override GenericAudioFile LoadFileFromPath(string _)
        {
            return new NullFile();
        }

        public override void PlaySound(GenericAudioFile f)
        {
            return;
        }

        public override void DisposeOfFile(GenericAudioFile f)
        {
            return;
        }
    }
}