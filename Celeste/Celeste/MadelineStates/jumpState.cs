using Celeste.Character;

using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class jumpState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            bool wasClimbing = m.isClimbing;
            m.SetCrouching(false);
            m.ConsumeJumpGrace();
            m.velocityX += PlayerJumpHorizontalBoost * m.moveX;
            m.velocityY = -PlayerJumpSpeed;
            m.ApplyLiftBoostToVelocity();
            m.BeginVariableJump();
            m.onGround  = false;
            m.Maddy.JumpFast();
            if (wasClimbing)
            {
                m.Maddy.SweatJump();
            }
            else
            {
                m.Maddy.ClearSweat();
            }
        }

        public void Update(Madeline m, float dt)
        {
            if (m.TryHandleDashPress())
            {
                return;
            }

            if (m.velocityY > 0) m.changeState(m.fallState);

            m.RefreshFacingFromInput();
        }

        public void Exit(Madeline m) { }
    }
}
