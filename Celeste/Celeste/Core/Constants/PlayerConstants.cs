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
        public const float PlayerDashDuration = 0.2f;
        public const float PlayerDashSpeedMultiplier = 4f;
    }
}
