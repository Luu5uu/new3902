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
    public partial class Madeline : Celeste.GamePlay.IUpdateable, Celeste.GamePlay.IDrawable, ICollider
    {
        public MaddySprite Maddy { get; private set; }

        // State machine
        public IMadelineState _state;
        public IMadelineState standState;
        public IMadelineState runState;
        public IMadelineState jumpState;
        public IMadelineState springState;
        public IMadelineState fallState;
        public IMadelineState dashState;
        public IMadelineState dangleState;
        public IMadelineState starFlyState;

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
        private Vector2 _storedLiftSpeed;
        private float _storedLiftTimer;
        private float _climbJumpConversionTimer;
        private int _climbJumpConversionDirection;
        private float _climbJumpRefundAmount;
        private float _starFlyTimer;
        private float _starFlyTransformTimer;
        private float _starFlyAngle;
        private float _starFlyCurrentSpeed;
        private float _starFlySpeedLerp;
        private bool _starFlyTransforming;
        private Color _starFlyColor = StarFlyGold;
        private Vector2 _starFlyStartDirection = new Vector2(0f, -1f);
        private float _wallJumpCooldownLeft;
        private float _wallJumpCooldownRight;
        private const float WallJumpSameSideCooldown = 0.15f;
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
            return GetBoundsAt(targetPosition, crouching, isStarFlying);
        }

        private Rectangle GetBoundsAt(Vector2 targetPosition, bool crouching, bool starFlying)
        {
            int width = starFlying ? PlayerStarFlyHitboxWidth : PlayerHitboxWidth;
            int height = starFlying
                ? PlayerStarFlyHitboxHeight
                : (crouching ? PlayerDuckHitboxHeight : PlayerNormalHitboxHeight);
            int left = (int)(targetPosition.X - width / 2f);
            int bottom = starFlying
                ? (int)(targetPosition.Y - PlayerStarFlyHitboxBottomInset)
                : (int)targetPosition.Y;
            int top = bottom - height;
            return new Rectangle(left, top, width, height);
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
        public bool climbHeldForCollision;
        public bool isStarFlying;
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
        private Texture2D _starFlyDotTex;
        private HairRenderer _starFlyTail;
        private ParticleSystem _dashParticles;
        private BurstEmitter _dashBurstEmitter;
        private OrbitRingEffect _dashRingEffect;
        private SmokeParticleSystem _landDustParticles;
        private Texture2D[] _smokeFrames;
        private static readonly Color StarFlyGold = new Color(255, 214, 92);
        private static readonly Color StarFlyRed = new Color(255, 78, 78);
        private static readonly Color DashTrailCyan = new Color(74, 204, 255);
        private static readonly Random ParticleRng = new();

        public void AddGhost(Vector2 pos, bool faceLeft) =>
            _ghosts.Add(new GhostFrame { Position = pos, FaceLeft = faceLeft, Alpha = 0.6f });

        // Sound Effect
        public IBlocks CurrentGroundBlock { get; set; }

        private float footstepTimer = 0f;
        private const float FootstepInterval = 0.12f;

        private float climbSoundTimer = 0f;
        private const float ClimbSoundInterval = 0.35f;

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
            springState = new springState();
            fallState = new fallState();
            dashState = new dashState();
            dangleState = new dangleState();
            starFlyState = new StarFlyState();
            _starFlyDotTex = content.Load<Texture2D>("hair00");
            _starFlyTail = new HairRenderer();
            _starFlyTail.LoadContent(content);
            _starFlyTail.DrawScale = DefaultScale;
            _smokeFrames = new[]
            {
                content.Load<Texture2D>("smoke0"),
                content.Load<Texture2D>("smoke1"),
                content.Load<Texture2D>("smoke2"),
                content.Load<Texture2D>("smoke3")
            };

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
            _landDustParticles = new SmokeParticleSystem(_smokeFrames);
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
            _storedLiftSpeed = Vector2.Zero;
            _storedLiftTimer = 0f;
            _wallJumpCooldownLeft = 0f;
            _wallJumpCooldownRight = 0f;
            _climbJumpConversionTimer = 0f;
            _climbJumpConversionDirection = 0;
            _climbJumpRefundAmount = 0f;
            _starFlyTimer = 0f;
            _starFlyTransformTimer = 0f;
            _starFlyAngle = -MathHelper.PiOver2;
            _starFlyCurrentSpeed = 0f;
            _starFlySpeedLerp = 0f;
            _starFlyTransforming = false;
            _starFlyColor = StarFlyGold;
            _starFlyStartDirection = new Vector2(0f, -1f);
            _tiredFlashPhase = 0f;
            _dashParticles = _deathDotTex != null ? new ParticleSystem(_deathDotTex) : null;
            _landDustParticles?.Clear();
            _dashRingEffect = null;

            velocityY = 0f;
            velocityX = 0f;
            onGround = false;
            hitCeiling = false;

            isClimbing = false;
            isDashing = false;
            isStarFlying = false;
            climbHeldForCollision = false;
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
            _landDustParticles?.Update(dt);
            UpdateStarFlyTrail(gameTime);
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
            climbHeldForCollision = climbHeld;
            if (ApplyQueuedTopOut(dt))
            {
                ClearTransientInput();
                return;
            }
            ApplyQueuedDashRecovery();

            // State logic
            _state.Update(this, dt);
            UpdateClimbJumpConversion(dt);

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

            if (_storedLiftTimer > 0f)
            {
                _storedLiftTimer = Math.Max(0f, _storedLiftTimer - dt);
                if (_storedLiftTimer <= 0f)
                {
                    _storedLiftSpeed = Vector2.Zero;
                }
            }

            if (_wallJumpCooldownLeft > 0f) _wallJumpCooldownLeft = Math.Max(0f, _wallJumpCooldownLeft - dt);
            if (_wallJumpCooldownRight > 0f) _wallJumpCooldownRight = Math.Max(0f, _wallJumpCooldownRight - dt);

            if (!isDashing && !isClimbing && !isDangle && !isStarFlying)
            {
                UpdateHorizontalVelocity(dt);
            }
            else if (!isDashing && !isStarFlying)
            {
                velocityX = Approach(velocityX, 0f, PlayerDashCarryDeceleration * dt);
            }

            if (!isDashing && !isClimbing && !isDangle && !isStarFlying)
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

            if (IsTired)
            {
                _tiredFlashPhase += TiredFlashSpeed * dt;
            }
            else
            {
                _tiredFlashPhase = 0f;
            }

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
            _landDustParticles?.Draw(spriteBatch);
            if (isStarFlying)
            {
                DrawStarFlyBody(spriteBatch, drawOutline: true);
                DrawStarFlyTrail(spriteBatch);
                DrawStarFlyBody(spriteBatch, drawOutline: false);
                return;
            }

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

        public void SpawnLandDust(Vector2 pos)
        {
            if (_landDustParticles == null) return;

            Vector2 origin = pos + new Vector2(0f, -2f);
            for (int i = 0; i < 8; i++)
            {
                float spreadAngle = MathHelper.ToRadians(-90f + RandomRange(-75f, 75f));
                float speed = RandomRange(32f, 46f);
                var particle = new SmokeParticle
                {
                    Position = origin + new Vector2(RandomRange(-2f, 2f), RandomRange(-1f, 1f)),
                    Velocity = new Vector2((float)Math.Cos(spreadAngle) * speed, (float)Math.Sin(spreadAngle) * speed),
                    Acceleration = new Vector2(0f, 5f),
                    Damping = RandomRange(21.5f, 22.5f),
                    Age = 0f,
                    Lifetime = 0.5f,
                    StartScale = RandomRange(0.25f, 0.65f) * DefaultScale,
                    EndScale = 0f,
                    StartAlpha = 1.0f,
                    EndAlpha = 0f,
                    Tint = Color.White,
                    Rotation = RandomRange(0f, MathHelper.TwoPi),
                    AngularVelocity = RandomRange(-3f, 3f),
                    Effects = RandomFlip()
                };

                _landDustParticles.Add(particle);
            }
        }

        public void SpawnWallKickDust(Vector2 pos, int wallDir)
        {
            if (_landDustParticles == null) return;

            float wallSide = Math.Sign(wallDir);
            if (wallSide == 0f)
            {
                wallSide = FaceLeft ? -1f : 1f;
            }

            Vector2 origin = pos + new Vector2(wallSide * 7f, -PlayerNormalHitboxHeight * 0.55f);
            for (int i = 0; i < 6; i++)
            {
                float baseAngle = wallSide > 0f ? MathHelper.Pi : 0f;
                float spreadAngle = baseAngle + MathHelper.ToRadians(RandomRange(-70f, 70f));
                float speed = RandomRange(28f, 42f);
                var particle = new SmokeParticle
                {
                    Position = origin + new Vector2(RandomRange(-1f, 1f), RandomRange(-3f, 3f)),
                    Velocity = new Vector2((float)Math.Cos(spreadAngle) * speed, (float)Math.Sin(spreadAngle) * speed - 8f),
                    Acceleration = new Vector2(0f, 5f),
                    Damping = RandomRange(21.5f, 22.5f),
                    Age = 0f,
                    Lifetime = 0.45f,
                    StartScale = RandomRange(0.25f, 0.6f) * DefaultScale,
                    EndScale = 0f,
                    StartAlpha = 1.0f,
                    EndAlpha = 0f,
                    Tint = Color.White,
                    Rotation = RandomRange(0f, MathHelper.TwoPi),
                    AngularVelocity = RandomRange(-3f, 3f),
                    Effects = RandomFlip()
                };

                _landDustParticles.Add(particle);
            }
        }

        public void SpawnDashTrail(Vector2 pos, Vector2 dashDir)
        {
            if (_dashParticles == null) return;
            var rng = new Random();
            float baseAngle = (float)Math.Atan2(dashDir.Y, dashDir.X);
            for (int i = 0; i < 2; i++)
            {
                float spreadAngle = baseAngle + MathHelper.ToRadians((float)(rng.NextDouble() * 30f - 15f));
                float speed = 12f + (float)(rng.NextDouble() * 18f);
                var p = new Particle
                {
                    Position = pos,
                    Velocity = new Vector2((float)Math.Cos(spreadAngle) * speed, (float)Math.Sin(spreadAngle) * speed),
                    Age = 0f,
                    Lifetime = 0.2f + (float)(rng.NextDouble() * 0.1f),
                    StartSize = 0.35f,
                    EndSize = 0f,
                    StartAlpha = 0.9f,
                    EndAlpha = 0f,
                    StartTint = DashTrailCyan,
                    EndTint = DashTrailCyan,
                    Tint = DashTrailCyan,
                };
                _dashParticles.Add(p);
            }
        }

        private static float RandomRange(float min, float max)
        {
            return min + (float)ParticleRng.NextDouble() * (max - min);
        }

        private static SpriteEffects RandomFlip()
        {
            bool horizontal = ParticleRng.Next(2) == 0;
            bool vertical = ParticleRng.Next(2) == 0;
            return (horizontal, vertical) switch
            {
                (true, true) => SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically,
                (true, false) => SpriteEffects.FlipHorizontally,
                (false, true) => SpriteEffects.FlipVertically,
                _ => SpriteEffects.None
            };
        }

        private void UpdateStarFlyTrail(GameTime gameTime)
        {
            if (!isStarFlying || _starFlyTail == null)
            {
                return;
            }

            _starFlyTail.DrawScale = DefaultScale;
            _starFlyTail.UpdateFloatingTail(gameTime, GetStarFlyCenter(), GetStarFlyTailDirection());
        }

        private void DrawStarFlyTrail(SpriteBatch spriteBatch)
        {
            if (!isStarFlying)
            {
                return;
            }

            _starFlyTail?.DrawTail(spriteBatch, _starFlyColor, DefaultScale);
        }

        private void DrawStarFlyBody(SpriteBatch spriteBatch, bool drawOutline = true)
        {
            if (_starFlyDotTex == null)
            {
                Maddy.Draw(spriteBatch, position, _starFlyColor, scale: DefaultScale, faceLeft: FaceLeft);
                return;
            }

            Vector2 center = GetStarFlyCenter();
            Vector2 origin = new Vector2(_starFlyDotTex.Width / 2f, _starFlyDotTex.Height / 2f);
            float scale = 0.9f * DefaultScale;
            Vector2[] outlineOffsets =
            {
                new Vector2(DefaultScale, 0f),
                new Vector2(-DefaultScale, 0f),
                new Vector2(0f, DefaultScale),
                new Vector2(0f, -DefaultScale)
            };

            if (drawOutline)
            {
                foreach (Vector2 offset in outlineOffsets)
                {
                    spriteBatch.Draw(
                        _starFlyDotTex,
                        center + offset,
                        null,
                        Color.Black * 0.75f,
                        0f,
                        origin,
                        scale,
                        SpriteEffects.None,
                        0f);
                }
            }

            spriteBatch.Draw(
                _starFlyDotTex,
                center,
                null,
                _starFlyColor,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f);
        }

        private void ResetStarFlyTail(Vector2 direction)
        {
            if (_starFlyTail == null)
            {
                return;
            }

            _starFlyTail.DrawScale = DefaultScale;
            _starFlyTail.ResetFloatingTail(GetStarFlyCenter(), direction);
        }

        private Vector2 GetStarFlyCenter()
        {
            return position + new Vector2(0f, -PlayerStarFlyHitboxHeight * 0.5f);
        }

        private Vector2 GetStarFlyTailDirection()
        {
            Vector2 velocity = new Vector2(velocityX, velocityY);
            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
                return velocity;
            }

            return AngleToVector(_starFlyAngle);
        }

        private Vector2 GetStarFlyInputDirection()
        {
            Vector2 input = new Vector2(moveX, moveY);
            if (input.LengthSquared() > 0.0001f)
            {
                input.Normalize();
                return input;
            }

            return new Vector2(0f, -1f);
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

        public int GetWallJumpDirection(int checkDistance = PlayerWallJumpCheckDistance)
        {
            bool wallOnRight = (touchingRightWall || IsWallNear(1, checkDistance)) && _wallJumpCooldownRight <= 0f;
            bool wallOnLeft  = (touchingLeftWall  || IsWallNear(-1, checkDistance)) && _wallJumpCooldownLeft  <= 0f;

            if (wallOnRight && !wallOnLeft) return -1;
            if (wallOnLeft && !wallOnRight) return 1;
            return 0;
        }

        public void PerformWallJump()
        {
            PerformWallJump(GetWallJumpDirection(), resetVerticalSpeed: true);
        }

        public bool TrySuperWallJump()
        {
            int direction = GetWallJumpDirection(PlayerSuperWallJumpCheckDistance);
            if (direction == 0)
            {
                return false;
            }

            SetCrouching(false);
            isClimbing = false;
            isDangle = false;
            isDashing = false;
            ClearClimbJumpConversion();
            ConsumeJumpGrace();
            velocityX = PlayerSuperWallJumpHorizontalSpeed * direction;
            velocityY = -PlayerSuperWallJumpSpeed;
            ApplyLiftBoostToVelocity();
            BeginVariableJump(PlayerSuperWallJumpVariableTime);
            FaceLeft = direction < 0;
            Maddy.JumpFast(restart: true);
            ForceAirJumpState();
            return true;
        }

        public void PerformClimbJump()
        {
            int conversionDirection = GetWallJumpDirection();

            SetCrouching(false);
            isClimbing = false;
            isDangle = false;
            ClearClimbJumpConversion();
            ConsumeJumpGrace();

            float staminaCost = Math.Min(climbStamina, PlayerClimbJumpStaminaCost);
            climbStamina = Math.Max(0f, climbStamina - staminaCost);
            _climbJumpConversionTimer = conversionDirection != 0 ? PlayerClimbJumpConversionTime : 0f;
            _climbJumpConversionDirection = conversionDirection;
            _climbJumpRefundAmount = staminaCost;

            velocityX += PlayerJumpHorizontalBoost * moveX;
            velocityY = -PlayerJumpSpeed;
            ApplyLiftBoostToVelocity();
            BeginVariableJump();
            ForceAirJumpState();
            Maddy.JumpFast(restart: true);
            Maddy.SweatJump();
        }

        private void PerformWallJump(int direction, bool resetVerticalSpeed)
        {
            if (direction == 0)
            {
                return;
            }

            SetCrouching(false);
            isClimbing = false;
            isDangle = false;
            ClearClimbJumpConversion();
            ConsumeJumpGrace();
            velocityX = PlayerWallJumpHorizontalSpeed * direction;
            if (resetVerticalSpeed)
            {
                velocityY = -PlayerJumpSpeed;
            }
            ApplyLiftBoostToVelocity();
            BeginVariableJump();
            FaceLeft = direction < 0;
            if (direction > 0) _wallJumpCooldownLeft  = WallJumpSameSideCooldown;
            else               _wallJumpCooldownRight = WallJumpSameSideCooldown;
            SpawnWallKickDust(position, -direction);
            Maddy.JumpFast(restart: true);
            ForceAirJumpState();
        }

        private bool IsWallNear(int side, int checkDistance)
        {
            Rectangle bounds = Bounds;
            for (int distance = 1; distance <= checkDistance; distance++)
            {
                Rectangle probe = bounds;
                probe.X += side * distance;

                for (int i = 0; i < _worldBlocks.Count; i++)
                {
                    IBlocks block = _worldBlocks[i];
                    if (!IsWallJumpSolid(block))
                    {
                        continue;
                    }

                    Rectangle blockBounds = block.Bounds;
                    if (blockBounds != Rectangle.Empty && probe.Intersects(blockBounds))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsWallJumpSolid(IBlocks block)
        {
            return block != null && (block is not CrushBlock crushBlock || crushBlock.CanStandOn);
        }

        private void ForceAirJumpState()
        {
            if (_state == jumpState)
            {
                return;
            }

            _state.Exit(this);
            _state = jumpState;
        }

        public void StoreLiftSpeed(Vector2 speed)
        {
            if (speed == Vector2.Zero)
            {
                return;
            }

            _storedLiftSpeed = speed;
            _storedLiftTimer = PlayerLiftMomentumStorageTime;
        }

        public Vector2 GetStoredLiftBoost()
        {
            if (_storedLiftTimer <= 0f)
            {
                return Vector2.Zero;
            }

            Vector2 boost = _storedLiftSpeed;
            if (Math.Abs(boost.X) > PlayerLiftSpeedXCap)
            {
                boost.X = PlayerLiftSpeedXCap * Math.Sign(boost.X);
            }

            if (boost.Y > 0f)
            {
                boost.Y = 0f;
            }
            else if (boost.Y < PlayerLiftSpeedYCap)
            {
                boost.Y = PlayerLiftSpeedYCap;
            }

            return boost;
        }

        public void ApplyLiftBoostToVelocity()
        {
            Vector2 boost = GetStoredLiftBoost();
            velocityX += boost.X;
            velocityY += boost.Y;
        }

        public void RefillDash()
        {
            canDash = true;
            Maddy.OnDashRefill();
        }

        public void BounceAwayFrom(Vector2 source)
        {
            Rectangle bounds = Bounds;
            Vector2 center = new Vector2(bounds.Center.X, bounds.Center.Y);
            Vector2 direction = center - source;
            if (direction == Vector2.Zero)
            {
                direction = FaceLeft ? new Vector2(-1f, 0f) : Vector2.UnitX;
            }
            else
            {
                direction.Normalize();
            }

            velocityX = direction.X * PlayerDashEndSpeed;
            velocityY = Math.Min(direction.Y * PlayerDashEndSpeed, -PlayerJumpSpeed * 0.5f);
        }

        private void UpdateClimbJumpConversion(float dt)
        {
            if (_climbJumpConversionTimer <= 0f)
            {
                return;
            }

            _climbJumpConversionTimer = Math.Max(0f, _climbJumpConversionTimer - dt);
            if (onGround || _climbJumpConversionDirection == 0)
            {
                ClearClimbJumpConversion();
                return;
            }

            if (Math.Sign(moveX) == _climbJumpConversionDirection)
            {
                climbStamina = Math.Min(PlayerClimbMaxStamina, climbStamina + _climbJumpRefundAmount);
                PerformWallJump(_climbJumpConversionDirection, resetVerticalSpeed: false);
                ClearClimbJumpConversion();
            }
            else if (_climbJumpConversionTimer <= 0f)
            {
                ClearClimbJumpConversion();
            }
        }

        private void ClearClimbJumpConversion()
        {
            _climbJumpConversionTimer = 0f;
            _climbJumpConversionDirection = 0;
            _climbJumpRefundAmount = 0f;
        }

        public void StartStarFly()
        {
            if (_state == starFlyState)
            {
                _starFlyTimer = PlayerStarFlyTime;
                _starFlyColor = StarFlyGold;
                return;
            }

            changeState(starFlyState);
        }

        public void BeginStarFly()
        {
            SetCrouching(false);
            isStarFlying = true;
            isDashing = false;
            isClimbing = false;
            isDangle = false;
            onGround = false;
            hitCeiling = false;
            canDash = true;
            Maddy.OnDashRefill();
            Maddy.ClearSweat();

            velocityX = 0f;
            velocityY = 0f;
            _starFlyTimer = PlayerStarFlyTime;
            _starFlyTransformTimer = PlayerStarFlyTransformTime;
            _starFlyTransforming = true;
            _starFlySpeedLerp = 0f;
            _starFlyCurrentSpeed = 0f;
            _starFlyStartDirection = GetStarFlyInputDirection();
            _starFlyAngle = (float)Math.Atan2(_starFlyStartDirection.Y, _starFlyStartDirection.X);
            _starFlyColor = StarFlyGold;
            ResetStarFlyTail(_starFlyStartDirection);
        }

        public void UpdateStarFly(float dt)
        {
            if (_starFlyTransforming)
            {
                velocityX = Approach(velocityX, 0f, PlayerStarFlyTransformDeceleration * dt);
                velocityY = Approach(velocityY, 0f, PlayerStarFlyTransformDeceleration * dt);
                _starFlyTransformTimer = Math.Max(0f, _starFlyTransformTimer - dt);
                if (_starFlyTransformTimer <= 0f)
                {
                    BeginStarFlyMovement();
                }
                return;
            }

            if (canDash && ConsumeDashPress())
            {
                changeState(dashState);
                return;
            }

            if (climbHeld && GetWallJumpDirection() != 0)
            {
                changeState(climbState);
                return;
            }

            if (onGround && ConsumeJumpPress())
            {
                float horizontalSign = Math.Sign((float)Math.Cos(_starFlyAngle));
                if (horizontalSign == 0f)
                {
                    horizontalSign = FaceLeft ? -1f : 1f;
                }

                velocityX = PlayerStarFlyEndHorizontalSpeed * horizontalSign;
                velocityY = -PlayerJumpSpeed;
                BeginVariableJump(PlayerStarFlyEndVariableJumpTime);
                ForceAirJumpState();
                Maddy.JumpFast(restart: true);
                return;
            }

            _starFlyTimer -= dt;
            if (_starFlyTimer <= 0f)
            {
                FinishStarFlyTimer();
                return;
            }

            _starFlyColor = _starFlyTimer < PlayerStarFlyEndWarningTime
                && ((int)(_starFlyTimer * 20f) % 2 == 0)
                    ? StarFlyRed
                    : StarFlyGold;

            Vector2 input = new Vector2(moveX, moveY);
            bool hasInput = input.LengthSquared() > 0.0001f;
            if (hasInput)
            {
                input.Normalize();
                float targetAngle = (float)Math.Atan2(input.Y, input.X);
                _starFlyAngle = ApproachAngle(_starFlyAngle, targetAngle, PlayerStarFlyTurnSpeed * dt);

                Vector2 currentDirection = AngleToVector(_starFlyAngle);
                float maxSpeed;
                if (Vector2.Dot(currentDirection, input) >= 0.45f)
                {
                    _starFlySpeedLerp = Approach(_starFlySpeedLerp, 1f, dt / PlayerStarFlyMaxLerpTime);
                    maxSpeed = MathHelper.Lerp(PlayerStarFlyTargetSpeed, PlayerStarFlyMaxSpeed, _starFlySpeedLerp);
                }
                else
                {
                    _starFlySpeedLerp = 0f;
                    maxSpeed = PlayerStarFlyTargetSpeed;
                }

                _starFlyCurrentSpeed = Approach(_starFlyCurrentSpeed, maxSpeed, PlayerStarFlyAcceleration * dt);
            }
            else
            {
                _starFlySpeedLerp = 0f;
                _starFlyCurrentSpeed = Approach(
                    _starFlyCurrentSpeed,
                    PlayerStarFlySlowSpeed,
                    PlayerStarFlyAcceleration * 0.5f * dt);
            }

            Vector2 velocity = AngleToVector(_starFlyAngle) * _starFlyCurrentSpeed;
            velocityX = velocity.X;
            velocityY = velocity.Y;
        }

        public void EndStarFly()
        {
            ResolveStarFlyExitHitbox();
            isStarFlying = false;
            _starFlyTransforming = false;
            _starFlyColor = StarFlyGold;
        }

        public bool HandleStarFlyHorizontalCollision()
        {
            if (!isStarFlying)
            {
                return false;
            }

            if (climbHeldForCollision && !IsTired)
            {
                changeState(climbState);
                return true;
            }

            velocityX = 0f;

            SyncStarFlyFromVelocity();
            return true;
        }

        public bool HandleStarFlyVerticalCollision()
        {
            if (!isStarFlying)
            {
                return false;
            }

            velocityY = 0f;

            SyncStarFlyFromVelocity();
            return true;
        }

        private void BeginStarFlyMovement()
        {
            Vector2 direction = _starFlyStartDirection;
            if (direction == Vector2.Zero)
            {
                direction = new Vector2(0f, -1f);
            }

            _starFlyAngle = (float)Math.Atan2(direction.Y, direction.X);
            _starFlyCurrentSpeed = PlayerStarFlyStartSpeed;
            Vector2 velocity = direction * _starFlyCurrentSpeed;
            velocityX = velocity.X;
            velocityY = velocity.Y;
            _starFlyTransforming = false;
            _starFlyTimer = PlayerStarFlyTime;
        }

        private void FinishStarFlyTimer()
        {
            if (moveY < 0f)
            {
                velocityY = PlayerStarFlyExitUpSpeed;
            }

            if (moveY < 1f)
            {
                BeginVariableJump(PlayerVariableJumpTime);
            }

            if (velocityY > PlayerStarFlyMaxExitY)
            {
                velocityY = PlayerStarFlyMaxExitY;
            }

            if (Math.Abs(velocityX) > PlayerStarFlyMaxExitX)
            {
                velocityX = PlayerStarFlyMaxExitX * Math.Sign(velocityX);
            }

            changeState(onGround ? (moveX == 0f ? standState : runState) : fallState);
        }

        private void SyncStarFlyFromVelocity()
        {
            Vector2 velocity = new Vector2(velocityX, velocityY);
            _starFlyCurrentSpeed = velocity.Length();
            if (velocity != Vector2.Zero)
            {
                _starFlyAngle = (float)Math.Atan2(velocity.Y, velocity.X);
            }
        }

        private void ResolveStarFlyExitHitbox()
        {
            if (!OverlapsWorld(GetBoundsAt(position, isCrouching, starFlying: false)))
            {
                return;
            }

            Vector2 start = position;

            position.Y -= PlayerStarFlyHitboxBottomInset;
            if (!OverlapsWorld(GetBoundsAt(position, isCrouching, starFlying: false)))
            {
                return;
            }

            position = start;
            isCrouching = true;
            position.Y -= PlayerStarFlyHitboxBottomInset;
            if (!OverlapsWorld(GetBoundsAt(position, crouching: true, starFlying: false)))
            {
                return;
            }

            position = start;
            isCrouching = false;
        }

        private bool OverlapsWorld(Rectangle bounds)
        {
            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                IBlocks block = _worldBlocks[i];
                if (!IsWallJumpSolid(block))
                {
                    continue;
                }

                Rectangle blockBounds = block.Bounds;
                if (blockBounds != Rectangle.Empty && bounds.Intersects(blockBounds))
                {
                    return true;
                }
            }

            return false;
        }

        private static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        private static float ApproachAngle(float current, float target, float maxDelta)
        {
            float difference = WrapAngle(target - current);
            if (Math.Abs(difference) <= maxDelta)
            {
                return target;
            }

            return current + Math.Sign(difference) * maxDelta;
        }

        private static float WrapAngle(float angle)
        {
            while (angle > MathHelper.Pi)
            {
                angle -= MathHelper.TwoPi;
            }

            while (angle < -MathHelper.Pi)
            {
                angle += MathHelper.TwoPi;
            }

            return angle;
        }

        public void LaunchFromSpring(float launchSpeed = PlayerSpringLaunchSpeed)
        {
            SetCrouching(false);
            isClimbing = false;
            isDangle = false;
            isDashing = false;
            onGround = false;
            hitCeiling = false;
            CurrentGroundBlock = null;
            _dashRecoveryQueued = false;
            _ledgeTopOutQueued = false;
            _variableJumpTimer = 0f;
            _variableJumpSpeed = 0f;
            velocityY = -launchSpeed;
            changeState(springState);
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

        public void BeginVariableJump(float duration = PlayerVariableJumpTime)
        {
            _variableJumpTimer = duration;
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

        public void UpdateClimbSound(float dt)
        {
            bool shouldPlay =
                isClimbing &&
                IsTouchingWall &&
                !onGround &&
                !isDashing &&
                !isDangle &&
                !IsTired;

            if (!shouldPlay)
            {
                climbSoundTimer = 0f;
                return;
            }

            climbSoundTimer += dt;

            if (climbSoundTimer >= ClimbSoundInterval)
            {
                climbSoundTimer = 0f;
                SoundManager.PlaySequence("climb");
            }
        }
    }
}
