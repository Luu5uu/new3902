using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Celeste.Animation;
using Celeste.MadelineStates;
using Celeste.Sprites;

namespace Celeste.Character
{
    public class Madeline
    {
        public MaddySprite Maddy { get; private set; }

        // State machine
        public IMadelineState _state;
        public IMadelineState standState;
        public IMadelineState runState;
        public IMadelineState jumpState;
        public IMadelineState fallState;
        public IMadelineState dashState;

        // Input (read each frame by setMovX)
        KeyboardState _prev;
        public bool jumpPressed;
        public bool dashPressed;
        public float moveX;

        // Position & facing
        public Vector2 position;
        public bool FaceLeft;
        public float ground;

        // Horizontal movement speed
        public float velocity = 100f;

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
            _state.setState(this);
        }

        public void changeState(IMadelineState next)
        {
            _state = next;
            _state.setState(this);
        }

        public void update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var kb = Keyboard.GetState();
            moveX = 0f;
            if (kb.IsKeyDown(Keys.D)) moveX += 1f;
            else if (kb.IsKeyDown(Keys.A)) moveX -= 1f;

            jumpPressed = kb.IsKeyDown(Keys.Space) && !_prev.IsKeyDown(Keys.Space);
            dashPressed = kb.IsKeyDown(Keys.Enter)  && !_prev.IsKeyDown(Keys.Enter);
            _prev = kb;

            _state.update(this, dt);

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

            Maddy.SetPosition(position, scale: 2f, faceLeft: FaceLeft);
            Maddy.Update(gameTime);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            Maddy.Draw(spriteBatch, position, Color.White, scale: 2f, faceLeft: FaceLeft);
        }
    }
}
