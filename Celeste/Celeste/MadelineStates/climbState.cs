using System;
using Celeste.Character;

using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class climbState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.FaceTowardWall();
            m.Maddy.Dangling();
            m.isClimbing = true;
            m.isDangle = false;
            m.onGround = false;
            m.velocityY = 0f;
        }

        public void Update(Madeline m, float dt)
        {
            if (m.ConsumeJumpPress())
            {
                m.changeState(m.jumpState);
                return;
            }
            
            if (!m.climbHeld)
            {
                m.changeState(m.fallState);
                return;
            }

            if (!m.IsTouchingWall)
            {
                m.changeState(m.fallState);
                return;
            }

            m.FaceTowardWall();

            if (m.climbStamina <= 0f)
            {
                m.changeState(m.fallState);
                return;
            }

            if (m.onGround && m.moveY >= 0f)
            {
                m.changeState(m.standState);
                return;
            }

            if (m.moveY < 0f)
            {
                m.Maddy.ClimbUp();
                m.position.Y -= PlayerClimbUpSpeed * dt;
                m.climbStamina = Math.Max(0f, m.climbStamina - PlayerClimbUpCostPerSecond * dt);
            }
            else if (m.moveY > 0f)
            {
                m.Maddy.Dangling();
                m.position.Y += PlayerClimbDownSpeed * dt;
            }
            else
            {
                m.Maddy.Dangling();
                if (!m.onGround)
                {
                    m.climbStamina = Math.Max(0f, m.climbStamina - PlayerClimbStillCostPerSecond * dt);
                }
            }

        }

        public void Exit(Madeline m) {
            m.isClimbing = false;
        }
    }
}
