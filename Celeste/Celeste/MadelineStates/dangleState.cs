using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Character;

namespace Celeste.MadelineStates
{
    internal class dangleState:IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.isDangle = true;
            m.Maddy.Dangling();
        }

        public void Update(Madeline m, float dt)
        {

            if (m.jumpPressed)
            {
                m.changeState(m.jumpState);
            }
            if (m.climbHeld) { m.changeState(m.climbState); return; }


            if (m.moveX != 0)
            {
                m.changeState(m.fallState);
            }

            if (!m.onGround)
            {
                
                m.position.Y += m.dangleFallSpeed*dt;
            }
            else if (m.onGround)
            {
                m.isClimbing = false;
                m.changeState(m.standState);
            }
        }

        public void Exit(Madeline m) {
            m.isDangle = false;
        }
    }
}

