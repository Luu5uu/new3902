using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;
using Celeste.Items;

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
        public float Scale { get; set; } = 2.0f;

        private readonly ItemAnimation _animation;
        private readonly Vector2 _start;
        private readonly Vector2 _end;
        private readonly Vector2 _direction;
        private float _speed;
        private bool _forwardCheck = true;

        public MoveBlock(Vector2 start, float distance, float speed, float angleDegrees, AnimationCatalog catalog, float scale = 2.5f)
        {
            var clip = catalog.Clips[AnimationKeys.DevicesMoveBlock];
            _animation = new ItemAnimation(clip);
            float angleRad = MathHelper.ToRadians(angleDegrees);
            _speed = speed;
            _direction = new Vector2((float)Math.Cos(angleRad), (float)Math.Sin(angleRad));
            _start = start;
            _end = start + _direction * distance;
            _animation.Position = _start;
            Scale = scale;
        }

        public void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
            float step = _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_forwardCheck)
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

        public void Draw(SpriteBatch spriteBatch) => _animation.Draw(spriteBatch, Position, Scale);
    }
}
