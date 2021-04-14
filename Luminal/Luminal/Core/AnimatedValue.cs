namespace Luminal.Core
{
    public class AnimatedValue
    {
        public float Length = 0.0f;
        public float Min = 0.0f;
        public float Max = 0.0f;
        public float Time = 0.0f;

        public bool Playing = false;
        public bool Loop = false;

        public EasingFunction Ease = Easing.Linear;

        public float Calculate()
        {
            var t = Time / Length;
            var value = LMath.Mix(Ease(t), Min, Max);
            return value;
        }
    }
}
