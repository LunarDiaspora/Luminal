using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luminal.Audio;
using System.IO;
using FmodAudio;

namespace LuminalFMODCoreEngine
{
    public class FMODCoreFile : GenericAudioFile
    {
        public Sound snd;
        FMODCoreAudioEngine engine;

        public FMODCoreFile(string Path, FMODCoreAudioEngine n)
        {
            engine = n;
            Begin(Path);
        }

        bool Begin(string path)
        {
            var extension = Path.GetExtension(path);

            if (!File.Exists(path))
            {
                // The file literally isn't there. Typo in BMS?
                var p = TryAlternativeFmt(extension);
                if (Begin(Path.ChangeExtension(path, p)))
                {
                    //Console.WriteLine($"Sigh... It said {extension}, but it's actually a {p}. Great. Oh well, we did it.");
                    return true;
                }
                return false;
            }

            snd = engine.system.CreateSound(path, Mode.CreateSample);

            return true;

            /*if (extension == ".wav")
            {
                // wav file
            }
            else if (extension == ".ogg")
            {
                return true;
            }
            else if (extension == ".mp3")
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Unknown file type {extension}.");
                return false;
            }*/
        }

        string TryAlternativeFmt(string t)
        {
            var h = ".ogg";

            switch (t)
            {
                case ".wav":
                    h = ".ogg";
                    break;
                case ".ogg":
                    h = ".mp3";
                    break;
                case ".mp3":
                    h = ".wav";
                    break;
            }

            return h;
        }
    }
}
