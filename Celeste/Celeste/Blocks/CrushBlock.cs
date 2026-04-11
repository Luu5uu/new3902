using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;

namespace Celeste.Blocks
{
    public class CrushBlock : IBlocks
    {
        private const float WarningFrameTimeSeconds = 2f;
        private const float BrokenFrameTimeSeconds = 4f;
        private const int CollisionTopOffsetPixels = 12;
        private const int CollisionWidthPixels = 32;
        private const int CollisionHeightPixels = 8;

        public Vector2 Position
        { get; set; }

        public Texture2D Texture
        {
            get => _clip.Texture;
            set { }
        }

        public string Type => "crushBlock";
        public float Scale { get; set; } = 2.5f;
        public bool HasCollision => _frameIndex < 2;
        public bool CanStandOn => HasCollision;
        public bool IsActivated => _isActivated;

        private readonly AnimationClip _clip;
        private float _elapsedSeconds;
        private int _frameIndex;
        private bool _isActivated;

        public CrushBlock(Vector2 position, AnimationCatalog catalog, float scale = 2.5f)
        {
            _clip = catalog.Clips[AnimationKeys.DevicesCrushBlock];
            Position = position;
            Scale = scale;
            Reset();
        }

        public void Activate()
        {
            _isActivated = true;
        }

        public void Reset()
        {
            _elapsedSeconds = 0f;
            _frameIndex = 0;
            _isActivated = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!_isActivated || _frameIndex >= 2)
            {
                return;
            }

            _elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_elapsedSeconds >= BrokenFrameTimeSeconds)
            {
                _frameIndex = 2;
            }
            else if (_elapsedSeconds >= WarningFrameTimeSeconds)
            {
                _frameIndex = 1;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _clip.Texture,
                Position,
                _clip.GetSourceRect(_frameIndex),
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }

        public Rectangle Bounds
        {
            get
            {
                if (!HasCollision)
                {
                    return Rectangle.Empty;
                }

                int x = (int)Position.X;
                int y = (int)(Position.Y + CollisionTopOffsetPixels * Scale);
                int w = (int)(CollisionWidthPixels * Scale);
                int h = (int)(CollisionHeightPixels * Scale);

                return new Rectangle(
                    x,
                    y,
                    w,
                    h
                );
            }
        }
    }
}
