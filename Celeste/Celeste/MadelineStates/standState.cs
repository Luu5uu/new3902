using Celeste.Character;

namespace Celeste.MadelineStates
{
    public class standState : IMadelineState
    {
        public void setState(Madeline m)
        {
            m.Maddy.Idle();
        }

        public void update(Madeline m, float dt)
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
            if (m.moveX != 0f) m.changeState(m.runState);
        }

        public void exit(Madeline m) { }
    }
}
