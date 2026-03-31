using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Celeste.Animation;
using Celeste.Input;
using Celeste.MadelineStates;
using Celeste.Sprites;

using Celeste.DeathAnimation;
using Celeste.DeathAnimation.Particles;

using System;
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

        // Set each frame by input layer via SetMovementCommand; consumed in Update.
        public bool jumpPressed;
        public bool dashPressed;
        public bool deathPressed;
        public bool climbHeld;
        public float moveX;
        public float moveY;
        private float _jumpBufferTimer;
        private float _dashBufferTimer;
        private float _jumpGraceTimer;

        // Position & facing
        public Vector2 position;
        public bool FaceLeft;
        public float ground;
        public Vector2 RespawnPoint;

        private const int HitboxW = 16;
        private const int HitboxH = 32;

        public Rectangle Bounds
        {
            get
            {
                int left = (int)(position.X - HitboxW / 2f);
                int top = (int)(position.Y - HitboxH);
                return new Rectangle(left, top, HitboxW, HitboxH);
            }
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
        // Dash
        public bool isDashing;
        public bool canDash = true;
        //Dangling
        public bool isDangle;
        public float dangleFallSpeed = PlayerDangleFallSpeed;

        private AnimationClip _deathClip;
        private Texture2D _deathDotTex;
        public DeathEffect _deathEffect;
        private bool _levelResetRequested;

        private struct GhostFrame { public Vector2 Position; public bool FaceLeft; public float Alpha; }
        private readonly List<GhostFrame> _ghosts = new();
        private Texture2D _ghostBodyTex;

        public void AddGhost(Vector2 pos, bool faceLeft) =>
            _ghosts.Add(new GhostFrame { Position = pos, FaceLeft = faceLeft, Alpha = 0.6f });

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

            _state = new standState();
            _state.SetState(this);
        }

        public void ConfigureDeathAnimation(AnimationClip deathClip, Texture2D dotTexture)
        {
            _deathClip = deathClip;
            _deathDotTex = dotTexture;
            BuildGhostTexture();
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
            dashPressed = false;
            deathPressed = false;
            climbHeld = false;
            moveX = 0f;
            moveY = 0f;
            _jumpBufferTimer = 0f;
            _dashBufferTimer = 0f;
            _jumpGraceTimer = 0f;

            velocityY = 0f;
            velocityX = 0f;
            onGround = false;
            hitCeiling = false;

            isClimbing = false;
            isDashing = false;
            isDangle = false;

            touchingLeftWall = false;
            touchingRightWall = false;
        }

        public Vector2 GetDashDirection()
        {
            float x = moveX;
            float y = moveY;

            if (x == 0f && y == 0f)
            {
                x = FaceLeft ? -1f : 1f;
            }

            var dash = new Vector2(x, y);
            if (dash != Vector2.Zero)
            {
                dash.Normalize();
            }

            return dash;
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

            if (deathPressed && _deathClip != null && _deathDotTex != null && _deathEffect == null)
            {
                changeState(deathState);
            }

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

            if (!isDashing)
            {
                position.X += velocityX * dt;
                velocityX = Approach(velocityX, 0f, PlayerDashCarryDeceleration * dt);
            }

            // Gravity & vertical position
            if (!isDashing && !isClimbing && !isDangle)
            {
                if (!onGround) velocityY += PlayerGravity * dt;
                position.Y += velocityY;
            }

          
            ClearTransientInput();
        }

        private void ClearTransientInput()
        {
            jumpPressed = false;
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

            Maddy.Draw(spriteBatch, position, Color.White, scale: DefaultScale, faceLeft: FaceLeft);
        }

        internal void StartDeathEffect(bool wasDashing)
        {
            if (_deathClip == null || _deathDotTex == null)
                return;

            float scale = DefaultScale;
            Vector2 topLeft = position - new Vector2(
                _deathClip.FrameWidth * scale * 0.5f,
                _deathClip.FrameHeight * scale
            );

            Color deathColor = wasDashing ? DashDeathColor : NormalDeathColor;
            _deathEffect = new DeathEffect(_deathClip, _deathDotTex, topLeft, deathColor, scale);
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

        public bool CanGrabWall()
        {
            return climbHeld && IsTouchingWall && !IsTired;
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

        private static float Approach(float value, float target, float maxDelta)
        {
            if (value < target)
            {
                return Math.Min(value + maxDelta, target);
            }

            return Math.Max(value - maxDelta, target);
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
    }
}
