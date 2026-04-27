using Microsoft.Xna.Framework;

namespace Celeste
{
    public static class HairConstants
    {
        public static readonly Color DefaultHairColor = new(172, 50, 50);
        public static readonly Color DashUsedHairColor = new(68, 183, 255);

        public const int DefaultHairNodeCount = 4;
        public const int DefaultBangsFrameWidth = 8;
        public const int DefaultBangsFrameHeight = 8;
        public const int BangsFrameCount = 3;

        public const float HairMaxNodeDistanceNative = 3f;
        public const float HairStepApproachNative = 64f;
        public static readonly Vector2 HairStepPerSegmentNative = new(0f, 2f);
        public const float HairStepFacingNative = 0.5f;
        public const float HairTaperMin   = 0.25f;
        public const float HairTaperRange = 0.75f;

        public static readonly Vector2 PlayerBodyOrigin = new(16f, 32f);
        public const float DashHairUsedDisplayTime = 0.35f;
        public const float DashHairRefillFlashTime = 0.12f;
        public const float BaseHeadY = 12f;
        public const float DuckHeadY = 7f;
        public const float TiredHeadY = 10f;
    }
}
