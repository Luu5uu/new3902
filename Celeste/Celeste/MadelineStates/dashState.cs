using Microsoft.Xna.Framework;
using Celeste.Character;

using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class dashState : IMadelineState
    {
        private float _timeLeft;
        private Vector2 _dashVelocity;
        private float _ghostTimer;

        public void SetState(Madeline m)
        {
            m.Maddy.Dash();
            m.Maddy.OnDashUsed();
            m.isDashing = true;
            m.canDash   = false;
            m.isClimbing = false;
            m.isDangle = false;
            m.velocityY = 0f;

            _timeLeft = PlayerDashDuration;
            _ghostTimer = 0f;
            Vector2 dashDir = m.GetDashDirection();
            _dashVelocity = dashDir;

            if (_dashVelocity != Vector2.Zero)
            {
                m.FaceLeft = _dashVelocity.X < 0f;
            }
        }

        public void Update(Madeline m, float dt)
        {
            _ghostTimer -= dt;
            if (_ghostTimer <= 0f)
            {
                m.AddGhost(m.position, m.FaceLeft);
                _ghostTimer = 0.04f;
            }

            m.position += _dashVelocity * PlayerRunSpeed * PlayerDashSpeedMultiplier * dt;

            _timeLeft -= dt;
            if (_timeLeft <= 0f)
            {
                m.isDashing = false;
                if (m.climbHeld && m.IsTouchingWall) m.changeState(m.climbState);
                else if (!m.onGround) m.changeState(m.fallState);
                else if (m.moveX == 0f) m.changeState(m.standState);
                else m.changeState(m.runState);
            }
        }

        public void Exit(Madeline m) { }
    }
}
