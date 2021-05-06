using System;
using System.Collections.Generic;
using System.IO;
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
            var p = Path.GetFullPath(path);
            file.load(p);
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
