using Celeste.AudioSystem;
using Celeste.Character;
using Microsoft.Xna.Framework;
using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class dashState : IMadelineState
    {
        private float _timeLeft;
        private float _ghostTimer;
        private Vector2 _dashDirection;
        private bool _startedOnGround;
        private float _horizontalSpeedBeforeDash;
        private bool _dashStarted;
        private bool _waitingForAimFrame;

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
            _horizontalSpeedBeforeDash = m.GetCurrentHorizontalSpeed();
            _dashStarted = false;
            _waitingForAimFrame = true;
            _dashDirection = Vector2.Zero;
            m.velocityX = 0f;
            m.velocityY = 0f;
        }

        private void BeginDash(Madeline m)
        {
            _dashDirection = m.ConsumeDashDirection();
            if (_dashDirection == Vector2.Zero)
            {
                _dashDirection = m.FaceLeft ? new Vector2(-1f, 0f) : new Vector2(1f, 0f);
            }

            Vector2 dashVelocity = _dashDirection * PlayerDashSpeed;
            if (System.Math.Sign(_horizontalSpeedBeforeDash) == System.Math.Sign(dashVelocity.X)
                && System.Math.Abs(_horizontalSpeedBeforeDash) > System.Math.Abs(dashVelocity.X))
            {
                dashVelocity.X = _horizontalSpeedBeforeDash;
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


            if (_dashDirection.X < 0f)
            {
                SoundManager.Play("dash_left");
            }
            else if (_dashDirection.X > 0f)
            {
                SoundManager.Play("dash_right");
            }
            else
            {
                SoundManager.Play(m.FaceLeft ? "dash_left" : "dash_right");
            }

            _dashStarted = true;
        }

        public void Update(Madeline m, float dt)
        {
            if (_waitingForAimFrame)
            {
                _waitingForAimFrame = false;
                return;
            }

            if (!_dashStarted)
            {
                BeginDash(m);
            }

            _ghostTimer -= dt;
            if (_ghostTimer <= 0f)
            {
                m.AddGhost(m.position, m.FaceLeft);
                _ghostTimer = 0.04f;
            }

            _timeLeft -= dt;
            if (_timeLeft <= 0f)
            {
                _timeLeft = 0f;
                m.QueueDashRecovery(_dashDirection);
            }
        }

        public void Exit(Madeline m) { }
    }
}
