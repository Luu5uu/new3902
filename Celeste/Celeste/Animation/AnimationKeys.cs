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
        public const string PlayerStandard    = "Player/Standard";
        public const string PlayerIdle        = "Player/Idle";
        public const string PlayerIdleFidgetA = "Player/IdleFidgetA";
        public const string PlayerIdleFidgetB = "Player/IdleFidgetB";
        public const string PlayerIdleFidgetC = "Player/IdleFidgetC";
        public const string PlayerRun         = "Player/Run";
        public const string PlayerJumpFast    = "Player/JumpFast";
        public const string PlayerFallSlow    = "Player/FallSlow";
        public const string PlayerDash        = "Player/Dash";
        public const string PlayerClimbUp     = "Player/ClimbUp";
        public const string PlayerDangling    = "Player/Dangling";
        public const string PlayerWallSlide   = "Player/WallSlide";
        public const string PlayerTired       = "Player/Tired";
        public const string PlayerTiredStill  = "Player/TiredStill";
        public const string PlayerClimbPull   = "Player/ClimbPull";
        public const string PlayerDuck        = "Player/Duck";
        public const string PlayerDeath       = "Player/Death";
        public const string PlayerDeathSide   = "Player/DeathSide";
        public const string PlayerDeathUp     = "Player/DeathUp";
        public const string PlayerDeathDown   = "Player/DeathDown";

        // Player sweat overlay
        public const string PlayerSweatIdle      = "PlayerSweat/Idle";
        public const string PlayerSweatStill     = "PlayerSweat/Still";
        public const string PlayerSweatClimb     = "PlayerSweat/Climb";
        public const string PlayerSweatClimbLoop = "PlayerSweat/ClimbLoop";
        public const string PlayerSweatDanger    = "PlayerSweat/Danger";
        public const string PlayerSweatJump      = "PlayerSweat/Jump";

        // Items
        public const string ItemNormalStaw = "Item/NormalStaw";
        public const string ItemFlyStaw    = "Item/FlyStaw";
        public const string ItemCrystal    = "Item/Crystal";

        // Devices
        public const string DevicesSpring     = "Devices/Spring";
        public const string DevicesMoveBlock  = "Devices/MoveBlock";
        public const string DevicesCrushBlock = "Devices/CrushBlock";
    }
}
