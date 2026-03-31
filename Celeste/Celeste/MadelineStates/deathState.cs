using Celeste.Character;

namespace Celeste.MadelineStates
{
    /// <summary>
    /// DeathState:
    /// - On enter: spawn DeathEffect (DeathAnimation folder) at Madeline.position (top-left aligned).
    /// - During update: only update DeathEffect; block movement/physics.
    /// - On finish: clear effect and return to standState (or you can change to respawn logic).
    /// </summary>
    public sealed class DeathState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            // Capture before clearing — used to color death particles
            bool wasDashing = m.isDashing;

            // Freeze character (conservative: disable dash/jump movement)
            m.isDashing = false;
            m.isClimbing = false;
            m.isDangle = false;
            m.velocityY = 0f;
            m.moveX = 0f;
            m.onGround = false;
            m.touchingLeftWall = false;
            m.touchingRightWall = false;

            // Spawn the integrated death animation (sprite -> particles).
            m.StartDeathEffect(wasDashing);
        }

        public void Update(Madeline m, float dt)
        {
            m.UpdateDeathEffect(dt);

            if (m.IsDeathEffectFinished())
            {
                m.ClearDeathEffect();
                m.RequestLevelReset();
            }
        }

        public void Exit(Madeline m)
        {
            // No-op (state ends when effect ends).
        }
    }
}
