using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;

namespace Celeste.Blocks
{
    /// <summary>
    /// Spring block with an idle frame, one-shot activation animation, and a dedicated top collision strip.
    /// </summary>
    public class Spring : IBlocks
    {
        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }
        public Texture2D Texture
        {
            get => _clip.Texture;
            set { }
        }
        public string Type => "spring";
        public float Scale { get; set; } = 2.5f;

        private readonly AnimationClip _clip;
        private Vector2 _position;
        private int _frameIndex;
        private float _frameTimer;
        private bool _isAnimating;

        private const float PlaybackFps = 20f;
        private const int IdleFrameIndex = 0;

        public Spring(Vector2 position, AnimationCatalog catalog, float scale = 2.5f)
        {
            _clip = catalog.Clips[AnimationKeys.DevicesSpring];
            _position = position;
            Scale = scale;
            _frameIndex = IdleFrameIndex;
        }

        public void Activate()
        {
            if (_isAnimating)
            {
                return;
            }

            _isAnimating = true;
            _frameIndex = IdleFrameIndex;
            _frameTimer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            if (!_isAnimating || _clip.FrameCount <= 1)
            {
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (dt <= 0f)
            {
                return;
            }

            float secondsPerFrame = 1f / PlaybackFps;
            _frameTimer += dt;

            while (_frameTimer >= secondsPerFrame)
            {
                _frameTimer -= secondsPerFrame;
                _frameIndex++;

                if (_frameIndex >= _clip.FrameCount)
                {
                    _frameIndex = IdleFrameIndex;
                    _frameTimer = 0f;
                    _isAnimating = false;
                    break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle src = _clip.GetSourceRect(_frameIndex);
            spriteBatch.Draw(_clip.Texture, Position, src, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }

        public Rectangle Bounds
        {
            get
            {
                return Rectangle.Empty;
            }
        }

        public Rectangle TriggerBounds
        {
            get
            {
                if (Texture == null) return Rectangle.Empty;

                int fullWidth = (int)(_clip.FrameWidth * Scale);
                int fullHeight = (int)(_clip.FrameHeight * Scale);
                int xInset = System.Math.Max(1, (int)System.Math.Round(2f * Scale));
                int yOffset = System.Math.Max(0, (int)System.Math.Round(4f * Scale));
                int height = System.Math.Max(8, fullHeight - yOffset);

                return new Rectangle(
                    (int)Position.X + xInset,
                    (int)Position.Y + yOffset,
                    fullWidth - xInset * 2,
                    height
                );
            }
        }
    }
}
