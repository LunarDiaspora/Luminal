using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminal.Core
{
    public static class AnimationManager
    {
        public static float CurrentTime = 0.0f;
        public static Dictionary<string, AnimatedValue> Values = new();

        public static void Update(float Delta)
        {
            foreach (var (key, value) in Values)
            {
                if (!value.Playing) continue;
                value.Time += Delta;

                if (value.Time >= value.Length)
                {
                    if (value.Loop)
                    {
                        value.Time = 0.0f;
                    } else
                    {
                        value.Playing = false;
                    }
                }
            }

            CurrentTime += Delta;
        }

        public static float Value(string name)
        {
            var t = Values[name];
            return t.Calculate();
        }

        public static float Value(string name, float def)
        {
            if (!Values.ContainsKey(name))
            {
                return def;
            }
            return Value(name);
        }

        public static AnimatedValue Get(string name)
        {
            return Values[name];
        }

        public static void AddPlaying(string name, AnimatedValue v)
        {
            v.Playing = true;
            Values.Add(name, v);
        }

        public static void AddPaused(string name, AnimatedValue v)
        {
            v.Playing = false;
            Values.Add(name, v);
        }

        public static void Play(string name)
        {
            if (!Values.ContainsKey(name)) return;
            Values[name].Playing = true;
        }

        public static void Pause(string name)
        {
            if (!Values.ContainsKey(name)) return;
            Values[name].Playing = false;
        }
    }
}
