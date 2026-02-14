using Celeste.Animation;
using Celeste.Character;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.MadelineStates
{
    internal class dashState : IMadelineState
    {
        private const float DashDuration = 0.2f;
        private const float DashSpeedMul = 4f;

        private float _timeLeft;
        private float _dashDir;

        public void setState(Madeline m)
        {
            m._player.setCurrentAnimation(m._anima[AnimationKeys.PlayerDash]);
            m.isDashing = true;
            m.velocityY = 0f;

            _timeLeft = DashDuration;

            float x = m.moveX;
            if (x < 0f) _dashDir = -1f;
            else if (x > 0f) _dashDir = 1f;
            else
            {
                _dashDir = (m.effect == SpriteEffects.FlipHorizontally) ? -1f : 1f;
            }

            m.effect = (_dashDir < 0f) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }
        public void update(Madeline m, float dt)
        {
            float dashSpeed = m.velocity * DashSpeedMul;
            m.position.X += _dashDir * dashSpeed * dt;
            _timeLeft -= dt;
            if (_timeLeft <= 0f)
            {
                m.isDashing = false;
                if (!m.onGround)
                {
                    m.changeState(m.fallState);

                }
                else if (m.moveX == 0f)
                {
                    m.changeState(m.standState);
                }
                else
                {
                    m.changeState(m.runState);
                }
            }
        }
        public void exit(Madeline m)
        {
        }
    }
}
