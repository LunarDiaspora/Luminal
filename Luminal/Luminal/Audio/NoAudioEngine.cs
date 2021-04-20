using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Audio
{
    public class NoAudioEngine : GenericAudioEngine
    {
        public NoAudioEngine(int sr) : base(sr)
        {
        }

        public override void Dispose()
        {
        }

        public override void DisposeOfFile(GenericAudioFile f)
        {
        }

        public override void Initialise()
        {
        }

        public override GenericAudioFile LoadFileFromPath(string p)
        {
            return new NoAudioFile();
        }

        public override void PlaySound(GenericAudioFile f)
        {
        }
    }
}
