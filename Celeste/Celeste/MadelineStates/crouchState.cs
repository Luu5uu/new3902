using Celeste.Character;

namespace Celeste.MadelineStates
{
    public sealed class crouchState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.SetCrouching(true);
            m.Maddy.Duck();
            m.Maddy.ClearSweat();
        }

        public void Update(Madeline m, float dt)
        {
            if (!m.onGround)
            {
                m.SetCrouching(false);
                m.changeState(m.fallState);
                return;
            }

            if (m.TryHandleDashPress())
            {
                return;
            }

            if (m.ConsumeJumpPress())
            {
                m.SetCrouching(false);
                m.changeState(m.jumpState);
                return;
            }

            m.Maddy.Duck();

            if (m.moveY > 0f || !m.CanStandUp())
            {
                return;
            }

            m.SetCrouching(false);
            m.changeState(m.moveX == 0f ? m.standState : m.runState);
        }

        public void Exit(Madeline m)
        {
            m.SetCrouching(false);
        }
    }
}
