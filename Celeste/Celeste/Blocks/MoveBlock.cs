using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;
using Celeste.CollectableItems;
using System;

namespace Celeste.Blocks
{
    public class MoveBlock : IBlocks
    {
        public Vector2 Position
        {
            get => _animation.Position;
            set => _animation.Position = value;
        }
        public Texture2D Texture
        {
            get => _animation.Clip?.Texture;
            set { }
        }

        public string Type => "moveBlock";

        private ItemAnimation _animation;
        // for movement

        private Vector2 _start;
        private Vector2 _end;
        private Vector2 _direction;
        private float _speed = 60f;
        private bool _forwardCheck = true;


        public MoveBlock(Vector2 start, float distance, float speed, float angleD, AnimationCatalog catalog)
        {
            AnimationClip clip = catalog.Clips[AnimationKeys.DevicesMoveBlock];
            _animation = new ItemAnimation(clip);
            // note: check out mathhelper more
            float angleR = MathHelper.ToRadians(angleD);
            _speed = speed;
            _direction = new Vector2((float)Math.Cos(angleR), (float)Math.Sin(angleR));

            _start = start;
            _end = start + _direction * distance;

            _animation.Position = _start;

        }

        public void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
            float step = _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_forwardCheck == true)
            {
                _animation.Position += _direction * step;
                if (Vector2.Dot(_end - _animation.Position, _direction) < 0)
                {
                    _animation.Position = _end;
                    _forwardCheck = false;
                }
            }
            else
            {
                _animation.Position -= _direction * step;
                if (Vector2.Dot(_animation.Position - _start, _direction) < 0)
                {
                    _animation.Position = _start;
                    _forwardCheck = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch);
        }
    }
}
