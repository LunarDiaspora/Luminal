using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Audio
{
    public abstract class GenericAudioEngine : IDisposable
    {
        public GenericAudioEngine(int sampleRate) { }

        public abstract void Initialise();
        public abstract void Dispose();
        public abstract void PlaySound(GenericAudioFile f);
        public abstract GenericAudioFile LoadFileFromPath(string p);
        public virtual void Update(float dt) { }
    }
}
