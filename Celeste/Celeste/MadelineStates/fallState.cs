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
    internal class fallState:IMadelineState
    {
        public void setState(Madeline m)
        {
            m._player.setCurrentAnimation(m._anima[AnimationKeys.PlayerFallSlow]);
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


            if (x < 0f) m.effect = SpriteEffects.FlipHorizontally;
            else if (x > 0f) m.effect = SpriteEffects.None;

            if (m.onGround)
            {
                m.changeState(m.standState);
            }

        }
        public void exit(Madeline m)
        {

        }
    }
}
