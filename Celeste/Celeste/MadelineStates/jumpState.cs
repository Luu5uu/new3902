using Celeste.Character;

using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class jumpState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.velocityY = -PlayerJumpSpeed;
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

            float x = m.moveX * PlayerAirSpeed * dt;
            m.position.X += x;

            if (m.velocityY > 0) m.changeState(m.fallState);

            if (x < 0f) m.FaceLeft = true;
            else if (x > 0f) m.FaceLeft = false;
        }

        public void Exit(Madeline m) { }
    }
}
