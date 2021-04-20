using Luminal.Logging;
using Luminal.Modular;
using System;
using System.Reflection;

namespace Luminal.Audio
{
    public class AudioEngineManager
    {
        public static GenericAudioEngine LoadEngineAndReturn(string Path)
        {
            var asm = Assembly.LoadFrom(Path);
            GenericAudioEngine ae = null;

            foreach (var t in asm.ExportedTypes)
            {
                object[] attrs = t.GetCustomAttributes(typeof(LuminalAudioModule), false);
                if (attrs.Length > 1) throw new ArgumentOutOfRangeException($"Expecting 1 LuminalAudioModule on class {t.Name}, but found {attrs.Length}.");
                if (attrs.Length == 1)
                {
                    Log.Info($"Luminal: Loading {t.Name} as audio engine");
                    ae = (GenericAudioEngine)Activator.CreateInstance(t, 44100);
                    break;
                }
            }

            return ae;
        }

        public static void LoadEngine(string Path)
        {
            if (Path == null) return;
            Engine = LoadEngineAndReturn(Path);
            Engine.Initialise();
        }

        public static GenericAudioEngine Engine = new NoAudioEngine(0);
    }
}