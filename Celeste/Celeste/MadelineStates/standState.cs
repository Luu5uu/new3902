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
        // Set the texture ready to draw
        public void setState(Madeline m)
        {
            m._player.setCurrentAnimation(m._anima[AnimationKeys.PlayerIdle]);
        }
        public void update(Madeline m, float dt)
        {
            // Check whether user wants to jump
            if(m.onGround && m.jumpPressed)
            {
                m.changeState(m.jumpState);
                return;
            }
            // Check whether user wants to dash
            if (m.dashPressed)
            {
                m.changeState(m.dashState);
                return;
            }

            // If user press A or D, moveX will not be not then change to runstate
            if (m.moveX != 0) m.changeState(m.runState);


        }
        public void exit(Madeline m)
        {

        }

    }
}
