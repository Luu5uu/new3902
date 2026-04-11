using Celeste.Character;

namespace Celeste.MadelineStates
{
    internal class dangleState:IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.isDangle = true;
            m.isClimbing = false;
            m.onGround = false;
            m.velocityX = 0f;
            m.velocityY = m.dangleFallSpeed;
            m.Maddy.WallSlide();
            m.Maddy.ClearSweat();
        }

        public void Update(Madeline m, float dt)
        {
            if (m.ConsumeJumpPress())
            {
                if (m.CanWallJump())
                {
                    m.PerformWallJump();
                }
                else
                {
                    m.changeState(m.jumpState);
                }
                return;
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

            if (!m.IsTouchingWall)
            {
                m.changeState(m.fallState);
                return;
            }

            if (!m.onGround)
            {
                m.velocityY = m.dangleFallSpeed;
            }
            else if (m.onGround)
            {
                m.isClimbing = false;
                m.velocityY = 0f;
                m.changeState(m.WantsToCrouch() ? m.crouchState : m.standState);
                return;
            }
        }

        public void Exit(Madeline m) {
            m.isDangle = false;
            m.Maddy.ClearSweat();
        }
    }
}
