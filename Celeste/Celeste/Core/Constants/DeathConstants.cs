using Microsoft.Xna.Framework;

namespace Celeste
{
    public static class DeathConstants
    {
        public static readonly Color NormalDeathColor = new(172,  50,  50);
        public static readonly Color DashDeathColor   = new( 44, 183, 255);

        public const int   BurstCount    = 8;
        public const float BurstMinSpeed = 150f;
        public const float BurstMaxSpeed = 220f;
        public const float BurstMinLife  = 0.12f;
        public const float BurstMaxLife  = 0.22f;
        public const float BurstMinSize  = 1.0f;
        public const float BurstMaxSize  = 1.8f;
    }
}
