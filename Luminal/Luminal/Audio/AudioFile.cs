using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoLoud;

namespace Luminal.Audio
{
    public class AudioFile : IDisposable
    {
        internal Wav file;

        public AudioFile(string path)
        {
            file = new();
            Load(path);
        }

        public void Load(string path)
        {
            if (file.load(path) != 1)
            {
                throw new Exception($"Failed to load audio file {path}.");
            }
        }

        public void Play()
        {
            AudioEngine.Instance.PlaySound(this);
        }

        public void Dispose()
        {
            file.stop();
        }
    }
}
