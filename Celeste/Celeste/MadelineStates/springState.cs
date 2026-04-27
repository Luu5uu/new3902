using Celeste.Character;

namespace Celeste.MadelineStates
{
    public sealed class springState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.SetCrouching(false);
            m.isClimbing = false;
            m.isDangle = false;
            m.isDashing = false;
            m.onGround = false;
            m.Maddy.JumpFast(restart: true);
            m.Maddy.ClearSweat();
        }

        public void Update(Madeline m, float dt)
        {
            if (m.onGround)
            {
                m.changeState(m.WantsToCrouch() ? m.crouchState : m.standState);
                return;
            }

            if (m.canDash && m.ConsumeDashPress())
            {
                m.changeState(m.dashState);
                return;
            }

            if (m.CanGrabWall())
            {
                m.BeginWallGrab();
                return;
            }

            if (m.velocityY >= 0f)
            {
                m.changeState(m.fallState);
                return;
            }

            m.RefreshFacingFromInput();
        }

        public void Exit(Madeline m) { }
    }
}
