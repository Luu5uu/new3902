using System;
using System.Collections.Generic;
using Celeste.Animation;
using Celeste.AudioSystem;
using Celeste.Blocks;
using Celeste.DeathAnimation;
using Celeste.DeathAnimation.Particles;
using Celeste.MadelineStates;
using Celeste.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.DeathAnimation.Particles.Emitters;
using Microsoft.Xna.Framework.Content;
using static Celeste.PlayerConstants;
using static Celeste.GlobalConstants;
using static Celeste.DeathConstants;

namespace Celeste.Character
{
    public class Madeline : Celeste.GamePlay.IUpdateable, Celeste.GamePlay.IDrawable, ICollider
    {
        public MaddySprite Maddy { get; private set; }

        // State machine
        public IMadelineState _state;
        public IMadelineState standState;
        public IMadelineState runState;
        public IMadelineState jumpState;
        public IMadelineState fallState;
        public IMadelineState dashState;
        public IMadelineState dangleState;

        public IMadelineState deathState;

        //New
        public IMadelineState climbState;
        public IMadelineState crouchState;

        // Set each frame by input layer via SetMovementCommand; consumed in Update.
        public bool jumpPressed;
        public bool jumpHeld;
        public bool dashPressed;
        public bool deathPressed;
        public bool climbHeld;
        public float moveX;
        public float moveY;
        private float _jumpBufferTimer;
        private float _dashBufferTimer;
        private float _jumpGraceTimer;
        private float _variableJumpTimer;
        private float _variableJumpSpeed;
        private Vector2 _lastAim = Vector2.UnitX;
        private bool _dashRecoveryQueued;
        private Vector2 _dashRecoveryDirection;
        private bool _ledgeTopOutQueued;
        private Vector2 _ledgeTopOutPosition;
        private float _ledgeTopOutTimer;
        private readonly List<IBlocks> _worldBlocks = new();
        //
        public float ClimbStaminaPercent => climbStamina / PlayerClimbMaxStamina;
        public float ClimbTiredPercent => PlayerClimbTiredThreshold / PlayerClimbMaxStamina;
        private float _tiredFlashPhase;
        private const float TiredFlashSpeed = 12f;


        // Position & facing
        public Vector2 position;
        public bool FaceLeft;
        public float ground;
        public Vector2 RespawnPoint;

        public Rectangle Bounds
        {
            get
            {
                return GetBoundsAt(position, isCrouching);
            }
        }

        public Rectangle GetBoundsAt(Vector2 targetPosition, bool crouching)
        {
            int height = crouching ? PlayerDuckHitboxHeight : PlayerNormalHitboxHeight;
            int left = (int)(targetPosition.X - PlayerHitboxWidth / 2f);
            int top = (int)(targetPosition.Y - height);
            return new Rectangle(left, top, PlayerHitboxWidth, height);
        }


        // Physics (runtime state only — speed constants live in PlayerConstants)
        public float velocityX;
        public float velocityY;
        public bool onGround;
        public bool hitCeiling;

        //Climb
        public bool isClimbing;
        public bool touchingLeftWall;
        public bool touchingRightWall;
        public bool IsTouchingWall => touchingLeftWall || touchingRightWall;
        public float climbStamina = PlayerClimbMaxStamina;
        public bool IsTired => climbStamina <= PlayerClimbTiredThreshold;
        public bool isCrouching;
        // Dash
        public bool isDashing;
        public bool canDash = true;
        //Dangling
        public bool isDangle;
        public float dangleFallSpeed = PlayerDangleFallSpeed;

        private AnimationClip _deathSideClip;
        private AnimationClip _deathUpClip;
        private AnimationClip _deathDownClip;
        private Texture2D _deathDotTex;
        public DeathEffect _deathEffect;
        private bool _levelResetRequested;

        private struct GhostFrame { public Vector2 Position; public bool FaceLeft; public float Alpha; }
        private readonly List<GhostFrame> _ghosts = new();
        private Texture2D _ghostBodyTex;
        private ParticleSystem _dashParticles;
        private BurstEmitter _dashBurstEmitter;
        private OrbitRingEffect _dashRingEffect;

