using Celeste.Character;

using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class fallState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.Maddy.FallSlow();
            m.Maddy.ClearSweat();
        }

        public void Update(Madeline m, float dt)
        {
            if (m.ConsumeJumpPress())
            {
                if (m.CanUseJumpGrace())
                {
                    m.changeState(m.jumpState);
                    return;
                }

                if (m.CanWallJump())
                {
                    m.PerformWallJump();
                    return;
                }
            }

            if (m.canDash && m.ConsumeDashPress())
            {
                m.changeState(m.dashState);
                return;
            }
            if (m.CanGrabWall())
            {
                m.changeState(m.climbState);
                return;
            }
            if (m.IsTouchingWall && m.velocityY >= 0f)
            {
                m.changeState(m.dangleState);
                return;
            }

            m.RefreshFacingFromInput();

            if (m.onGround)
            {
                m.changeState(m.WantsToCrouch() ? m.crouchState : m.standState);
            }
        }

        public void Exit(Madeline m) { }
    }
}
