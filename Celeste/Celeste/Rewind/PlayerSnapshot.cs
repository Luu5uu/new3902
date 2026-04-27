using Microsoft.Xna.Framework;

namespace Celeste.Rewind
{
    public struct PlayerSnapshot
    {
        // Core transform / motion
        public Vector2 Position;
        public float VelocityX;
        public float VelocityY;
        public bool FaceLeft;

        // Collision-related runtime state
        public bool OnGround;
        public bool HitCeiling;
        public bool TouchingLeftWall;
        public bool TouchingRightWall;
        public bool IsCrouching;

        // Ability / movement mode
        public bool IsClimbing;
        public bool IsDashing;
        public bool IsDangle;
        public bool IsStarFlying;
        public bool CanDash;
        public float ClimbStamina;

        // Buffered timers / jump feel
        public float JumpBufferTimer;
        public float DashBufferTimer;
        public float JumpGraceTimer;
        public float VariableJumpTimer;
        public float VariableJumpSpeed;
        public Vector2 StoredLiftSpeed;
        public float StoredLiftTimer;
        public float ClimbJumpConversionTimer;
        public int ClimbJumpConversionDirection;
        public float ClimbJumpRefundAmount;
        public float StarFlyTimer;
        public float StarFlyTransformTimer;
        public float StarFlyAngle;
        public float StarFlyCurrentSpeed;
        public float StarFlySpeedLerp;
        public bool StarFlyTransforming;

        // Aim / state transition helpers
        public Vector2 LastAim;
        public bool DashRecoveryQueued;
        public Vector2 DashRecoveryDirection;

        public bool LedgeTopOutQueued;
        public Vector2 LedgeTopOutPosition;
        public float LedgeTopOutTimer;

        // Small runtime timers
        public float FootstepTimer;
        public float ClimbSoundTimer;
        public float TiredFlashPhase;

        // State machine
        public RewindStateKind StateKind;
    }
}
