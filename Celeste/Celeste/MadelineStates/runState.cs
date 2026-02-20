using Celeste.Character;

namespace Celeste.MadelineStates
{
    public class runState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.Maddy.Run();
        }

        public void Update(Madeline m, float dt)
        {
            if (m.onGround && m.jumpPressed)
            {
                m.changeState(m.jumpState);
                return;
            }
            if (m.dashPressed && m.canDash)
            {
                m.changeState(m.dashState);
                return;
            }

            float x = m.moveX;
            if (x == 0f)
            {
                m.changeState(m.standState);
                return;
            }

            m.position.X += x * m.velocity * dt;
            if (x < 0f) m.FaceLeft = true;
            else if (x > 0f) m.FaceLeft = false;
        }

        public void Exit(Madeline m) { }
    }
}
