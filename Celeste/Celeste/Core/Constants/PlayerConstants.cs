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

        // Player movement / physics (used by Madeline and MadelineStates)
        public const float PlayerTileScale = 2.5f;
        public const float PlayerRunSpeed = 90f * PlayerTileScale;
        public const float PlayerAirSpeed = 90f * PlayerTileScale;
        public const float PlayerRunAcceleration = 1000f * PlayerTileScale;
        public const float PlayerRunDeceleration = 400f * PlayerTileScale;
        public const float PlayerAirAccelerationMultiplier = 0.65f;
        public const float PlayerJumpSpeed = 105f * PlayerTileScale;
        public const float PlayerJumpHorizontalBoost = 40f * PlayerTileScale;
        public const float PlayerGravity = 900f * PlayerTileScale;
        public const float PlayerHalfGravityThreshold = 40f * PlayerTileScale;
        public const float PlayerMaxFallSpeed = 160f * PlayerTileScale;
        public const float PlayerJumpBufferTime = 0.1f;
        public const float PlayerDashBufferTime = 0.1f;
        public const float PlayerJumpGraceTime = 0.1f;
        public const float PlayerVariableJumpTime = 0.2f;
        public const float PlayerJumpHoldGravityMultiplier = 0.5f;
        public const float PlayerDashDuration = 0.15f;
        public const float PlayerDashSpeed = 240f * PlayerTileScale;
        public const float PlayerDashEndSpeed = 160f * PlayerTileScale;
        public const float PlayerDashEndUpMultiplier = 0.75f;
        public const float PlayerDashCarryDeceleration = 1200f * PlayerTileScale;
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
    }
}
