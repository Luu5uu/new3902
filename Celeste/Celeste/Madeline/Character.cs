using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;
using Celeste.MadelineStates;
using Microsoft.Xna.Framework.Input;


// // This file represents the object of the character itself.
// // So, it should include all general elements about the character such as its current posion, velocity, side it faces to etc..
namespace Celeste.Character
{
    public class Madeline
    {
        // Catalog is used to load all necessary textures
        public AnimationCatalog _anima;
        public AnimationPlayer _player = new AnimationPlayer();
        public IMadelineState _state;

        KeyboardState prev;
        public Boolean jumpPressed;
        public Boolean dashPressed;
        public Boolean isDashing;

        public IMadelineState standState;
        public IMadelineState runState;
        public IMadelineState jumpState;
        public IMadelineState fallState;
        public IMadelineState dashState;

        public float ground;
        public Vector2 position;
        public float moveX;
        public float velocity = 100;
        public SpriteEffects effect = SpriteEffects.None;

        // Test for jump
        public float airSpeed = 200f;
        public float velocityY;
        public float jumpSpeed = 15f;
        public float gravity = 60f;
        public Boolean onGround;
        

        public Madeline(AnimationCatalog anima, Vector2 startPos)
        {
            _anima = anima;
            position = startPos;
            ground = position.Y;
            onGround = true;

            standState = new standState();
            runState = new runState();
            jumpState = new jumpState();
            fallState = new fallState();
            dashState = new dashState();

            _state = new standState();
            _state.setState(this);
        }

        public void changeState(IMadelineState nextState) 
        {
            _state = nextState;
            _state.setState(this);


        }

        public void setMovX()
        {
            var k = Keyboard.GetState();
            moveX = 0f;
            if (k.IsKeyDown(Keys.D)) moveX += 1f;
            else if(k.IsKeyDown(Keys.A)) moveX -= 1f;

            jumpPressed = k.IsKeyDown(Keys.Space) && !prev.IsKeyDown(Keys.Space);
            dashPressed = k.IsKeyDown(Keys.Enter) && !prev.IsKeyDown(Keys.Enter);

            prev = k;



        }


        public void physics(float dt)
        {
            if (!isDashing)
            {
                if (!onGround)
                {
                    velocityY += gravity * dt;

                }
                position.Y += velocityY;
            }



            if(position.Y>= ground)
            {
                position.Y = ground;
                onGround = true;
                velocityY = 0f;
            }
            else
            {
                onGround = false;
            }
        }

        public void update(GameTime gameTime)
        {
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;


            setMovX();
            




            _state.update(this, dt);
            physics(dt);
            
            _player.update(dt);
            jumpPressed = false;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            _player.draw(spriteBatch, position, effect);
        }

    }
}
