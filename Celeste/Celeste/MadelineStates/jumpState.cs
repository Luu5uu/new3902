using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Animation;
using Celeste.Character;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.MadelineStates
{
    public class jumpState:IMadelineState
    {
        public void setState(Madeline m)
        {
            m.velocityY = -m.jumpSpeed;
            m.onGround = false;
            m._player.setCurrentAnimation(m._anima[AnimationKeys.PlayerJumpFast]);
        }
        public void update(Madeline m, float dt)
        {
            if (m.dashPressed)
            {
                m.changeState(m.dashState);
                return;
            }
            var x = m.moveX * m.airSpeed * dt;
            m.position.X += x;

            if (m.velocityY > 0)
            {
                m.changeState(m.fallState);
            }

            if (x < 0f) m.effect = SpriteEffects.FlipHorizontally;
            else if (x > 0f) m.effect = SpriteEffects.None;
        }
        public void exit(Madeline m)
        {

        }
    }
}
