using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Character;
using Celeste.Animation;

namespace Celeste.MadelineStates
{
    public class standState:IMadelineState
    {
        public void setState(Madeline m)
        {
            m._player.setCurrentAnimation(m._anima[AnimationKeys.PlayerIdle]);
        }
        public void update(Madeline m, float dt)
        {
            if(m.onGround && m.jumpPressed)
            {
                m.changeState(m.jumpState);
                return;
            }

            if (m.dashPressed)
            {
                m.changeState(m.dashState);
                return;
            }

            if (m.moveX != 0) m.changeState(m.runState);


        }
        public void exit(Madeline m)
        {

        }

    }
}
