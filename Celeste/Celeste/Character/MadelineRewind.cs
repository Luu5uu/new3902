using System.Collections.Generic;
using Celeste.MadelineStates;
using Celeste.Rewind;
using Microsoft.Xna.Framework;

namespace Celeste.Character
{
    public partial class Madeline
    {
        private const int MaxRewindSnapshots = 300;
        private readonly List<PlayerSnapshot> _rewindBuffer = new();

        public bool CanRewind => _rewindBuffer.Count > 1;
        public bool IsInDeathSequence => _deathEffect != null || _state == deathState;
        private RewindStateKind GetCurrentRewindStateKind()
        {
            if (_state == standState) return RewindStateKind.Stand;
            if (_state == runState) return RewindStateKind.Run;
            if (_state == jumpState) return RewindStateKind.Jump;
            if (_state == fallState) return RewindStateKind.Fall;
            if (_state == dashState) return RewindStateKind.Dash;
            if (_state == dangleState) return RewindStateKind.Dangle;
            if (_state == climbState) return RewindStateKind.Climb;
            if (_state == crouchState) return RewindStateKind.Crouch;

            return RewindStateKind.Stand;
        }

        private void SetStateFromRewindKind(RewindStateKind kind)
        {
            IMadelineState target = kind switch
            {
                RewindStateKind.Stand => standState,
                RewindStateKind.Run => runState,
                RewindStateKind.Jump => jumpState,
                RewindStateKind.Fall => fallState,
                RewindStateKind.Dash => dashState,
                RewindStateKind.Dangle => dangleState,
                RewindStateKind.Climb => climbState,
                RewindStateKind.Crouch => crouchState,
                _ => standState
            };

            if (_state != target)
            {
                changeState(target);
            }
        }

        public PlayerSnapshot CaptureSnapshot()
        {
            return new PlayerSnapshot
            {
                Position = position,
                VelocityX = velocityX,
                VelocityY = velocityY,
                FaceLeft = FaceLeft,

                OnGround = onGround,
                HitCeiling = hitCeiling,
                TouchingLeftWall = touchingLeftWall,
                TouchingRightWall = touchingRightWall,
                IsCrouching = isCrouching,

                IsClimbing = isClimbing,
                IsDashing = isDashing,
                IsDangle = isDangle,
                CanDash = canDash,
                ClimbStamina = climbStamina,

                JumpBufferTimer = _jumpBufferTimer,
                DashBufferTimer = _dashBufferTimer,
                JumpGraceTimer = _jumpGraceTimer,
                VariableJumpTimer = _variableJumpTimer,
                VariableJumpSpeed = _variableJumpSpeed,

                LastAim = _lastAim,
                DashRecoveryQueued = _dashRecoveryQueued,
                DashRecoveryDirection = _dashRecoveryDirection,

                LedgeTopOutQueued = _ledgeTopOutQueued,
                LedgeTopOutPosition = _ledgeTopOutPosition,
                LedgeTopOutTimer = _ledgeTopOutTimer,

                FootstepTimer = footstepTimer,
                ClimbSoundTimer = climbSoundTimer,
                TiredFlashPhase = _tiredFlashPhase,

                StateKind = GetCurrentRewindStateKind()
            };
        }

        public void ApplySnapshot(PlayerSnapshot snapshot)
        {
            position = snapshot.Position;
            velocityX = snapshot.VelocityX;
            velocityY = snapshot.VelocityY;
            FaceLeft = snapshot.FaceLeft;

            onGround = snapshot.OnGround;
            hitCeiling = snapshot.HitCeiling;
            touchingLeftWall = snapshot.TouchingLeftWall;
            touchingRightWall = snapshot.TouchingRightWall;
            isCrouching = snapshot.IsCrouching;

            isClimbing = snapshot.IsClimbing;
            isDashing = snapshot.IsDashing;
            isDangle = snapshot.IsDangle;
            canDash = snapshot.CanDash;
            climbStamina = snapshot.ClimbStamina;

            _jumpBufferTimer = snapshot.JumpBufferTimer;
            _dashBufferTimer = snapshot.DashBufferTimer;
            _jumpGraceTimer = snapshot.JumpGraceTimer;
            _variableJumpTimer = snapshot.VariableJumpTimer;
            _variableJumpSpeed = snapshot.VariableJumpSpeed;

            _lastAim = snapshot.LastAim;
            _dashRecoveryQueued = snapshot.DashRecoveryQueued;
            _dashRecoveryDirection = snapshot.DashRecoveryDirection;

            _ledgeTopOutQueued = snapshot.LedgeTopOutQueued;
            _ledgeTopOutPosition = snapshot.LedgeTopOutPosition;
            _ledgeTopOutTimer = snapshot.LedgeTopOutTimer;

            footstepTimer = snapshot.FootstepTimer;
            climbSoundTimer = snapshot.ClimbSoundTimer;
            _tiredFlashPhase = snapshot.TiredFlashPhase;

            SetStateFromRewindKind(snapshot.StateKind);

            jumpPressed = false;
            jumpHeld = false;
            dashPressed = false;
            deathPressed = false;
            climbHeld = false;
            moveX = 0f;
            moveY = 0f;
        }

        public void SaveRewindSnapshot()
        {
            if (_deathEffect != null)
            {
                return;
            }

            PlayerSnapshot snapshot = CaptureSnapshot();

            if (_rewindBuffer.Count > 0)
            {
                PlayerSnapshot last = _rewindBuffer[_rewindBuffer.Count - 1];

                bool sameAsLast =
                    last.Position == snapshot.Position &&
                    last.VelocityX == snapshot.VelocityX &&
                    last.VelocityY == snapshot.VelocityY &&
                    last.FaceLeft == snapshot.FaceLeft &&
                    last.OnGround == snapshot.OnGround &&
                    last.IsClimbing == snapshot.IsClimbing &&
                    last.IsDashing == snapshot.IsDashing &&
                    last.IsDangle == snapshot.IsDangle &&
                    last.IsCrouching == snapshot.IsCrouching &&
                    last.StateKind == snapshot.StateKind;

                if (sameAsLast)
                {
                    return;
                }
            }

            _rewindBuffer.Add(snapshot);

            if (_rewindBuffer.Count > MaxRewindSnapshots)
            {
                _rewindBuffer.RemoveAt(0);
            }
        }

        public bool StepRewind()
        {
            if (_rewindBuffer.Count <= 1)
            {
                return false;
            }

            _rewindBuffer.RemoveAt(_rewindBuffer.Count - 1);
            PlayerSnapshot previous = _rewindBuffer[_rewindBuffer.Count - 1];
            ApplySnapshot(previous);
            return true;
        }

        public void ClearRewindHistory()
        {
            _rewindBuffer.Clear();
        }

        public void SeedInitialRewindSnapshot()
        {
            _rewindBuffer.Clear();
            SaveRewindSnapshot();
        }
    }
}