        public void AddGhost(Vector2 pos, bool faceLeft) =>
            _ghosts.Add(new GhostFrame { Position = pos, FaceLeft = faceLeft, Alpha = 0.6f });

        // Sound Effect
        public IBlocks CurrentGroundBlock { get; set; }

        private float footstepTimer = 0f;
        private const float FootstepInterval = 0.12f;

        public Madeline(ContentManager content, AnimationCatalog catalog, Vector2 startPos)
        {
            Maddy = MaddySprite.Build(content, catalog);
            position = startPos;
            RespawnPoint = startPos;
            ground = startPos.Y;
            onGround = true;

            standState = new standState();
            runState = new runState();
            jumpState = new jumpState();
            fallState = new fallState();
            dashState = new dashState();
            dangleState = new dangleState();

            deathState = new DeathState();
            climbState = new climbState();
            crouchState = new crouchState();

            _state = standState;
            _state.SetState(this);
        }

        public void ConfigureDeathAnimation(AnimationClip deathSideClip, AnimationClip deathUpClip, AnimationClip deathDownClip, Texture2D dotTexture)
        {
            _deathSideClip = deathSideClip;
            _deathUpClip = deathUpClip;
            _deathDownClip = deathDownClip;
            _deathDotTex = dotTexture;
            _dashParticles = new ParticleSystem(dotTexture);
            _dashBurstEmitter = new BurstEmitter(
                count: 8,
                minSpeed: 90f * DefaultScale,
                maxSpeed: 170f * DefaultScale,
                minLife: 0.08f,
                maxLife: 0.18f,
                minSize: 0.12f * DefaultScale,
                maxSize: 0.18f * DefaultScale,
                tint: DashDeathColor);
            BuildGhostTexture();
        }

        public void SetWorldBlocks(IEnumerable<IBlocks> worldBlocks)
        {
            _worldBlocks.Clear();
            if (worldBlocks == null)
            {
                return;
            }

            _worldBlocks.AddRange(worldBlocks);
        }

        private void BuildGhostTexture()
        {
            Texture2D source = Maddy.BodyAtlasTexture;

            Color[] pixels = new Color[source.Width * source.Height];
            source.GetData(pixels);

            // Replace every non-transparent pixel with premultiplied white
            // so the ghost can be tinted to any solid color
            for (int i = 0; i < pixels.Length; i++)
            {
                byte a = pixels[i].A;
                pixels[i] = new Color(a, a, a, a);
            }

            _ghostBodyTex = new Texture2D(source.GraphicsDevice, source.Width, source.Height);
            _ghostBodyTex.SetData(pixels);
        }

        public void Reset()
        {
            ClearDeathEffect();
            _levelResetRequested = false;
            position = RespawnPoint;
            ClearMotionState();
            canDash = true;
            climbStamina = PlayerClimbMaxStamina;
            _tiredFlashPhase = 0f;
            changeState(standState);
        }

        public void RequestLevelReset()
        {
            _levelResetRequested = true;
        }

        public bool ConsumeLevelResetRequest()
        {
            bool requested = _levelResetRequested;
            _levelResetRequested = false;
            return requested;
        }

        private void ClearMotionState()
        {
            jumpPressed = false;
            jumpHeld = false;
            dashPressed = false;
            deathPressed = false;
            climbHeld = false;
            moveX = 0f;
            moveY = 0f;
            _jumpBufferTimer = 0f;
            _dashBufferTimer = 0f;
            _jumpGraceTimer = 0f;
            _variableJumpTimer = 0f;
            _variableJumpSpeed = 0f;
            _lastAim = FaceLeft ? new Vector2(-1f, 0f) : Vector2.UnitX;
            _dashRecoveryQueued = false;
            _dashRecoveryDirection = Vector2.Zero;
            _ledgeTopOutQueued = false;
            _ledgeTopOutPosition = position;
            _ledgeTopOutTimer = 0f;
            _tiredFlashPhase = 0f;
            _dashParticles = _deathDotTex != null ? new ParticleSystem(_deathDotTex) : null;
            _dashRingEffect = null;

            velocityY = 0f;
            velocityX = 0f;
            onGround = false;
            hitCeiling = false;

            isClimbing = false;
            isDashing = false;
            isDangle = false;
            isCrouching = false;

            touchingLeftWall = false;
            touchingRightWall = false;
        }

