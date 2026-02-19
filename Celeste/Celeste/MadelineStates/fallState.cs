using Celeste.Character;

namespace Celeste.MadelineStates
{
    public class fallState : IMadelineState
    {
        public void setState(Madeline m)
        {
            m.Maddy.FallSlow();
        }

        public void update(Madeline m, float dt)
        {
            if (m.dashPressed && m.canDash)
            {
                m.changeState(m.dashState);
                return;
            }

            float x = m.moveX * m.airSpeed * dt;
            m.position.X += x;

            if (x < 0f) m.FaceLeft = true;
            else if (x > 0f) m.FaceLeft = false;

            if (m.onGround) m.changeState(m.standState);
        }

        public void exit(Madeline m) { }
    }
}
