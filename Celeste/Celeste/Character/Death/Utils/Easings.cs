namespace Celeste.DeathAnimation.Utils
{
    public static class Easings
    {
        public static float CubicOut(float t)
        {
            float p = 1f - t;
            return 1f - p * p * p;
        }
    }
}
