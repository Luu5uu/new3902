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
            m.Maddy.Dangling();
        }

        public void Update(Madeline m, float dt)
        {
            if (m.jumpPressed)
            {
                m.changeState(m.jumpState);
                return;
            }

            if (m.climbHeld)
            {
                m.changeState(m.climbState);
                return;
            }

            if (m.moveX != 0f)
            {
                m.changeState(m.fallState);
                return;
            }

            if (!m.IsTouchingWall)
            {
                m.changeState(m.fallState);
                return;
            }

            if (!m.onGround)
            {
                m.position.Y += m.dangleFallSpeed * dt;
            }
            else if (m.onGround)
            {
                m.isClimbing = false;
                m.changeState(m.standState);
                return;
            }
        }

        public void Exit(Madeline m) {
            m.isDangle = false;
        }
    }
}
