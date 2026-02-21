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
                return;
            }
            if (m.climbHeld) 
            { 
                m.changeState(m.climbState); 
                return; 
            }

            if (m.dashPressed && m.canDash)
            {
                m.changeState(m.dashState);
                return;
            }

            if (m.moveX != 0)
            {
                m.changeState(m.fallState);
                return;
            }

            if (!m.onGround)
            {
                
                m.position.Y += m.dangleFallSpeed*dt;
            }
            else if (m.onGround)
            {
                m.changeState(m.standState);
            }
        }

        public void Exit(Madeline m) {
            m.isDangle = false;
        }
    }
}

