using System;
using Celeste.Character;

namespace Celeste.MadelineStates
{
    public class standState : IMadelineState
    {
        private static readonly Random _rng = new();

        // Seconds of standing still before a fidget plays.
        private const float IdleThreshold = 12f;

        // Durations (frames / fps) for each fidget animation.
        private const float FidgetDurA = 12f / 6f;  // idleA: 12 frames @ 6fps
        private const float FidgetDurB = 24f / 6f;  // idleB: 24 frames @ 6fps
        private const float FidgetDurC = 12f / 6f;  // idleC: 12 frames @ 6fps

        private float _idleTimer  = 0f;  // time standing still before next fidget
        private float _fidgetTimer = -1f; // countdown for current fidget; -1 = not playing

        public void setState(Madeline m)
        {
            _idleTimer   = 0f;
            _fidgetTimer = -1f;
            m.Maddy.Idle();
        }

        public void update(Madeline m, float dt)
        {
            // --- Idle fidget cycling ---
            if (_fidgetTimer >= 0f)
            {
                _fidgetTimer -= dt;
                if (_fidgetTimer <= 0f)
                {
                    _fidgetTimer = -1f;
                    m.Maddy.Idle();
                }
            }
            else
            {
                _idleTimer += dt;
                if (_idleTimer >= IdleThreshold)
                {
                    _idleTimer = 0f;
                    int pick = _rng.Next(3);
                    if      (pick == 0) { m.Maddy.IdleA(); _fidgetTimer = FidgetDurA; }
                    else if (pick == 1) { m.Maddy.IdleB(); _fidgetTimer = FidgetDurB; }
                    else               { m.Maddy.IdleC(); _fidgetTimer = FidgetDurC; }
                }
            }

            // --- State transitions ---
            if (m.onGround && m.jumpPressed)      { m.changeState(m.jumpState); return; }
            if (m.dashPressed && m.canDash)        { m.changeState(m.dashState); return; }
            if (m.moveX != 0f)                     { m.changeState(m.runState);  return; }
        }

        public void exit(Madeline m)
        {
            _idleTimer   = 0f;
            _fidgetTimer = -1f;
        }
    }
}
