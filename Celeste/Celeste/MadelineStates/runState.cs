using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Animation;
using Celeste.Character;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Celeste.MadelineStates
{
    public class runState:IMadelineState
    {

        // Set the texture ready to draw
        public void setState(Madeline m)
        {
            m._player.setCurrentAnimation(m._anima[AnimationKeys.PlayerRun]);
        }
        public void update(Madeline m, float dt)
        {
            // Change state
            if (m.onGround && m.jumpPressed)
            {
                m.changeState(m.jumpState);
                return;
            }

            // If user doesn't press any key, the sprite will be in stand state.
            float x = m.moveX;
            if (x == 0f)
            {
                m.changeState(m.standState);
                return;
            }

            // Movement horizontally and change of facing side
            m.position.X += x * m.velocity * dt;
            if (x < 0f) m.effect = SpriteEffects.FlipHorizontally;
            else if(x > 0f) m.effect = SpriteEffects.None;

        }
        public void exit(Madeline m)
        {

        }
    }
}
