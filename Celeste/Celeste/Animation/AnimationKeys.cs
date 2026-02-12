namespace Celeste.Animation
{
    /// <summary>
    /// Centralized string keys for animation lookup.
    /// Keep these stable to avoid breaking other systems.
    /// </summary>
    /// <author> Albert Liu </author>
    public static class AnimationKeys
    {
        // Player
        public const string PlayerStandard = "Player/Standard";
        public const string PlayerIdle     = "Player/Idle";
        public const string PlayerRun      = "Player/Run";
        public const string PlayerJumpFast = "Player/JumpFast";
        public const string PlayerFallSlow = "Player/FallSlow";
        public const string PlayerDash     = "Player/Dash";
        public const string PlayerClimbUp  = "Player/ClimbUp";
        public const string PlayerDangling = "Player/Dangling";

        // Items
        public const string ItemNormalStaw = "Item/NormalStaw";
        public const string ItemFlyStaw    = "Item/FlyStaw";
    }
}