using System;
using Celeste.Character;

using static Celeste.PlayerConstants;

namespace Celeste.MadelineStates
{
    public class climbState : IMadelineState
    {
        public void SetState(Madeline m)
        {
            m.SetCrouching(false);
            m.FaceTowardWall();
            m.Maddy.WallSlide(restart: true);
            m.Maddy.SetClimbSweat(climbingUp: false, tired: m.IsTired, onGround: m.onGround);
            m.isClimbing = true;
            m.isDangle = false;
            m.onGround = false;
            m.velocityX = 0f;
            m.velocityY = 0f;
        }

        public void Update(Madeline m, float dt)
        {
            if (m.climbStamina <= 0f)
            {
                m.changeState(m.fallState);
                return;
            }

            if (m.ConsumeJumpPress())
            {
                int wallJumpDirection = m.GetWallJumpDirection();
                if (wallJumpDirection != 0 && System.Math.Sign(m.moveX) == wallJumpDirection)
                {
                    m.PerformWallJump();
                }
                else
                {
                    m.PerformClimbJump();
                }
                return;
            }

            if (m.canDash && m.ConsumeDashPress())
            {
                m.changeState(m.dashState);
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

            if (m.onGround && m.moveY >= 0f)
            {
                m.changeState(m.standState);
                return;
            }

            if (m.moveY < 0f)
            {
                m.Maddy.ClimbUp();
                m.Maddy.SetClimbSweat(climbingUp: true, tired: m.IsTired, onGround: m.onGround);
                m.velocityY = -PlayerClimbUpSpeed;
                m.climbStamina = Math.Max(0f, m.climbStamina - PlayerClimbUpCostPerSecond * dt);
            }
            else if (m.moveY > 0f)
            {
                m.Maddy.WallSlide();
                m.Maddy.SetClimbSweat(climbingUp: false, tired: m.IsTired, onGround: m.onGround);
                m.velocityY = PlayerClimbDownSpeed;
            }
            else
            {
                m.Maddy.WallSlide();
                m.Maddy.SetClimbSweat(climbingUp: false, tired: m.IsTired, onGround: m.onGround);
                m.velocityY = 0f;
                if (!m.onGround)
                {
                    m.climbStamina = Math.Max(0f, m.climbStamina - PlayerClimbStillCostPerSecond * dt);
                }
            }

        }

        public void Exit(Madeline m) {
            m.isClimbing = false;
            m.Maddy.ClearSweat();
        }
    }
}
