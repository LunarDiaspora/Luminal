namespace Luminal.Core
{
    public static class Timing
    {
        public static float DeltaTime = 0.0f;

        public static float TotalElapsedTime = 0.0f;

        public static float FrameRate = 0.0f;

        public static long FrameNumber = 0;

        internal static int frameCount = 0;

        internal static float fpsCounter = 0.0f;
    }
}