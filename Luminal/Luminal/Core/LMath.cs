namespace Luminal.Core
{
    public static class LMath
    {
        public static float Lerp(float alpha, float a, float b)
        {
            return (a * (1.0f - alpha)) + (b * alpha);
        }

        // alias for Lerp to make things less confusing
        public static float Mix(float alpha, float a, float b)
        {
            return Lerp(alpha, a, b);
        }
    }
}