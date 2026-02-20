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
            // Freeze character (conservative: disable dash/jump movement)
            m.isDashing = false;
            m.velocityY = 0f;
            m.moveX = 0f;

            // Spawn the integrated death animation (sprite -> particles).
            m.StartDeathEffect();
        }

        public void Update(Madeline m, float dt)
        {
            m.UpdateDeathEffect(dt);

            if (m.IsDeathEffectFinished())
            {
                m.ClearDeathEffect();
                m.changeState(m.standState);
            }
        }

        public void Exit(Madeline m)
        {
            // No-op (state ends when effect ends).
        }
    }
}