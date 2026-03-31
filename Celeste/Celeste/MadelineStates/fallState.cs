using Celeste.Character;

using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class fallState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.Maddy.FallSlow();
        }

        public void Update(Madeline m, float dt)
        {
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
            float x = m.moveX * PlayerAirSpeed * dt;
            m.position.X += x;

            if (x < 0f) m.FaceLeft = true;
            else if (x > 0f) m.FaceLeft = false;

            if (m.onGround) m.changeState(m.standState);
        }

        public void Exit(Madeline m) { }
    }
}