        public Vector2 GetDashDirection()
        {
            return _lastAim;
        }

        public Vector2 ConsumeDashDirection()
        {
            return GetDashDirection();
        }

        public float GetCurrentHorizontalSpeed()
        {
            float inputSpeed = moveX * (onGround ? PlayerRunSpeed : PlayerAirSpeed);
            if (Math.Abs(inputSpeed) > Math.Abs(velocityX))
            {
                return inputSpeed;
            }

            return velocityX;
        }

        public void RefreshFacingFromInput()
        {
            if (moveX < 0f)
            {
                FaceLeft = true;
            }
            else if (moveX > 0f)
            {
                FaceLeft = false;
            }
        }

        public void changeState(IMadelineState next)
        {
            _state.Exit(this);
            _state = next;
            _state.SetState(this);
        }

        // Called by input layer each frame before Update.
        public void Move(float direction)
        {
            this.moveX = direction;
        }

        public void AimVertical(float direction)
        {
            this.moveY = direction;
        }

        public void Jump()
        {

            jumpPressed = true;
            _jumpBufferTimer = PlayerJumpBufferTime;
        }

        public void SetJumpHeld(bool held)
        {
            jumpHeld = held;
        }

        public void Dash()
        {

            dashPressed = true;
            _dashBufferTimer = PlayerDashBufferTime;
        }

        public void Die()
        {
            this.deathPressed = true;
        }

        public void SetClimb(bool held)
        {
            this.climbHeld = held;
        }

        public void Climb()
        {
            this.isClimbing = true;
        }


        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Decay ghost trail alphas
            for (int i = _ghosts.Count - 1; i >= 0; i--)
            {
                var g = _ghosts[i];
                g.Alpha -= dt * 4f;
                if (g.Alpha <= 0f) { _ghosts.RemoveAt(i); continue; }
                _ghosts[i] = g;
            }

            _dashParticles?.Update(dt);
            if (_dashRingEffect != null)
            {
                _dashRingEffect.Update(dt);
                if (_dashRingEffect.IsFinished)
                {
                    _dashRingEffect = null;
                }
            }

            if (deathPressed && _deathSideClip != null && _deathUpClip != null && _deathDownClip != null && _deathDotTex != null && _deathEffect == null)
            {
                changeState(deathState);
            }

            RefreshLastAim();
            if (ApplyQueuedTopOut(dt))
            {
                ClearTransientInput();
                return;
            }
            ApplyQueuedDashRecovery();

            // State logic
            _state.Update(this, dt);

            if (_deathEffect != null)
            {
                ClearTransientInput();
                return;
            }

            if (onGround)
            {
                climbStamina = PlayerClimbMaxStamina;
                _jumpGraceTimer = PlayerJumpGraceTime;
            }
            else if (_jumpGraceTimer > 0f)
            {
                _jumpGraceTimer = Math.Max(0f, _jumpGraceTimer - dt);
            }

            if (_jumpBufferTimer > 0f)
            {
                _jumpBufferTimer = Math.Max(0f, _jumpBufferTimer - dt);
            }

            if (_dashBufferTimer > 0f)
            {
                _dashBufferTimer = Math.Max(0f, _dashBufferTimer - dt);
            }

            if (_variableJumpTimer > 0f)
            {
                _variableJumpTimer = Math.Max(0f, _variableJumpTimer - dt);
            }

            if (!isDashing && !isClimbing && !isDangle)
            {
                UpdateHorizontalVelocity(dt);
            }
            else if (!isDashing)
            {
                velocityX = Approach(velocityX, 0f, PlayerDashCarryDeceleration * dt);
            }

