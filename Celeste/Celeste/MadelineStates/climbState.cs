using Celeste.Character;

namespace Celeste.MadelineStates
{
    public class climbState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.Maddy.ClimbUp();
            m.isClimbing = true;
            m.isDangle = false;
            m.onGround = false;
            m.velocityY = 0f;
        }

        public void Update(Madeline m, float dt)
        {
            if (m.moveX != 0f)
            {
                m.changeState(m.fallState);
                return;
            }

            if (m.jumpPressed)
            {
                m.changeState(m.jumpState);
                return;
            }
            
            if (!m.climbHeld)
            {
                m.changeState(m.dangleState);
                return;
            }

            if (!m.IsTouchingWall)
            {
                m.changeState(m.fallState);
                return;
            }

            m.position.Y -= m.climbSpeed * dt;

        }

        public void Exit(Madeline m) {
            m.isClimbing = false;
        }
    }
}
