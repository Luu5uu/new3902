using Celeste.Character;

namespace Celeste.MadelineStates
{
    public class jumpState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.velocityY = -m.jumpSpeed;
            m.onGround  = false;
            m.Maddy.JumpFast();
        }

        public void Update(Madeline m, float dt)
        {
            if (m.dashPressed && m.canDash)
            {
                m.changeState(m.dashState);
                return;
            }

            float x = m.moveX * m.airSpeed * dt;
            m.position.X += x;

            if (m.velocityY > 0) m.changeState(m.fallState);

            if (x < 0f) m.FaceLeft = true;
            else if (x > 0f) m.FaceLeft = false;
        }

        public void Exit(Madeline m) { }
    }
}
