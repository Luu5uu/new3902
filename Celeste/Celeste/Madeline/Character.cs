using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;
using Celeste.Input;
using Celeste.MadelineStates;
using Celeste.Sprites;

using static Celeste.GameConstants;

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

        // Set each frame by input layer via SetMovementCommand; consumed in Update.
        public bool jumpPressed;
        public bool dashPressed;
        public float moveX;

        // Position & facing
        public Vector2 position;
        public bool FaceLeft;
        public float ground;

        // Horizontal movement speed (tuned for visible hair lag at 60fps)
        public float velocity = 200f;

        // Jump / fall
        public float airSpeed  = 200f;
        public float velocityY;
        public float jumpSpeed = 15f;
        public float gravity   = 60f;
        public bool  onGround;

        // Dash
        public bool isDashing;
        public bool canDash = true;

        public Madeline(ContentManager content, AnimationCatalog catalog, Vector2 startPos)
        {
            Maddy    = MaddySprite.Build(content, catalog);
            position = startPos;
            ground   = startPos.Y;
            onGround = true;

            standState = new standState();
            runState   = new runState();
            jumpState  = new jumpState();
            fallState  = new fallState();
            dashState  = new dashState();

            _state = new standState();
            _state.SetState(this);
        }

        public void changeState(IMadelineState next)
        {
            _state = next;
            _state.SetState(this);
        }

        // Called by input layer each frame before Update.
        public void SetMovementCommand(PlayerCommand cmd)
        {
            moveX = cmd.MoveX;
            jumpPressed = cmd.JumpPressed;
            dashPressed = cmd.DashPressed;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _state.Update(this, dt);

            // Gravity & vertical position
            if (!isDashing)
            {
                if (!onGround) velocityY += gravity * dt;
                position.Y += velocityY;
            }

            if (position.Y >= ground)
            {
                position.Y = ground;
                onGround   = true;
                canDash    = true;
                velocityY  = 0f;
            }
            else
            {
                onGround = false;
            }

            jumpPressed = false;

            Maddy.SetPosition(position, scale: DefaultScale, faceLeft: FaceLeft);
            Maddy.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Maddy.Draw(spriteBatch, position, Color.White, scale: DefaultScale, faceLeft: FaceLeft);
        }
    }
}
