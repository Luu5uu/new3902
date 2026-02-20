using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Character;

namespace Celeste.MadelineStates
{
    internal class danglingState:IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.Maddy.Dangling();
        }

        public void Update(Madeline m, float dt)
        {
            if (!m.onGround)
            {
                m.position.Y += m.dangleFallSpeed;
            }
            else if (m.onGround)
            {
                m.changeState(m.standState);
            }
        }

        public void Exit(Madeline m) { }
    }
}