            if (!isDashing && !isClimbing && !isDangle)
            {
                if (!onGround)
                {
                    float gravity = PlayerGravity;
                    if (_variableJumpTimer > 0f && jumpHeld && Math.Abs(velocityY) < PlayerHalfGravityThreshold)
                    {
                        gravity *= PlayerJumpHoldGravityMultiplier;
                    }
                    else if (!jumpHeld || velocityY >= 0f)
                    {
                        _variableJumpTimer = 0f;
                    }

                    velocityY += gravity * dt;
                    velocityY = Math.Min(velocityY, PlayerMaxFallSpeed);
                }
                else if (velocityY > 0f)
                {
                    velocityY = 0f;
                }

                if (_variableJumpTimer > 0f)
                {
                    if (jumpHeld)
                    {
                        velocityY = Math.Min(velocityY, _variableJumpSpeed);
                    }
                    else
                    {
                        _variableJumpTimer = 0f;
                    }
                }
            }

            position.X += velocityX * dt;
            position.Y += velocityY * dt;

          
            ClearTransientInput();
        }

        private void ClearTransientInput()
        {
            jumpPressed = false;
            jumpHeld = false;
            dashPressed = false;
            deathPressed = false;
            moveX = 0f;
            moveY = 0f;
            climbHeld = false;
        }

        // Call after collision resolution so the hair anchor matches the final body position.
        public void UpdateSprite(GameTime gameTime)
        {
            Maddy.SetPosition(position, scale: DefaultScale, faceLeft: FaceLeft);
            Maddy.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_deathEffect != null)
            {
                _deathEffect.Draw(spriteBatch);
                return;
            }

            if (_ghostBodyTex != null && _ghosts.Count > 0)
            {
                var (src, origin) = Maddy.BodyCurrentFrame;
                var ghostColor = new Color(44, 183, 255);
                foreach (var g in _ghosts)
                {
                    float scaleX = g.FaceLeft ? -DefaultScale : DefaultScale;
                    spriteBatch.Draw(_ghostBodyTex, g.Position, src, ghostColor * g.Alpha,
                        0f, origin, new Vector2(scaleX, DefaultScale), SpriteEffects.None, 0f);
                }
            }

