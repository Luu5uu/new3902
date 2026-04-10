using Microsoft.Xna.Framework;

namespace Celeste
{
    public static class DeathConstants
    {
        public static readonly Color NormalDeathColor = new(172,  50,  50);
        public static readonly Color DashDeathColor   = new( 44, 183, 255);

        public const int   BurstCount    = 14;
        public const float BurstMinSpeed = 135f;
        public const float BurstMaxSpeed = 210f;
        public const float BurstMinLife  = 0.14f;
        public const float BurstMaxLife  = 0.30f;
        public const float BurstMinSize  = 1.15f;
        public const float BurstMaxSize  = 1.75f;

        public const int   DeathRingCount = 8;
        public const float DeathRingRadius = 10f;
        public const float DeathRingEndRadius = 30f;
        public const float DeathRingLifetime = 0.34f;
        public const float DeathRingDotScale = 1.25f;
        public const float DeathRingEndDotScale = 1.75f;
        public const float DeathRingFadeStart = 0.90f;

        public const float DeathSpritePlaybackMultiplier = 1.8f;
        public const float DeathBurstDelay = 0.04f;
        public const float DeathCutoutDelay = 0.34f;
        public const float DeathCutoutDuration = 0.28f;
        public const float DeathCutoutMaxHalfSize = 120f;
    }
}
