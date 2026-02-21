using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Celeste.Animation;
using Celeste.Input;
using Celeste.MadelineStates;
using Celeste.Sprites;

using Celeste.DeathAnimation;
using Celeste.DeathAnimation.Particles;

using static Celeste.GameConstants;
using System;

namespace Celeste.Character
{
    public class Madeline : Celeste.GamePlay.IUpdateable, Celeste.GamePlay.IDrawable
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

        // NEW: DeathState
        public IMadelineState deathState;

        //New
        public IMadelineState climbState;

        // Set each frame by input layer via SetMovementCommand; consumed in Update.
        public bool jumpPressed;
        public bool dashPressed;
        public bool deathPressed; // NEW
        public bool climbHeld;
        public float moveX;

        // Position & facing
        public Vector2 position;
        public bool FaceLeft;
        public float ground;

        // Horizontal movement speed 
        public float velocity = 200f;

        // Jump / fall
        public float airSpeed = 200f;
        public float velocityY;
        public float jumpSpeed = 15f;
        public float gravity = 60f;
        public bool onGround;

        //Climb
        public float climbSpeed = 60f;
        public bool isClimbing;
        // Dash
        public bool isDashing;
        public bool canDash = true;
        //Dangling
        public bool isDangle;
        public float dangleFallSpeed =40f;

        // ===== DeathAnimation integration (DeathEffect already includes sprite+particles) =====
        private AnimationClip _deathClip;
        private Texture2D _deathDotTex;
        private DeathEffect _deathEffect;

        public Madeline(ContentManager content, AnimationCatalog catalog, Vector2 startPos)
        {
            Maddy = MaddySprite.Build(content, catalog);
            position = startPos;
            ground = startPos.Y;
            onGround = true;

            standState = new standState();
            runState = new runState();
            jumpState = new jumpState();
            fallState = new fallState();
            dashState = new dashState();
            dangleState = new dangleState();

            // NEW
            deathState = new DeathState();
            climbState = new climbState();

            _state = new standState();
            _state.SetState(this);
        }

        /// <summary>
        /// Inject DeathAnimation resources (called from Game1 after constructing Madeline).
        /// Keeps Madeline constructor unchanged (fixes CS1729).
        /// </summary>
        public void ConfigureDeathAnimation(AnimationClip deathClip, Texture2D dotTexture)
        {
            _deathClip = deathClip;
            _deathDotTex = dotTexture;
        }

        public void changeState(IMadelineState next)
        {
            _state.Exit(this);
            _state = next;
            _state.SetState(this);
        }

        // Called by input layer each frame before Update.
        public void SetMovementCommand(PlayerCommand cmd)
        {
            moveX = cmd.MoveX;
            jumpPressed = cmd.JumpPressed;
            dashPressed = cmd.DashPressed;
            deathPressed = cmd.DeathPressed; // NEW
            climbHeld = cmd.ClimbHeld;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // ===== Global trigger: do NOT depend on standState transition =====
            // Only allow if resources are configured.
            if (deathPressed && _deathClip != null && _deathDotTex != null)
            {
                // Avoid re-entering DeathState if already playing
                if (_deathEffect == null)
                {
                    changeState(deathState);
                }
            }

            // State logic
            _state.Update(this, dt);

            // If death effect is playing, we generally want to block physics + sprite updates.
            // DeathState itself updates the effect; here we just early-out from normal physics when active.
            if (_deathEffect != null)
            {
                // consume one-frame inputs
                jumpPressed = false;
                dashPressed = false;
                deathPressed = false;
                return;
            }

            // Gravity & vertical position
            if (!isDashing && !isClimbing && !isDangle)
            {
                if (!onGround) velocityY += gravity * dt;
                position.Y += velocityY;
            }

            if (position.Y >= ground)
            {
                position.Y = ground;
                onGround = true;
                velocityY = 0f;
                if (!isDashing)
                {
                    Maddy.OnDashRefill(); // MaddySprite ignores this until blue display timer expires
                    canDash = true;
                }
            }
            else
            {
                onGround = false;
            }

            // consume one-frame inputs
            jumpPressed = false;
            dashPressed = false;
            deathPressed = false;

            Maddy.SetPosition(position, scale: DefaultScale, faceLeft: FaceLeft);
            Maddy.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // If death effect exists, draw it (DeathAnimation folder implementation).
            if (_deathEffect != null)
            {
                _deathEffect.Draw(spriteBatch);
                return;
            }

            Maddy.Draw(spriteBatch, position, Color.White, scale: DefaultScale, faceLeft: FaceLeft);
        }

        // ===== Methods used by DeathState =====
        internal void StartDeathEffect()
        {
            if (_deathClip == null || _deathDotTex == null)
                return;

            float scale = DefaultScale;

            // Assume Madeline.position is bottom-center:
            // topLeft.x = centerX - halfWidth
            // topLeft.y = bottomY - fullHeight
            Vector2 topLeft = position - new Vector2(
                _deathClip.FrameWidth * scale * 0.5f,
                _deathClip.FrameHeight * scale
            );

            _deathEffect = new DeathEffect(_deathClip, _deathDotTex, topLeft, scale);
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
    }
}