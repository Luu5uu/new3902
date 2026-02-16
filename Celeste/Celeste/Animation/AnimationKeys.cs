namespace Celeste.Animation
{
    /// <summary>
    /// Centralized string keys for animation lookup.
    /// Keep these stable to avoid breaking other systems.
    /// </summary>
    /// <author> Albert Liu </author>
    public static class AnimationKeys
    {
        // Player (composite sprite: idle, idleA, runFast, etc.)
        public const string PlayerStandard = "Player/Standard";
        public const string PlayerIdle     = "Player/Idle";
        public const string PlayerIdleFidgetA = "Player/IdleFidgetA";
        public const string PlayerRun      = "Player/Run";
        public const string PlayerJumpFast = "Player/JumpFast";
        public const string PlayerFallSlow = "Player/FallSlow";
        public const string PlayerDash     = "Player/Dash";
        public const string PlayerClimbUp  = "Player/ClimbUp";
        public const string PlayerDangling = "Player/Dangling";

        // Player legacy (H toggle): manually-drawn hair strips. Content assets named *_static_hair.
        public const string PlayerStandardStaticHair  = "Player/StandardStaticHair";
        public const string PlayerIdleStaticHair     = "Player/IdleStaticHair";
        public const string PlayerRunStaticHair     = "Player/RunStaticHair";
        public const string PlayerJumpFastStaticHair = "Player/JumpFastStaticHair";
        public const string PlayerFallSlowStaticHair = "Player/FallSlowStaticHair";
        public const string PlayerDashStaticHair     = "Player/DashStaticHair";
        public const string PlayerClimbUpStaticHair  = "Player/ClimbUpStaticHair";
        public const string PlayerDanglingStaticHair  = "Player/DanglingStaticHair";

        // Items
        public const string ItemNormalStaw = "Item/NormalStaw";
        public const string ItemFlyStaw    = "Item/FlyStaw";
    }
}