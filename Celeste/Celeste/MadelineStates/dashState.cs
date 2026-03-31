using Microsoft.Xna.Framework;
using Celeste.Character;

using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class dashState : IMadelineState
    {
        private float _timeLeft;
        private float _ghostTimer;
        private Vector2 _dashDirection;
        private bool _startedOnGround;

        public void SetState(Madeline m)
        {
            m.Maddy.Dash();
            m.Maddy.OnDashUsed();
            m.isDashing = true;
            m.canDash   = false;
            m.isClimbing = false;
            m.isDangle = false;

            _timeLeft = PlayerDashDuration;
            _ghostTimer = 0f;
            _startedOnGround = m.onGround;
            _dashDirection = m.GetDashDirection();

            if (_dashDirection == Vector2.Zero)
            {
                _dashDirection = m.FaceLeft ? new Vector2(-1f, 0f) : new Vector2(1f, 0f);
            }

            Vector2 dashVelocity = _dashDirection * PlayerDashSpeed;

            float horizontalSpeedBeforeDash = m.GetCurrentHorizontalSpeed();
            if (System.Math.Sign(horizontalSpeedBeforeDash) == System.Math.Sign(dashVelocity.X)
                && System.Math.Abs(horizontalSpeedBeforeDash) > System.Math.Abs(dashVelocity.X))
            {
                dashVelocity.X = horizontalSpeedBeforeDash;
            }

            if (_startedOnGround && _dashDirection.X != 0f && _dashDirection.Y > 0f && dashVelocity.Y > 0f)
            {
                _dashDirection.X = System.Math.Sign(_dashDirection.X);
                _dashDirection.Y = 0f;
                dashVelocity.Y = 0f;
            }

            m.velocityX = dashVelocity.X;
            m.velocityY = dashVelocity.Y;

            if (_dashDirection.X != 0f)
            {
                m.FaceLeft = _dashDirection.X < 0f;
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

            m.position.X += m.velocityX * dt;
            m.position.Y += m.velocityY * dt;

            _timeLeft -= dt;
            if (_timeLeft <= 0f)
            {
                m.isDashing = false;
                if (_dashDirection.Y <= 0f)
                {
                    m.velocityX = _dashDirection.X * PlayerDashEndSpeed;
                    m.velocityY = m.hitCeiling ? 0f : _dashDirection.Y * PlayerDashEndSpeed;

                    if (m.velocityY < 0f)
                    {
                        m.velocityY *= PlayerDashEndUpMultiplier;
                    }
                }

                if (m.CanGrabWall()) m.changeState(m.climbState);
                else if (!m.onGround) m.changeState(m.fallState);
                else if (m.moveX == 0f) m.changeState(m.standState);
                else m.changeState(m.runState);
            }
        }

        public void Exit(Madeline m) { }
    }
}
