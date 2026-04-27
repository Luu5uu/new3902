using Celeste.MadelineStates;
using Celeste.Rewind;
using Celeste.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static Celeste.GlobalConstants;
using static Celeste.PlayerConstants;

namespace Celeste.Character
{
    public partial class Madeline
    {
        private const int MaxRewindSnapshots = 300;
        private readonly List<PlayerSnapshot> _rewindBuffer = new();

        public bool CanRewind => _rewindBuffer.Count > 1;
        public bool IsInDeathSequence => _deathEffect != null || _state == deathState;
        public bool HasRewindTrail => _rewindBuffer.Count > 1;
        private Rectangle _startGhostSourceRect;
        private Vector2 _startGhostOrigin;
        private HairRenderer.HairSnapshot _startHairSnapshot;
        private bool _hasStartHairSnapshot;
        private RewindStateKind GetCurrentRewindStateKind()
        {
            if (_state == standState) return RewindStateKind.Stand;
            if (_state == runState) return RewindStateKind.Run;
            if (_state == jumpState) return RewindStateKind.Jump;
            if (_state == fallState) return RewindStateKind.Fall;
            if (_state == dashState) return RewindStateKind.Dash;
            if (_state == dangleState) return RewindStateKind.Dangle;
            if (_state == starFlyState) return RewindStateKind.StarFly;
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
                RewindStateKind.StarFly => starFlyState,
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
                IsStarFlying = isStarFlying,
                CanDash = canDash,
                ClimbStamina = climbStamina,

                JumpBufferTimer = _jumpBufferTimer,
                DashBufferTimer = _dashBufferTimer,
                JumpGraceTimer = _jumpGraceTimer,
                VariableJumpTimer = _variableJumpTimer,
                VariableJumpSpeed = _variableJumpSpeed,
                StoredLiftSpeed = _storedLiftSpeed,
                StoredLiftTimer = _storedLiftTimer,
                ClimbJumpConversionTimer = _climbJumpConversionTimer,
                ClimbJumpConversionDirection = _climbJumpConversionDirection,
                ClimbJumpRefundAmount = _climbJumpRefundAmount,
                StarFlyTimer = _starFlyTimer,
                StarFlyTransformTimer = _starFlyTransformTimer,
                StarFlyAngle = _starFlyAngle,
                StarFlyCurrentSpeed = _starFlyCurrentSpeed,
                StarFlySpeedLerp = _starFlySpeedLerp,
                StarFlyTransforming = _starFlyTransforming,

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
            isStarFlying = snapshot.IsStarFlying;
            canDash = snapshot.CanDash;
            climbStamina = snapshot.ClimbStamina;

            _jumpBufferTimer = snapshot.JumpBufferTimer;
            _dashBufferTimer = snapshot.DashBufferTimer;
            _jumpGraceTimer = snapshot.JumpGraceTimer;
            _variableJumpTimer = snapshot.VariableJumpTimer;
            _variableJumpSpeed = snapshot.VariableJumpSpeed;
            _storedLiftSpeed = snapshot.StoredLiftSpeed;
            _storedLiftTimer = snapshot.StoredLiftTimer;
            _climbJumpConversionTimer = snapshot.ClimbJumpConversionTimer;
            _climbJumpConversionDirection = snapshot.ClimbJumpConversionDirection;
            _climbJumpRefundAmount = snapshot.ClimbJumpRefundAmount;
            _starFlyTimer = snapshot.StarFlyTimer;
            _starFlyTransformTimer = snapshot.StarFlyTransformTimer;
            _starFlyAngle = snapshot.StarFlyAngle;
            _starFlyCurrentSpeed = snapshot.StarFlyCurrentSpeed;
            _starFlySpeedLerp = snapshot.StarFlySpeedLerp;
            _starFlyTransforming = snapshot.StarFlyTransforming;

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
                    last.IsStarFlying == snapshot.IsStarFlying &&
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

            var (src, origin) = Maddy.BodyCurrentFrame;
            _startGhostSourceRect = src;
            _startGhostOrigin = origin;

            if (Maddy.Hair is HairRenderer hairRenderer)
            {
                _startHairSnapshot = hairRenderer.CaptureSnapshot();
                _hasStartHairSnapshot = true;
            }
            else
            {
                _hasStartHairSnapshot = false;
            }
        }

        public void DrawRewindTrail(SpriteBatch spriteBatch, Texture2D pixel)
        {
            Vector2 bodyOffset = new Vector2(0f, -PlayerNormalHitboxHeight * 0.5f);
            PlayerSnapshot start = _rewindBuffer[0];
            float startScaleX = start.FaceLeft ? -DefaultScale : DefaultScale;
            Color startColor = new Color(173, 216, 230) * 0.55f;

            if (_ghostBodyTex != null)
            {
                spriteBatch.Draw(
                    _ghostBodyTex,
                    start.Position,
                    _startGhostSourceRect,
                    startColor,
                    0f,
                    _startGhostOrigin,
                    new Vector2(startScaleX, DefaultScale),
                    SpriteEffects.None,
                    0f);
            }

            if (_hasStartHairSnapshot && Maddy.Hair is HairRenderer hairRenderer)
            {
                hairRenderer.DrawSnapshot(spriteBatch, _startHairSnapshot, startColor, DefaultScale);
            }

            if (pixel == null)
            {
                return;
            }

            for (int i = 0; i < _rewindBuffer.Count - 1; i++)
            {
                Vector2 p1 = _rewindBuffer[i].Position + bodyOffset;
                Vector2 p2 = _rewindBuffer[i + 1].Position + bodyOffset;

                DrawLine(spriteBatch, pixel, p1, p2, new Color(120, 200, 255) * 0.75f, 2f);
            }
        }
        private void DrawLine(SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)System.Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();

            spriteBatch.Draw(
                pixel,
                start,
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(length, thickness),
                SpriteEffects.None,
                0f);
        }
    }
}
