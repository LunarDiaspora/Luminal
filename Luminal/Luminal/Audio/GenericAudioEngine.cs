using System;

namespace Luminal.Audio
{
    public abstract class GenericAudioEngine : IDisposable
    {
        public GenericAudioEngine(int sampleRate)
        {
        }

        public abstract void Initialise();

        public abstract void Dispose();

        public abstract void PlaySound(GenericAudioFile f);

        public abstract GenericAudioFile LoadFileFromPath(string p);

        public virtual void Update(float dt)
        {
        }
    }
}