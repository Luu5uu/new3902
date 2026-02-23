using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Celeste.Animation;
using Celeste.Input;
using Celeste.MadelineStates;
using Celeste.Sprites;

using Celeste.DeathAnimation;
using Celeste.DeathAnimation.Particles;

using static Celeste.PlayerConstants;
using static Celeste.GlobalConstants;

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

        public IMadelineState deathState;

        //New
        public IMadelineState climbState;

        // Set each frame by input layer via SetMovementCommand; consumed in Update.
        public bool jumpPressed;
        public bool dashPressed;
        public bool deathPressed;
        public bool climbHeld;
        public float moveX;

        // Position & facing
        public Vector2 position;
        public bool FaceLeft;
        public float ground;

        // Physics (runtime state only — speed constants live in PlayerConstants)
        public float velocityY;
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

            deathState = new DeathState();
            climbState = new climbState();

            _state = new standState();
            _state.SetState(this);
        }

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
            deathPressed = cmd.DeathPressed;
            climbHeld = cmd.ClimbHeld;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (deathPressed && _deathClip != null && _deathDotTex != null && _deathEffect == null)
            {
                changeState(deathState);
            }

            // State logic
            _state.Update(this, dt);

            if (_deathEffect != null)
            {
                jumpPressed = false;
                dashPressed = false;
                deathPressed = false;
                return;
            }

            // Gravity & vertical position
            if (!isDashing && !isClimbing && !isDangle)
            {
                if (!onGround) velocityY += PlayerGravity * dt;
                position.Y += velocityY;
            }

            if (position.Y >= ground)
            {
                position.Y = ground;
                onGround = true;
                velocityY = 0f;
                if (!isDashing)
                {
                    Maddy.OnDashRefill();
                    canDash = true;
                }
            }
            else
            {
                onGround = false;
            }

            jumpPressed = false;
            dashPressed = false;
            deathPressed = false;

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

            Maddy.Draw(spriteBatch, position, Color.White, scale: DefaultScale, faceLeft: FaceLeft);
        }

        internal void StartDeathEffect()
        {
            if (_deathClip == null || _deathDotTex == null)
                return;

            float scale = DefaultScale;
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
