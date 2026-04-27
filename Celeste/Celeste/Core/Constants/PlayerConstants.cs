namespace Celeste
{
    /// <summary>
    /// Constants for the player: sprite frame size, movement, and physics.
    /// </summary>
    public static class PlayerConstants
    {
        // Frame width/height for player body sprite strips
        public const int PlayerBodyFrameWidth = 32;
        public const int PlayerBodyFrameHeight = 32;
        public const int PlayerHitboxWidth = 16;
        public const int PlayerNormalHitboxHeight = 32;
        public const int PlayerDuckHitboxHeight = 18;

        // Player movement / physics (used by Madeline and MadelineStates)
        public const float PlayerTileScale = 2.5f;
        public const float PlayerRunSpeed = 90f * PlayerTileScale;
        public const float PlayerAirSpeed = 90f * PlayerTileScale;
        public const float PlayerRunAcceleration = 1000f * PlayerTileScale;
        public const float PlayerRunDeceleration = 400f * PlayerTileScale;
        public const float PlayerDuckDeceleration = 500f * PlayerTileScale;
        public const float PlayerAirAccelerationMultiplier = 0.65f;
        public const float PlayerJumpSpeed = 120f * PlayerTileScale;
        public const float PlayerJumpHorizontalBoost = 40f * PlayerTileScale;
        public const float PlayerWallJumpHorizontalSpeed = PlayerRunSpeed + PlayerJumpHorizontalBoost;
        public const float PlayerWallJumpSpeed = PlayerJumpSpeed * 0.70f;
        public const float PlayerNeutralWallJumpSpeed = PlayerJumpSpeed * 0.50f;
        public const float PlayerWallJumpVariableTime = 0.12f;
        public const int PlayerWallJumpCheckDistance = 2;
        public const int PlayerSuperWallJumpCheckDistance = 5;
        public const float PlayerSuperWallJumpHorizontalSpeed = PlayerRunSpeed + PlayerJumpHorizontalBoost * 2f;
        public const float PlayerSuperWallJumpSpeed = 160f * PlayerTileScale;
        public const float PlayerSuperWallJumpVariableTime = 0.25f;
        public const float PlayerClimbJumpConversionTime = 0.16f;
        public const float PlayerClimbJumpStaminaCost = 27.5f;
        public const float PlayerLiftMomentumStorageTime = 0.08f;
        public const float PlayerLiftSpeedXCap = 250f * PlayerTileScale;
        public const float PlayerLiftSpeedYCap = -130f * PlayerTileScale;
        public const float PlayerGravity = 900f * PlayerTileScale;
        public const float PlayerHalfGravityThreshold = 40f * PlayerTileScale;
        public const float PlayerMaxFallSpeed = 160f * PlayerTileScale;
        public const float PlayerJumpBufferTime = 0.1f;
        public const float PlayerDashBufferTime = 0.1f;
        public const float PlayerJumpGraceTime = 0.1f;
        public const float PlayerVariableJumpTime = 0.2f;
        public const float PlayerJumpHoldGravityMultiplier = 0.5f;
        public const float PlayerSpringLaunchSpeed = 400f * PlayerTileScale;
        public const float PlayerDashDuration = 0.15f;
        public const float PlayerDashSpeed = 240f * PlayerTileScale;
        public const float PlayerDashEndSpeed = 160f * PlayerTileScale;
        public const float PlayerDashEndUpMultiplier = 0.75f;
        public const float PlayerDashCarryDeceleration = 1200f * PlayerTileScale;
        public const float PlayerDashGhostInterval = 0.03f;
        public const float PlayerClimbMaxStamina = 110f;
        public const float PlayerClimbUpCostPerSecond = 45f;
        public const float PlayerClimbStillCostPerSecond = 10f;
        public const float PlayerClimbTiredThreshold = 20f;
        public const float PlayerClimbUpSpeed = 45f * PlayerTileScale;
        public const float PlayerClimbDownSpeed = 80f * PlayerTileScale;
        public const float PlayerClimbSlipSpeed = 30f * PlayerTileScale;
        public const float PlayerDangleFallSpeed = 40f * PlayerTileScale;
        public const float PlayerLedgeTopOutInset = 2f;
        public const float PlayerLedgeTopOutHeightWindow = 10f;
        public const float PlayerLedgeTopOutAnimationTime = 4f / 12f;
        public const int PlayerStarFlyHitboxWidth = PlayerHitboxWidth;
        public const int PlayerStarFlyHitboxHeight = 23;
        public const int PlayerStarFlyHitboxBottomInset = 5;
        public const float PlayerStarFlyCollisionStep = 4f;
        public const float PlayerStarFlyTransformTime = 0.4f;
        public const float PlayerStarFlyTime = 2f;
        public const float PlayerStarFlyStartSpeed = 250f * PlayerTileScale;
        public const float PlayerStarFlyTargetSpeed = 140f * PlayerTileScale;
        public const float PlayerStarFlyMaxSpeed = 190f * PlayerTileScale;
        public const float PlayerStarFlySlowSpeed = PlayerStarFlyTargetSpeed * 0.65f;
        public const float PlayerStarFlyMaxLerpTime = 1f;
        public const float PlayerStarFlyAcceleration = 1000f * PlayerTileScale;
        public const float PlayerStarFlyTransformDeceleration = 1000f * PlayerTileScale;
        public const float PlayerStarFlyTurnSpeed = 5.585f;
        public const float PlayerStarFlyEndWarningTime = 0.5f;
        public const float PlayerStarFlyEndHorizontalSpeed = 160f * PlayerTileScale;
        public const float PlayerStarFlyEndVariableJumpTime = 0.1f;
        public const float PlayerStarFlyExitUpSpeed = -100f * PlayerTileScale;
        public const float PlayerStarFlyMaxExitY = 0f;
        public const float PlayerStarFlyMaxExitX = 140f * PlayerTileScale;
        public const float PlayerOutOfBoundsDeathGrace = 72f;
    }
}
