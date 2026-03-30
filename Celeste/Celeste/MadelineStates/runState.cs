using Celeste.Character;

using static Celeste.PlayerConstants;

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
            if (m.CanUseJumpGrace() && m.ConsumeJumpPress())
            {
                m.changeState(m.jumpState);
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
            float x = m.moveX;
            if (x == 0f)
            {
                m.changeState(m.standState);
                return;
            }

            m.position.X += x * PlayerRunSpeed * dt;
            if (x < 0f) m.FaceLeft = true;
            else if (x > 0f) m.FaceLeft = false;
        }

        public void Exit(Madeline m) { }
    }
}
