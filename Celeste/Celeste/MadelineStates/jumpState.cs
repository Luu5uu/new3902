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
    internal class jumpState:IMadelineState
    {
        public void setState(Madeline m)
        {
            m._player.setCurrentAnimation(m._anima[AnimationKeys.PlayerRun]);
        }
        public void update(Madeline m, float dt)
        {
            float x = m.moveX;
            if (x == 0f)
            {
                m.changeState(m.standState);
                return;
            }

            // 位移
            m.position.X += x * m.velocity * dt;
            if (x < 0f) m.effect = SpriteEffects.FlipHorizontally;
            else if (x > 0f) m.effect = SpriteEffects.None;

        }
        public void exit(Madeline m)
        {

        }
    }
}
