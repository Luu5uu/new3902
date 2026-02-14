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
        Boolean jumpPressed;

        public IMadelineState standState;
        public IMadelineState runState;

        public float ground;
        public Vector2 position;
        public float moveX;
        public float velocity = 100;
        public SpriteEffects effect = SpriteEffects.None;
        

        public Madeline(AnimationCatalog anima, Vector2 startPos)
        {
            _anima = anima;
            position = startPos;
            ground = position.Y;

            standState = new standState();
            runState = new runState();


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


            prev = k;



        }
        public void update(GameTime gameTime)
        {
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;


            setMovX();
            




            _state.update(this, dt);
            _player.update(dt);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            _player.draw(spriteBatch, position, effect);
        }

    }
}
