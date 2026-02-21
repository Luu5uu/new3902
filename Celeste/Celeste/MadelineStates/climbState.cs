using Celeste.Character;

namespace Celeste.MadelineStates
{
    public class climbState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.Maddy.ClimbUp();
            m.isClimbing = true;
            m.velocityY = 0f;
        }

        public void Update(Madeline m, float dt)
        {

            if(m.moveX != 0)
            {
                m.changeState(m.fallState);
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

                m.position.Y -= m.climbSpeed * dt;

        }

        public void Exit(Madeline m) {
            m.isClimbing = false;

        }
    }
}