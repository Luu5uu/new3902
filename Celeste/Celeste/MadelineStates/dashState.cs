using Celeste.Character;

using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class dashState : IMadelineState
    {
        private float _timeLeft;
        private float _dashDir;
        private float _ghostTimer;

        public void SetState(Madeline m)
        {
            m.Maddy.Dash();
            m.Maddy.OnDashUsed();
            m.isDashing = true;
            m.canDash   = false;
            m.velocityY = 0f;

            _timeLeft = PlayerDashDuration;
            _ghostTimer = 0f;

            float x = m.moveX;
            if (x < 0f) _dashDir = -1f;
            else if (x > 0f) _dashDir = 1f;
            else _dashDir = m.FaceLeft ? -1f : 1f;

            m.FaceLeft = _dashDir < 0f;
        }

        public void Update(Madeline m, float dt)
        {
            _ghostTimer -= dt;
            if (_ghostTimer <= 0f)
            {
                m.AddGhost(m.position, m.FaceLeft);
                _ghostTimer = 0.04f;
            }

            m.position.X += _dashDir * PlayerRunSpeed * PlayerDashSpeedMultiplier * dt;

            _timeLeft -= dt;
            if (_timeLeft <= 0f)
            {
                m.isDashing = false;
                if (!m.onGround) m.changeState(m.fallState);
                else if (m.moveX == 0f) m.changeState(m.standState);
                else m.changeState(m.runState);
            }
        }

        public void Exit(Madeline m) { }
    }
}