            _dashRingEffect?.Draw(spriteBatch);
            _dashParticles?.Draw(spriteBatch);
            Maddy.Draw(spriteBatch, position, GetTiredFlashColor(), scale: DefaultScale, faceLeft: FaceLeft);
        }

        internal void TriggerDashVisual(Vector2 dashDirection)
        {
            if (_deathDotTex == null || _dashParticles == null || _dashBurstEmitter == null)
            {
                return;
            }

            Vector2 center = position + new Vector2(0f, -PlayerNormalHitboxHeight * 0.5f);
            _dashBurstEmitter.Emit(_dashParticles, center);

            float initialAngle = dashDirection == Vector2.Zero
                ? 0f
                : (float)Math.Atan2(dashDirection.Y, dashDirection.X);
            _dashRingEffect = new OrbitRingEffect(
                _deathDotTex,
                center,
                count: 8,
                radius: 7f * DefaultScale,
                angularSpeed: 18f,
                lifetime: 0.10f,
                dotScale: 0.14f * DefaultScale,
                color: DashDeathColor,
                initialAngle: initialAngle);
        }

        internal Vector2 GetDeathDirection()
        {
            if (isDashing)
            {
                return GetDashDirection();
            }

            Vector2 direction = new Vector2(velocityX, velocityY);
            if (direction == Vector2.Zero)
            {
                direction = FaceLeft ? new Vector2(-1f, 0f) : Vector2.UnitX;
            }

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            return direction;
        }

        internal void StartDeathEffect(bool wasDashing, Vector2 direction)
        {
            SoundManager.Play("death");
            if (_deathSideClip == null || _deathUpClip == null || _deathDownClip == null || _deathDotTex == null)
                return;



            AnimationClip clip = ResolveDeathClip(direction);

            float scale = DefaultScale;
            Vector2 topLeft = position - new Vector2(
                clip.FrameWidth * scale * 0.5f,
                clip.FrameHeight * scale
            );

            Color deathColor = Maddy.Hair.HairColor;
            if (deathColor == default)
            {
                deathColor = wasDashing ? DashDeathColor : NormalDeathColor;
            }
            bool faceLeft = direction.X < 0f;
            _deathEffect = new DeathEffect(clip, _deathDotTex, topLeft, deathColor, faceLeft, scale);
        }

        private AnimationClip ResolveDeathClip(Vector2 direction)
        {
            if (direction.Y < -0.5f)
            {
                return _deathUpClip;
            }

            if (direction.Y > 0.5f)
            {
                return _deathDownClip;
            }

            return _deathSideClip;
        }

        internal void UpdateDeathEffect(float dt)
        {
            _deathEffect?.Update(dt);
        }

        internal bool IsDeathEffectFinished()
        {
            return _deathEffect != null && _deathEffect.IsFinished;
        }

        internal void ClearDeathEffect()
        {
            _deathEffect = null;
        }

        public void SetCrouching(bool crouching)
        {
            if (!crouching && !CanStandUp())
            {
                return;
            }

            isCrouching = crouching;
        }

        public bool WantsToCrouch()
        {
            return onGround && moveY > 0f && !isClimbing && !isDashing;
        }

        public bool CanStandUp()
        {
            Rectangle standingBounds = GetBoundsAt(position, crouching: false);

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                Rectangle blockBounds = _worldBlocks[i].Bounds;
                if (blockBounds == Rectangle.Empty)
                {
                    continue;
                }

                if (standingBounds.Intersects(blockBounds))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanGrabWall()
        {
            return climbHeld && IsTouchingWall && !IsTired && !isCrouching;
        }

        public bool CanWallJump()
        {
            return !onGround && !isCrouching && GetWallJumpDirection() != 0;
        }

        public int GetWallJumpDirection()
        {
            if (touchingRightWall && !touchingLeftWall)
            {
                return -1;
            }

            if (touchingLeftWall && !touchingRightWall)
            {
                return 1;
            }

            return 0;
        }

        public void PerformWallJump()
        {
            int direction = GetWallJumpDirection();
            if (direction == 0)
            {
                return;
            }

            SetCrouching(false);
            isClimbing = false;
            isDangle = false;
            ConsumeJumpGrace();
            velocityX = PlayerWallJumpHorizontalSpeed * direction;
            velocityY = -PlayerJumpSpeed;
            BeginVariableJump();
            FaceLeft = direction < 0;
            Maddy.JumpFast(restart: true);
        }

        public void FaceTowardWall()
        {
            if (touchingLeftWall && !touchingRightWall)
            {
                FaceLeft = true;
            }
            else if (touchingRightWall && !touchingLeftWall)
            {
                FaceLeft = false;
            }
        }
        //
        private Color GetTiredFlashColor()
        {
            if (!IsTired)
            {
                return Color.White;
            }

            float wave = (float)Math.Sin(_tiredFlashPhase);

            return wave > 0f ? new Color(255, 90, 90) : Color.White;
        }


        private static float Approach(float value, float target, float maxDelta)
        {
            if (value < target)
            {
                return Math.Min(value + maxDelta, target);
            }

            return Math.Max(value - maxDelta, target);
        }

        private void UpdateHorizontalVelocity(float dt)
        {
            if (isCrouching)
            {
                velocityX = Approach(velocityX, 0f, PlayerDuckDeceleration * dt);
                return;
            }

            float max = onGround ? PlayerRunSpeed : PlayerAirSpeed;
            float mult = onGround ? 1f : PlayerAirAccelerationMultiplier;
            float targetSpeed = max * moveX;
            bool movingSameDirectionOverMax =
                moveX != 0f
                && Math.Abs(velocityX) > max
                && Math.Sign(velocityX) == Math.Sign(moveX);

            float approach = movingSameDirectionOverMax
                ? PlayerRunDeceleration * mult * dt
                : PlayerRunAcceleration * mult * dt;

            velocityX = Approach(velocityX, targetSpeed, approach);

            if (moveX != 0f)
            {
                RefreshFacingFromInput();
            }
        }

        public bool ConsumeJumpPress()
        {
            if (_jumpBufferTimer <= 0f)
            {
                return false;
            }

            _jumpBufferTimer = 0f;
            jumpPressed = false;
            return true;
        }

        public bool ConsumeDashPress()
        {
            if (_dashBufferTimer <= 0f)
            {
                return false;
            }

            _dashBufferTimer = 0f;
            dashPressed = false;
            return true;
        }

        public bool CanUseJumpGrace()
        {
            return onGround || _jumpGraceTimer > 0f;
        }

        public void ConsumeJumpGrace()
        {
            onGround = false;
            _jumpGraceTimer = 0f;
        }

        public void BeginVariableJump()
        {
            _variableJumpTimer = PlayerVariableJumpTime;
            _variableJumpSpeed = velocityY;
            SoundManager.Play("jump");
        }

        public void QueueDashRecovery(Vector2 dashDirection)
        {
            _dashRecoveryQueued = true;
            _dashRecoveryDirection = dashDirection;
        }

        public void QueueLedgeTopOut(Vector2 targetPosition)
        {
            _ledgeTopOutQueued = true;
            _ledgeTopOutPosition = targetPosition;
            _ledgeTopOutTimer = PlayerLedgeTopOutAnimationTime;
            position = targetPosition;
            velocityY = 0f;
            velocityX = 0f;
            onGround = true;
            hitCeiling = false;
            touchingLeftWall = false;
            touchingRightWall = false;
            isClimbing = false;
            isDangle = false;
            isCrouching = false;
            Maddy.ClimbPull();
            Maddy.ClearSweat();
        }

        private bool ApplyQueuedTopOut(float dt)
        {
            if (!_ledgeTopOutQueued)
            {
                return false;
            }

            position = _ledgeTopOutPosition;
            velocityX = 0f;
            velocityY = 0f;
            onGround = true;
            _ledgeTopOutTimer = Math.Max(0f, _ledgeTopOutTimer - dt);
            if (_ledgeTopOutTimer > 0f && !Maddy.IsBodyAnimationFinished)
            {
                return true;
            }

            _ledgeTopOutQueued = false;
            _ledgeTopOutTimer = 0f;
            climbStamina = PlayerClimbMaxStamina;
            canDash = true;
            Maddy.OnDashRefill();
            changeState(moveX == 0f ? standState : runState);
            return true;
        }

        private void ApplyQueuedDashRecovery()
        {
            if (!_dashRecoveryQueued)
            {
                return;
            }

            _dashRecoveryQueued = false;
            isDashing = false;

            if (_dashRecoveryDirection.Y <= 0f)
            {
                velocityX = _dashRecoveryDirection.X * PlayerDashEndSpeed;
                velocityY = hitCeiling ? 0f : _dashRecoveryDirection.Y * PlayerDashEndSpeed;

                if (velocityY < 0f)
                {
                    velocityY *= PlayerDashEndUpMultiplier;
                }
            }

            if (CanGrabWall())
            {
                changeState(climbState);
            }
            else if (onGround)
            {
                changeState(moveX == 0f ? standState : runState);
            }
            else
            {
                changeState(fallState);
            }
        }

        private void RefreshLastAim()
        {
            _lastAim = ResolveAimFromInput();
        }

        private Vector2 ResolveAimFromInput()
        {
            Vector2 aim = new Vector2(moveX, moveY);
            if (aim == Vector2.Zero)
            {
                return FaceLeft ? new Vector2(-1f, 0f) : Vector2.UnitX;
            }

            aim.Normalize();
            return aim;
        }

        public void UpdateFootstep(float dt)
        {
            bool shouldPlay =
                onGround &&
                CurrentGroundBlock != null &&
                Math.Abs(velocityX) > 5f &&
                !isDashing &&
                !isClimbing &&
                !isDangle;

            if (!shouldPlay)
            {

                return;
            }

            footstepTimer += dt;

            if (footstepTimer >= FootstepInterval)
            {
                footstepTimer = 0f;
                SoundManager.PlayFootstep(CurrentGroundBlock.Type);
            }
        }
    }
}
