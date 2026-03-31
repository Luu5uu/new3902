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
        public const float PlayerRunSpeed = 200f;
        public const float PlayerAirSpeed = 200f;
        public const float PlayerJumpSpeed = 15f;
        public const float PlayerGravity = 60f;
        public const float PlayerJumpBufferTime = 0.1f;
        public const float PlayerDashBufferTime = 0.1f;
        public const float PlayerJumpGraceTime = 0.1f;
        public const float PlayerDashDuration = 0.15f;
        public const float PlayerDashSpeed = 540f;
        public const float PlayerDashEndSpeed = 360f;
        public const float PlayerDashEndUpMultiplier = 0.75f;
        public const float PlayerDashCarryDeceleration = 1200f;
        public const float PlayerClimbMaxStamina = 110f;
        public const float PlayerClimbUpCostPerSecond = 45f;
        public const float PlayerClimbStillCostPerSecond = 10f;
        public const float PlayerClimbTiredThreshold = 20f;
        public const float PlayerClimbUpSpeed = 60f;
        public const float PlayerClimbDownSpeed = 80f;
        public const float PlayerClimbSlipSpeed = 30f;
        public const float PlayerDangleFallSpeed = 40f;
    }
}
