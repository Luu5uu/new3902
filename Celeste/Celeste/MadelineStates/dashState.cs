using Celeste.Character;

namespace Celeste.MadelineStates
{
    public class dashState : IMadelineState
    {
        private const float DashDuration = 0.2f;
        private const float DashSpeedMul = 4f;

        private float _timeLeft;
        private float _dashDir;

        public void setState(Madeline m)
        {
            m.Maddy.Dash();
            m.isDashing = true;
            m.canDash   = false;
            m.velocityY = 0f;

            _timeLeft = DashDuration;

            float x = m.moveX;
            if (x < 0f) _dashDir = -1f;
            else if (x > 0f) _dashDir = 1f;
            else _dashDir = m.FaceLeft ? -1f : 1f;

            m.FaceLeft = _dashDir < 0f;
        }

        public void update(Madeline m, float dt)
        {
            m.position.X += _dashDir * m.velocity * DashSpeedMul * dt;

            _timeLeft -= dt;
            if (_timeLeft <= 0f)
            {
                m.isDashing = false;
                if (!m.onGround) m.changeState(m.fallState);
                else if (m.moveX == 0f) m.changeState(m.standState);
                else m.changeState(m.runState);
            }
        }

        public void exit(Madeline m) { }
    }
}
